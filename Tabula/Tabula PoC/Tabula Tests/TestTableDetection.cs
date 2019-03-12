/*using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using com.google.gson;
using ikvm.extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;


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

        public static ICollection<Object[]> data()
        {
            String[] regionCodes = {"eu, us"};

            List<Object[]> data = new List<Object[]>();

            foreach (var regionCode in regionCodes)
            {
                String directoryName = "src/test/resources/technology/tabula/icdar2013-dataset/competition-dataset-" + regionCode + "/";
                java.io.File dir = new java.io.File(directoryName);
            }

            java.io.File[]

            return data;
        }
    }
}*/