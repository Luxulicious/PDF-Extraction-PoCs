using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
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
        private static int numTests = 0;
        private static int numPassingTests = 0;
        private static int totalExpectedTables = 0;
        private static int totalCorrectlyDetectedTables = 0;
        private static int totalErroneouslyDetectedTables = 0;

/*
        private class TestStatus
        {
            public int numExpectedTables;
            public int numCorrectlyDetectedTables;
            public int numErroneouslyDetectedTables;
            public bool expectedFailure;

            [NonSerialized] private bool firstRun;
            [NonSerialized] private string pdfFilename;

            public TestStatus(string pdfFilename)
            {
                this.numExpectedTables = 0;
                this.numCorrectlyDetectedTables = 0;
                this.expectedFailure = false;
                this.pdfFilename = pdfFilename;
            }

            /*public static TestStatus load(String pdfFilename)
            {
                TestStatus status;
                try
                {
                    String json = UtilsForTesting.LoadJson(PdfToJsonFilename(pdfFilename));
                    status = new Gson().fromJson(json, TestStatus.class);
                    status.pdfFilename = pdfFilename;
                }
                catch (IOException ioe)
                {
                    status = new TestStatus(pdfFilename);
                    status.firstRun = true;
                }

                return status;
            }#1#

            public bool isFirstRun()
            {
                return this.firstRun;
            }

            private static string PdfToJsonFilename(string pdfFilename)
            {
                return pdfFilename.replace(".pdf", ".json");
            }
        }
*/

/*        public static ICollection<Object[]> data()
        {
            String[] regionCodes = {"eu, us"};
            List<Object[]> data = new List<Object[]>();
            var pdfs = new List<java.io.File>();
            foreach (var regionCode in regionCodes)
            {
                String directoryName = "src/test/resources/technology/tabula/icdar2013-dataset/competition-dataset-" +
                                       regionCode + "/";
                java.io.File dir = new java.io.File(directoryName);

                if (dir.getName().endsWith(".pdf"))
                    pdfs.Add(dir);
            }

            foreach (var pdf in pdfs)
            {
                data.Add(new Object[] {pdf});
            }

            return data;
        }*/

        private java.io.File pdf;

        private DocumentBuilder builder;
        /*
                private TestStatus status;
        */


        public TestTableDetection(java.io.File pdf)
        {
            this.pdf = pdf;
            DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
            this.builder = factory.newDocumentBuilder();
        }

        [TestMethod]
        public void TestDetectionOfTables()
        {
/*            numTests++;*/
            // xml parsing stuff for ground truth
            Document regionDocument = this.builder.parse(this.pdf.getAbsolutePath().replace(".pdf", "-reg.xml"));
            NodeList tables = regionDocument.getElementsByTagName("table");

            // tabula extractors
            PDDocument pdfDocument = PDDocument.load(this.pdf);
            ObjectExtractor extractor = new ObjectExtractor(pdfDocument);

            // parse expected tables from the ground truth dataset
            Dictionary<Integer, List<Rectangle>> expectedTables = new Dictionary<Integer, List<Rectangle>>();

            var numExpectedTables = 0;

            for (int i = 0; i < tables.getLength(); i++)
            {
                Element table = (Element) tables.item(i);
                Element region = (Element) table.getElementsByTagName("region").item(0);
                Element boundingBox = (Element) region.getElementsByTagName("bounding-box").item(0);

                // we want to know where tables appear in the document - save the page and areas where tables appear
                Integer page = Integer.decode(region.getAttribute("page"));
                float x1 = Float.parseFloat(boundingBox.getAttribute("x1"));
                float y1 = Float.parseFloat(boundingBox.getAttribute("y1"));
                float x2 = Float.parseFloat(boundingBox.getAttribute("x2"));
                float y2 = Float.parseFloat(boundingBox.getAttribute("y2"));

                var pageTables = expectedTables[page];
                if (pageTables == null)
                {
                    pageTables = new List<Rectangle>();
                    expectedTables.Add(page, pageTables);
                }

                // have to invert y co-ordinates
                // unfortunately the ground truth doesn't contain page dimensions
                // do some extra work to extract the page with tabula and get the dimensions from there
                Page extractedPage = extractor.extract(page.intValue()); //extractor.extractPage(page);

                float top = (float) extractedPage.getHeight() - y2;
                float left = x1;
                float width = x2 - x1;
                float height = y2 - y1;

                pageTables.Add(new Rectangle(top, left, width, height));
                numExpectedTables++;
            }

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

            // now compare
            Console.WriteLine("Testing " + this.pdf.getName());

            List<String> results = new List<String>();
            this.status.numExpectedTables = numExpectedTables;
            totalExpectedTables += numExpectedTables;

            foreach (Integer page in expectedTables.Keys)
            {
                List<Rectangle> expectedPageTables = expectedTables[page];
                List<Rectangle> detectedPageTables = detectedTables[page];

                if (detectedPageTables == null)
                {
                    results.Add("Page " + page.toString() + ": " + expectedPageTables.Count +
                                " expected tables not found");
                    continue;
                }

                results.AddRange(this.ComparePages(page, detectedPageTables, expectedPageTables));

                detectedTables.Remove(page);
            }
        }

        private List<String> ComparePages(Integer page, List<Rectangle> detectedPageTables,
            List<Rectangle> expectedPageTables)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Page:");
            sb.AppendLine(page.toString());
            sb.AppendLine("Detected:");
            detectedPageTables.ForEach(x => sb.AppendLine(x.toString()));
            sb.AppendLine("expectedPageTables:");
            expectedPageTables.ForEach(x => sb.AppendLine(x.toString()));
            var str = sb.ToString();
            Console.WriteLine(str);
            Assert.IsTrue(expectedPageTables.Equals(detectedPageTables));
            return new List<String>() {str};
        }
    }
}