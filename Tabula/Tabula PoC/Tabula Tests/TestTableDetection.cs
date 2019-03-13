using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using com.google.gson;
using ikvm.extensions;
using java.io;
using java.lang;
using java.util;
using javax.xml.parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.apache.pdfbox.pdmodel;
using File = System.IO.File;
using org.w3c.dom;
using technology.tabula;
using Object = System.Object;
using String = System.String;
using technology.tabula.detectors;
using Console = System.Console;
using StringBuilder = System.Text.StringBuilder;

namespace Tabula_Tests
{
    /// <summary>
    /// Summary description for TestTableDetection
    /// </summary>
    [TestClass]
    public class TestTableDetection
    {
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        private java.io.File pdf;
        private DocumentBuilder builder;

        public void TableDetection(java.io.File pdf)
        {
            this.pdf = pdf;
            DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
            this.builder = factory.newDocumentBuilder();
        }

        [TestMethod]
        public void TestDetectionOfTables()
        {
            TableDetection(new java.io.File(UtilsForTesting.defaultPath + "2018-0137.pdf"));

            // tabula extractors
            PDDocument pdfDocument = PDDocument.load(this.pdf);
            ObjectExtractor extractor = new ObjectExtractor(pdfDocument);

            // now find tables detected by tabula-java
            Dictionary<Integer, List<Rectangle>> detectedTables = new Dictionary<Integer, List<Rectangle>>();

            // the algorithm we're going to be testing
            NurminenDetectionAlgorithm detectionAlgorithm = new NurminenDetectionAlgorithm();

            PageIterator pages = extractor.extract();
            while (pages.hasNext())
            {
                Page page = pages.next();
                List<Rectangle> tablesOnPage = new List<Rectangle>();
                var tablesOnPageJList = (ArrayList) detectionAlgorithm.detect(page);
                foreach (var o in tablesOnPageJList)
                {
                    tablesOnPage.Add((Rectangle) o);
                }

                if (tablesOnPage.Count > 0)
                {
                    detectedTables.Add(new Integer(page.getPageNumber()), tablesOnPage);
                }
            }

            //TODO Remove this temp list
            Dictionary<Integer, List<Rectangle>> expectedTables = new Dictionary<Integer, List<Rectangle>>();
            expectedTables.Add(new Integer(0), new List<Rectangle>() {new Rectangle(230, 230, 460, 460)});

            #region Compare

            // now compare
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.pdf.getName());
            foreach (var detectedTable in detectedTables)
            {
                sb.AppendLine("Page " + detectedTable.Key.toString());
                detectedTable.Value.ForEach(x =>
                {
                   var points = x.getPoints();
                   var str = "";
                   foreach (var point in points)
                   {
                       str += point + " ";
                   }
                   sb.AppendLine(str);
                });
            }

            Console.WriteLine(sb.ToString());
            Assert.IsTrue(detectedTables.Any(), sb.ToString());

            #endregion
        }
    }
}