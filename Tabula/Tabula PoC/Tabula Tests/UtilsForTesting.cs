//Ported from: https://github.com/tabulapdf/tabula-java/blob/master/src/test/java/technology/tabula/UtilsForTesting.java

using System.Collections.Generic;
using java.io;
using java.lang;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.apache.pdfbox.pdmodel;
using technology.tabula;

namespace Tabula_Tests
{
    public class UtilsForTesting
    {
        public static string defaultPath =
            "C:\\Users\\t.ruijs\\Documents\\GitHub\\PDF-Extraction-PoCs\\Example PDFs\\";


        public static Page GetAreaFromFirstPage(string path, float top, float left, float bottom, float right)
        {
            return GetAreaFromPage(path, 1, top, left, bottom, right);
        }

        public static Page GetAreaFromPage(string path, int page, float top, float left, float bottom, float right)
        {
            return GetPage(path, page).getArea(top, left, bottom, right);
        }

        public static Page GetPage(string path, int pageNumber)
        {
            ObjectExtractor oe = null;
            try
            {
                var document = PDDocument
                    .load(new File(path));
                oe = new ObjectExtractor(document);
                var page = oe.extract(pageNumber);
                return page;
            }
            finally
            {
                if (oe != null)
                    oe.close();
            }
        }

        public static string[,] TableToArrayOfRows(Table table)
        {
            var tableRows = ((Table) table).getRows();
            var maxColCount = 0;

            for (var i = 0; i < tableRows.size(); i++)
            {
                var row = ((java.util.ArrayList) tableRows.get(i)).toArray();
                if (maxColCount < row.Length) maxColCount = row.Length;
            }

            Assert.IsTrue(maxColCount.Equals(table.getColCount()), maxColCount + " =/= " + table.getColCount());

            var rv = new string[tableRows.size(), maxColCount];

            for (var i = 0; i < tableRows.size(); i++)
            {
                var rowObjArray = ((java.util.ArrayList) table.getRows().get(i)).toArray();
                var row = new List<RectangularTextContainer>();
                foreach (var o in rowObjArray) row.Add((RectangularTextContainer) o);

                for (var j = 0; j < row.Count; j++) rv[i, j] = table.getCell(i, j).getText();
            }

            return rv;
        }

        public static string LoadJson(string path)
        {
            var stringBuilder = new StringBuilder();

            try
            {
                using (var reader =
                    new BufferedReader(new InputStreamReader(new FileInputStream(path), "UTF-8")))
                {
                    string line = null;
                    while ((line = reader.readLine()) != null) stringBuilder.append(line);
                }
            }
            catch (IOException e)
            {
                Assert.IsTrue(false, "Could not load JSON.");
            }

            return stringBuilder.toString();
        }
    }
}