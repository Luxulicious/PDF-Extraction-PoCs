using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using technology.tabula;
using technology.tabula.extractors;
using String = System.String;

namespace Tabula_Tests
{
    [TestClass]
    public class TestBasicExtractor
    {
        #region Expected

        private static readonly String[,] ArgentinaDiputadosVotingRecordExpected = new String[,]
        {
            {
                "ABDALA de MATARAZZO, Norma Amanda", "Frente Cívico por Santiago", "Santiago del Estero", "AFIRMATIVO"
            },
            {"ALBRIEU, Oscar Edmundo Nicolas", "Frente para la Victoria - PJ", "Rio Negro", "AFIRMATIVO"},
            {"ALONSO, María Luz", "Frente para la Victoria - PJ", "La Pampa", "AFIRMATIVO"},
            {"ARENA, Celia Isabel", "Frente para la Victoria - PJ", "Santa Fe", "AFIRMATIVO"},
            {"ARREGUI, Andrés Roberto", "Frente para la Victoria - PJ", "Buenos Aires", "AFIRMATIVO"},
            {"AVOSCAN, Herman Horacio", "Frente para la Victoria - PJ", "Rio Negro", "AFIRMATIVO"},
            {"BALCEDO, María Ester", "Frente para la Victoria - PJ", "Buenos Aires", "AFIRMATIVO"},
            {"BARRANDEGUY, Raúl Enrique", "Frente para la Victoria - PJ", "Entre Ríos", "AFIRMATIVO"},
            {"BASTERRA, Luis Eugenio", "Frente para la Victoria - PJ", "Formosa", "AFIRMATIVO"},
            {"BEDANO, Nora Esther", "Frente para la Victoria - PJ", "Córdoba", "AFIRMATIVO"},
            {"BERNAL, María Eugenia", "Frente para la Victoria - PJ", "Jujuy", "AFIRMATIVO"},
            {"BERTONE, Rosana Andrea", "Frente para la Victoria - PJ", "Tierra del Fuego", "AFIRMATIVO"},

            {"BIANCHI, María del Carmen", "Frente para la Victoria - PJ", "Cdad. Aut. Bs. As.", "AFIRMATIVO"},
            {"BIDEGAIN, Gloria Mercedes", "Frente para la Victoria - PJ", "Buenos Aires", "AFIRMATIVO"},
            {"BRAWER, Mara", "Frente para la Victoria - PJ", "Cdad. Aut. Bs. As.", "AFIRMATIVO"},
            {"BRILLO, José Ricardo", "Movimiento Popular Neuquino", "Neuquén", "AFIRMATIVO"},
            {"BROMBERG, Isaac Benjamín", "Frente para la Victoria - PJ", "Tucumán", "AFIRMATIVO"},
            {"BRUE, Daniel Agustín", "Frente Cívico por Santiago", "Santiago del Estero", "AFIRMATIVO"},
            {"CALCAGNO, Eric", "Frente para la Victoria - PJ", "Buenos Aires", "AFIRMATIVO"},
            {"CARLOTTO, Remo Gerardo", "Frente para la Victoria - PJ", "Buenos Aires", "AFIRMATIVO"},
            {"CARMONA, Guillermo Ramón", "Frente para la Victoria - PJ", "Mendoza", "AFIRMATIVO"},

            {"CATALAN MAGNI, Julio César", "Frente para la Victoria - PJ", "Tierra del Fuego", "AFIRMATIVO"},
            {"CEJAS, Jorge Alberto", "Frente para la Victoria - PJ", "Rio Negro", "AFIRMATIVO"},
            {"CHIENO, María Elena", "Frente para la Victoria - PJ", "Corrientes", "AFIRMATIVO"},
            {"CIAMPINI, José Alberto", "Frente para la Victoria - PJ", "Neuquén", "AFIRMATIVO"},

            {"CIGOGNA, Luis Francisco Jorge", "Frente para la Victoria - PJ", "Buenos Aires", "AFIRMATIVO"},
            {"CLERI, Marcos", "Frente para la Victoria - PJ", "Santa Fe", "AFIRMATIVO"},
            {"COMELLI, Alicia Marcela", "Movimiento Popular Neuquino", "Neuquén", "AFIRMATIVO"},
            {"CONTI, Diana Beatriz", "Frente para la Victoria - PJ", "Buenos Aires", "AFIRMATIVO"},
            {"CORDOBA, Stella Maris", "Frente para la Victoria - PJ", "Tucumán", "AFIRMATIVO"},
            {"CURRILEN, Oscar Rubén", "Frente para la Victoria - PJ", "Chubut", "AFIRMATIVO"}
        };

        #endregion

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
            var firstRowObjArray = ((java.util.ArrayList) table.getRows().get(0)).toArray();
            var firstRow = new List<RectangularTextContainer>();
            foreach (var o in firstRowObjArray)
            {
                firstRow.Add((RectangularTextContainer) o);
            }

            Assert.IsTrue(firstRow[1].getText().Equals("ALLEGIANT AIR"));
            Assert.IsTrue(firstRow[2].getText().Equals("ALLEGIANT AIR LLC"));
        }

        [TestMethod]
        public void TestColumnRecognition()
        {
            Page page = UtilsForTesting.GetAreaFromFirstPage(
                UtilsForTesting.defaultPath + "argentina_diputados_voting_record.pdf", 269.875f, 12.75f, 790.5f, 561f);
            BasicExtractionAlgorithm bea = new BasicExtractionAlgorithm();
            Table table = (Table) bea.extract(page).get(0);
            var tableRowArray = UtilsForTesting.TableToArrayOfRows(table);
            Assert.IsTrue(ArgentinaDiputadosVotingRecordExpected.Length.Equals(tableRowArray.Length));
        }
    }
}