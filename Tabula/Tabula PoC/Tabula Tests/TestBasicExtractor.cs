using System;
using java.lang;
using java.util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using technology.tabula;
using technology.tabula.extractors;

namespace Tabula_Tests
{
    [TestClass]
    public class TestBasicExtractor
    {
        [TestMethod]
        public void TestRemoveSequentialSpaces()
        {
            Page page = UtilsForTesting.GetAreaFromFirstPage(
                UtilsForTesting.defaultPath + "m27.pdf",
                79.2f,
                28.28f, 
                103.04f, 
                732.6f);
            BasicExtractionAlgorithm bea = new BasicExtractionAlgorithm();
            Table table = (Table) bea.extract(page).get(0);
            var firstRow = table.getRows().get(0);
            Console.WriteLine(firstRow.ToString());
            var typedFirstRow = (java.util.ArrayList)firstRow;
            Assert.IsTrue(typedFirstRow.get(1).ToString().Contains("ALLEGIANT AIR"));
            Assert.IsTrue(typedFirstRow.get(2).ToString().Contains("ALLEGIANT AIR LLC"));
        }
    }
}