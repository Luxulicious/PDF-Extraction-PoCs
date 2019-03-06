using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using BuildTablesFromPdf.Engine;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;

namespace HelloWorld
{
    /// <summary>
    /// This sample is the obligatory Hello World program.
    /// </summary>
    class Program
    {
        public static string defaultPath = "C:/Users/t.ruijs/Documents/GitHub/PDF-Extraction-PoCs/Example PDFs/factuur 2018-0137.pdf";


        static void Main(string[] args)
        {
            /*Console.WriteLine("Native extraction:");
            WriteTextUsingPdfSharpNative();
            Console.WriteLine("BuildTablesFromPdf extraction:");
            WriteTextUsingBuildTablesFromPdf();
            Console.WriteLine(PdfTextExtractor.GetText(defaultPath));*/
            PDFParser();
            Console.ReadKey();
            
        }

        public static void PDFParser()
        {
            var streamWriter = new StreamWriter("output.txt", false);

            String outputText = "";

            try
            {
                PdfDocument inputDocument = PdfReader.Open(defaultPath, PdfDocumentOpenMode.ReadOnly);

                foreach (PdfPage page in inputDocument.Pages)
                {
                    for (int index = 0; index < page.Contents.Elements.Count; index++)
                    {
                        PdfDictionary.PdfStream stream = page.Contents.Elements.GetDictionary(index).Stream;
                        outputText = new PDFTextExtractor().ExtractTextFromPDFBytes(stream.Value);

                        streamWriter.WriteLine(outputText);
                    }
                }

            }
            catch (Exception e)
            {

            }
            streamWriter.Close();
        }

        private static void WriteTextUsingBuildTablesFromPdf()
        {
            var pages = ContentExtractor.Read(defaultPath);
            foreach (var page in pages)
            {
                page.DetermineTableStructures();
                page.DetermineParagraphs();
                page.FillContent();
                foreach (var content in page.Contents)
                {
                    Console.WriteLine(content);
                }
                
            }
            //var pages = ExtractText.Read(defaultPath);
        }

        private static void WriteTextUsingPdfSharpNative()
        {
            PdfDocument doc =
                PdfReader.Open(
                    defaultPath,
                    PdfDocumentOpenMode.Import);
            Console.WriteLine(doc.Info.Title);
            IEnumerable<string> texts = doc.Pages[0].ExtractText();
            foreach (var text in texts)
            {
                Console.WriteLine(text);
            }
        }
    }


    public static class PdfSharpExtensions
    {
        public static IEnumerable<string> ExtractText(this PdfPage page)
        {
            var content = ContentReader.ReadContent(page);
            var text = content.ExtractText();
            return text;
        }

        public static IEnumerable<string> ExtractText(this CObject cObject)
        {
            if (cObject is COperator)
            {
                var cOperator = cObject as COperator;
                if (cOperator.OpCode.Name == OpCodeName.Tj.ToString() ||
                    cOperator.OpCode.Name == OpCodeName.TJ.ToString())
                {
                    foreach (var cOperand in cOperator.Operands)
                    foreach (var txt in ExtractText(cOperand))
                        yield return txt;
                }
            }
            else if (cObject is CSequence)
            {
                var cSequence = cObject as CSequence;
                foreach (var element in cSequence)
                foreach (var txt in ExtractText(element))
                    yield return txt;
            }
            else if (cObject is CString)
            {
                var cString = cObject as CString;
                yield return cString.Value;
            }
        }
    }

    public static class PdfTextExtractor
    {
        public static string GetText(string pdfFileName)
        {
            using (var _document = PdfReader.Open(pdfFileName, PdfDocumentOpenMode.ReadOnly))
            {
                var result = new StringBuilder();
                foreach (var page in _document.Pages.OfType<PdfPage>())
                {
                    ExtractText(ContentReader.ReadContent(page), result);
                    result.AppendLine();
                }
                return result.ToString();
            }
        }

        #region CObject Visitor
        private static void ExtractText(CObject obj, StringBuilder target)
        {
            if (obj is CArray)
                ExtractText((CArray)obj, target);
            else if (obj is CComment)
                ExtractText((CComment)obj, target);
            else if (obj is CInteger)
                ExtractText((CInteger)obj, target);
            else if (obj is CName)
                ExtractText((CName)obj, target);
            else if (obj is CNumber)
                ExtractText((CNumber)obj, target);
            else if (obj is COperator)
                ExtractText((COperator)obj, target);
            else if (obj is CReal)
                ExtractText((CReal)obj, target);
            else if (obj is CSequence)
                ExtractText((CSequence)obj, target);
            else if (obj is CString)
                ExtractText((CString)obj, target);
            else
                throw new NotImplementedException(obj.GetType().AssemblyQualifiedName);
        }

