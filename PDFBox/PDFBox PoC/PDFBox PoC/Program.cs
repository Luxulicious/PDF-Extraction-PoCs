using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ikvm.extensions;
using java.lang;
using java.util;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using Exception = System.Exception;
using String = System.String;

namespace PDFBox_PoC
{
    class Program
    {
        static void Main(string[] args)
        {
            var path =
                "C:/Users/t.ruijs/Documents/GitHub/PDF-Extraction-PoCs/Example PDFs/factuur 2018-0137.pdf";
            var text = ExtractTextFromPdf(path);
            Console.WriteLine("Extracted text:\n\n" + text + "\n\n");
            Console.WriteLine(GetPDDocument(path).getDocumentCatalog().toString());

            WriteTextFromPDF(path);
            var key = Console.ReadLine();
        }

        private static string ExtractTextFromPdf(string path)
        {
            PDDocument doc = null;
            try
            {
                doc = PDDocument.load(path);
                PDFTextStripper stripper = new PDFTextStripper();
                return stripper.getText(doc);
            }
            finally
            {
                if (doc != null)
                {
                    doc.close();
                }
            }
        }

        private static void WriteTextFromPDF(string path)
        {
            var doc = GetPDDocument(path);
            PDFTextExtractor pdfTextExtractor = new PDFTextExtractor();
            Console.WriteLine(pdfTextExtractor.getText(doc));
        }

        private static PDDocument GetPDDocument(string path)
        {
            PDDocument doc = null;
            try
            {
                doc = PDDocument.load(path);
                return doc;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }
    }
}

public class PDFTextExtractor : PDFTextStripper
{
    protected override void writeString(string str, List textPositions)
    {
        Iterator it = textPositions.iterator();
        while (it.hasNext())
        {
            var text = (TextPosition) it.next();
            Console.WriteLine(
                "["
                + "character=" + text.getCharacter() + " DirAdj="
                + text.getXDirAdj() + "," +
                text.getYDirAdj() + " fs=" + text.getFontSize() + " xscale=" +
                text.getXScale() + " height=" + text.getHeightDir() + " space=" +
                text.getWidthOfSpace() + " width="
                + "]");
        }
    }
}