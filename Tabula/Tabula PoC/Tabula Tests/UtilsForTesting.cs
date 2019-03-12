using System.Collections.Generic;
using java.io;
using java.lang;
using java.nio.charset;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.apache.commons.csv;
using org.apache.pdfbox.pdmodel;
using technology.tabula;
using String = System.String;

namespace Tabula_Tests
{
    /*[TestClass]*/
    public class UtilsForTesting
    {
        public static string defaultPath =
            "C:\\Users\\t.ruijs\\Documents\\GitHub\\PDF-Extraction-PoCs\\Example PDFs\\";


        public static Page GetAreaFromFirstPage(String path, float top, float left, float bottom, float right)
        {
            return GetAreaFromPage(path, 1, top, left, bottom, right);
        }

        public static Page GetAreaFromPage(String path, int page, float top, float left, float bottom, float right)
        {
            return GetPage(path, page).getArea(top, left, bottom, right);
        }

        public static Page GetPage(String path, int pageNumber)
        {
            ObjectExtractor oe = null;
            try
            {
                PDDocument document = PDDocument
                    .load(new File(path));
                oe = new ObjectExtractor(document);
                Page page = oe.extract(pageNumber);
                return page;
            }
            finally
            {
                if (oe != null)
                    oe.close();
            }
        }
    }
}