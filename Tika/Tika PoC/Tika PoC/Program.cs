using System;
using ikvm.extensions;
using java.io;
using org.apache.tika.exception;
using org.apache.tika.language;
using org.apache.tika.metadata;
using org.apache.tika.parser;
using org.apache.tika.parser.pdf;
using org.apache.tika.sax;
using Console = System.Console;

namespace Tika_PoC
{
    class Program
    {
        public static string path =
            "C:/Users/t.ruijs/Documents/GitHub/PDF-Extraction-PoCs/Example PDFs/factuur 2018-0137.pdf";

        static void Main(string[] args)
        {
            //Console.WriteLine(TextExtractor.GetTextAndMetaFromPDF(path));
            Console.WriteLine("Contents of PDF: \n" + TextExtractor.GetPlainTextFromPDF(path));
            Console.WriteLine("\n------------------------------------------\n");
            Console.WriteLine("Detected Language: \n" + TextExtractor.GetLanguageFromPDF(path));
            Console.ReadLine();
        }
    }

    public static class TextExtractor
    {
        public static string GetPlainTextFromPDF(string path)
        {
            BodyContentHandler handler = new BodyContentHandler();
            Metadata metadata = new Metadata();
            FileInputStream inputstream = new FileInputStream(new File(path));
            ParseContext context = new ParseContext();

            //Parsing the document using PDF parser
            PDFParser pdfParser = new PDFParser();
            pdfParser.parse(inputstream, handler, metadata, context);

            return handler.toString();
        }

        //TODO Refactor this with delegate
        public static string GetTextAndMetaFromPDF(string path)
        {
            BodyContentHandler handler = new BodyContentHandler();
            Metadata metadata = new Metadata();
            FileInputStream inputstream = new FileInputStream(new File(path));
            ParseContext context = new ParseContext();

            //Parsing the document using PDF parser
            PDFParser pdfParser = new PDFParser();
            pdfParser.parse(inputstream, handler, metadata, context);


            //Getting the content of the document
            var val = "Content of PDF: \n";
            val = handler.toString();
            val += "\n\n" + "MetaData of PDF: \n";
            foreach (var name in metadata.names())
            {
                val += name + ": " + metadata.get(name).toString() + "\n";
            }

            return val;
        }

        public static string GetLanguageFromPDF(string path)
        {
            var text = GetPlainTextFromPDF(path);
            LanguageIdentifier identifier = new LanguageIdentifier(text);
            return identifier.getLanguage();
        }
    }
}