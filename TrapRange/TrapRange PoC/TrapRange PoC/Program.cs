using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.giaybac.traprange;
using com.sun.tools.javac.util;
using ikvm.extensions;
using java.io;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.text;
using Console = System.Console;

namespace TrapRange_PoC
{
    class Program
    {

        public static string defaultPath =
            "C:\\Users\\Tom Ruijs\\Documents\\GitHub\\PDF-Extraction-PoCs\\Example PDFs\\";

        static void Main(string[] args)
        {
            Console.WriteLine("\n" + "010003576.pdf");
            Console.WriteLine(WriteTablesToCSV(defaultPath + "010003576.pdf", "010003576.txt"));


            Console.WriteLine("\n" + "2018-0137.pdf");
            Console.WriteLine(WriteTablesToCSV(defaultPath + "2018-0137.pdf", "2018-0137.txt"));
        
            Console.WriteLine("\n" + "factuur 2018-0137.pdf");
            Console.WriteLine(WriteTablesToCSV(defaultPath + "factuur 2018-0137.pdf", "factuur 2018-0137.txt"));


            Console.WriteLine("\n" + "sample.pdf");
            Console.WriteLine(WriteTablesToCSV(defaultPath + "sample.pdf", "sample.txt"));


            Console.ReadLine();
        }

        public static string WriteTablesToCSV(string inputPath, string outputPath)
        {
            var text = GetTablesCSV(inputPath);
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
               writer.WriteLine(text);
            }
            return text;
        }

        public static string GetTablesCSV(string path)
        {
            PDFTableExtractor extractor = new PDFTableExtractor();
            var tables = extractor.setSource(path)
                .addPage(0)
                .extract();
             return tables.get(0).toString();//table in csv format using semicolon as a delimiter 
        }
    }
}
