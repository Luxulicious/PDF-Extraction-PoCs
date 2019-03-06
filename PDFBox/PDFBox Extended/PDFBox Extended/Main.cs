using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;

namespace PDFBox_Extended
{
    public class Main
    {
        public static string defaultPath =
            "C:/Users/t.ruijs/Documents/GitHub/PDF-Extraction-PoCs/Example PDFs/sample.pdf";

        public static string GetPlainTextFromPdf(string path)
        {
            if (string.IsNullOrEmpty(path))
                path = defaultPath;
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
    }
}
