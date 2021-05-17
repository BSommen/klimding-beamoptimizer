using BeamOptimizer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System;
using ExcelDataReader;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using BeamOptimizer.Enums;

namespace BeamOptimizer.FileReaders
{
    public class GrondstofCsvReader : ITextReader<Grondstof>
    {
        public List<Grondstof> ReadFile(string path)
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {

                        var rowCounter = 0;
                        var tmpHeaders = new List<List<string>>();
                        var grondstoffen = new List<Grondstof>();

                        while (reader.Read()) //Each ROW
                        {
                            if (rowCounter < 1)
                            // Headers verzamelen
                            {
                                // Headers overslaan
                                rowCounter++;
                                continue;
                            }

                            var rowData = GetRowArray(reader);

                            int.TryParse(rowData[0], out int leverancier);
                            int.TryParse(rowData[1], out int houtsoort);

                            int.TryParse(rowData[2], out int hoogte);
                            int.TryParse(rowData[3], out int breedte);
                            int.TryParse(rowData[4], out int lengte);

                            double.TryParse(rowData[5], out double prijsExcl);
                            double.TryParse(rowData[6], out double prijsIncl);
                            int.TryParse(rowData[7], out int beschikbaar);

                            if (beschikbaar == 0)
                            {
                                continue;
                            }

                            var obj = new Grondstof((LeverancierEnum)leverancier, (HoutsoortEnum)houtsoort, hoogte, breedte, lengte, prijsExcl, prijsIncl);

                            grondstoffen.Add(obj);
                        }

                        return grondstoffen;
                        ;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string[] GetRowArray(IExcelDataReader reader)
        {
            var columnData = "";

            for (int column = 0; column < reader.FieldCount; column++)
            {
                columnData += reader.GetValue(column);

                if (column < reader.FieldCount - 1)
                {
                    columnData += ";";
                }
            }

            string[] result = columnData.Split(";");

            return result;
        }
    }
}
