using System;
using System.Collections.Generic;
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
            //Due to not being able to use type parameters in java IKVM.NET multiple conversions have to happen
            var firstRow = (RectangularTextContainer[])((java.util.ArrayList)table.getRows().get(0)).toArray();
            Assert.IsTrue(firstRow[1].getText().Equals("ALLEGIANT AIR"));
            Assert.IsTrue(firstRow[2].getText().Equals("ALLEGIANT AIR LLC"));
        }
    }
}