        private static void ExtractText(CArray obj, StringBuilder target)
        {
            foreach (var element in obj)
            {
                ExtractText(element, target);
            }
        }
        private static void ExtractText(CComment obj, StringBuilder target) { /* nothing */ }
        private static void ExtractText(CInteger obj, StringBuilder target) { /* nothing */ }
        private static void ExtractText(CName obj, StringBuilder target) { /* nothing */ }
        private static void ExtractText(CNumber obj, StringBuilder target) { /* nothing */ }
        private static void ExtractText(COperator obj, StringBuilder target)
        {
            if (obj.OpCode.OpCodeName == OpCodeName.Tj || obj.OpCode.OpCodeName == OpCodeName.TJ)
            {
                foreach (var element in obj.Operands)
                {
                    ExtractText(element, target);
                }
                target.Append(" ");
            }
        }
        private static void ExtractText(CReal obj, StringBuilder target) { /* nothing */ }
        private static void ExtractText(CSequence obj, StringBuilder target)
        {
            foreach (var element in obj)
            {
                ExtractText(element, target);
            }
        }
        private static void ExtractText(CString obj, StringBuilder target)
        {
            target.Append(obj.Value);
        }
        #endregion
    }

    public class PDFTextExtractor
    {
        /// BT = Beginning of a text object operator 
        /// ET = End of a text object operator
        /// Td move to the start of next line
        ///  5 Ts = superscript
        /// -5 Ts = subscript

        #region Fields

        #region _numberOfCharsToKeep
        /// <summary>
        /// The number of characters to keep, when extracting text.
        /// </summary>
        private static int _numberOfCharsToKeep = 15;
        #endregion

        #endregion



        #region ExtractTextFromPDFBytes
        /// <summary>
        /// This method processes an uncompressed Adobe (text) object 
        /// and extracts text.
        /// </summary>
        /// <param name="input">uncompressed</param>
        /// <returns></returns>
        public string ExtractTextFromPDFBytes(byte[] input)
        {
            if (input == null || input.Length == 0) return "";

            try
            {
                string resultString = "";

                // Flag showing if we are we currently inside a text object
                bool inTextObject = false;

                // Flag showing if the next character is literal 
                // e.g. '\\' to get a '\' character or '\(' to get '('
                bool nextLiteral = false;

                // () Bracket nesting level. Text appears inside ()
                int bracketDepth = 0;

                // Keep previous chars to get extract numbers etc.:
                char[] previousCharacters = new char[_numberOfCharsToKeep];
                for (int j = 0; j < _numberOfCharsToKeep; j++) previousCharacters[j] = ' ';


                for (int i = 0; i < input.Length; i++)
                {
                    char c = (char)input[i];

                    if (inTextObject)
                    {
                        // Position the text
                        if (bracketDepth == 0)
                        {
                            if (CheckToken(new string[] { "TD", "Td" }, previousCharacters))
                            {
                                resultString += "\n\r";
                            }
                            else
                            {
                                if (CheckToken(new string[] { "'", "T*", "\"" }, previousCharacters))
                                {
                                    resultString += "\n";
                                }
                                else
                                {
                                    if (CheckToken(new string[] { "Tj" }, previousCharacters))
                                    {
                                        resultString += " ";
                                    }
                                }
                            }
                        }

                        // End of a text object, also go to a new line.
                        if (bracketDepth == 0 &&
                            CheckToken(new string[] { "ET" }, previousCharacters))
                        {

                            inTextObject = false;
                            resultString += " ";
                        }
                        else
                        {
                            // Start outputting text
                            if ((c == '(') && (bracketDepth == 0) && (!nextLiteral))
                            {
                                bracketDepth = 1;
                            }
                            else
                            {
                                // Stop outputting text
                                if ((c == ')') && (bracketDepth == 1) && (!nextLiteral))
                                {
                                    bracketDepth = 0;
                                }
                                else
                                {
                                    // Just a normal text character:
                                    if (bracketDepth == 1)
                                    {
                                        // Only print out next character no matter what. 
                                        // Do not interpret.
                                        if (c == '\\' && !nextLiteral)
                                        {
                                            nextLiteral = true;
                                        }
                                        else
                                        {
                                            if (((c >= ' ') && (c <= '~')) ||
                                                ((c >= 128) && (c < 255)))
                                            {
                                                resultString += c.ToString();
                                            }

                                            nextLiteral = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Store the recent characters for 
                    // when we have to go back for a checking
                    for (int j = 0; j < _numberOfCharsToKeep - 1; j++)
                    {
                        previousCharacters[j] = previousCharacters[j + 1];
                    }
                    previousCharacters[_numberOfCharsToKeep - 1] = c;

                    // Start of a text object
                    if (!inTextObject && CheckToken(new string[] { "BT" }, previousCharacters))
                    {
                        inTextObject = true;
                    }
                }
                return resultString;
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region CheckToken
        /// <summary>
        /// Check if a certain 2 character token just came along (e.g. BT)
        /// </summary>
        /// <param name="search">the searched token</param>
        /// <param name="recent">the recent character array</param>
        /// <returns></returns>
        private bool CheckToken(string[] tokens, char[] recent)
        {
            foreach (string token in tokens)
            {
                if (token.Length > 1)
                {
                    if ((recent[_numberOfCharsToKeep - 3] == token[0]) &&
                        (recent[_numberOfCharsToKeep - 2] == token[1]) &&
                        ((recent[_numberOfCharsToKeep - 1] == ' ') ||
                        (recent[_numberOfCharsToKeep - 1] == 0x0d) ||
                        (recent[_numberOfCharsToKeep - 1] == 0x0a)) &&
                        ((recent[_numberOfCharsToKeep - 4] == ' ') ||
                        (recent[_numberOfCharsToKeep - 4] == 0x0d) ||
                        (recent[_numberOfCharsToKeep - 4] == 0x0a))
                        )
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }

            }
            return false;
        }
        #endregion
    }
}