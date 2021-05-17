using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BeamOptimizer.Enums;
using BeamOptimizer.FileReaders;
using BeamOptimizer.Models;
using BeamOptimizer.Optimizer;

namespace BeamOptimizer
{
    public class BeamOptimizerProcess
    {
        public BeamOptimizerProcess(string projectName, string grondStoffenPath, string stukkenPath, string targetPath, ITextReader<Grondstof> grondstofReader, ITextReader<Stuk> stukReader)
        {
            ProjectName = projectName;
            GrondStoffenPath = grondStoffenPath;
            StukkenPath = stukkenPath;
            TargetPath = targetPath;
            GrondstofReader = grondstofReader;
            StukReader = stukReader;
        }


        public string ProjectName { get; set; }
        public string GrondStoffenPath { get; private set; }
        public string StukkenPath { get; private set; }
        public string TargetPath { get; private set; }

        public ITextReader<Grondstof> GrondstofReader { get; private set; }
        public ITextReader<Stuk> StukReader { get; private set; }


        public  void Run()
        {
            try
            {

                var gronsdtoffen = GetGrondstoffen(GrondstofReader);

                var stukken = GetStukken(StukReader);

                var OptimizedBeams = new BeamOptimizerLogic().GetBeams(stukken, gronsdtoffen);
                var grouping = GetGroupedBeams(OptimizedBeams);

                Console.WriteLine("");
                Console.WriteLine("//////// TOTAAL ////////");
                WriteNuttigVsAfval(OptimizedBeams);
                Console.WriteLine("-----------------------");
                Console.WriteLine("");
                Console.WriteLine("");


                Console.WriteLine("//////// SOM ////////");
                WriteTotalBeamResult(grouping);
                Console.WriteLine("-----------------------");
                Console.WriteLine("");
                Console.WriteLine("");

                Console.WriteLine("//////// DETAIL ////////");
                WriteResult(OptimizedBeams);
                Console.WriteLine("-----------------------");
                Console.WriteLine("");
                Console.WriteLine("");

                WriteZaaglijstCsv(OptimizedBeams);
                WriteOrderListCsv(grouping);

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

        }

        private  List<Grondstof> GetGrondstoffen(ITextReader<Grondstof> grondstofReader)
        {
            try
            {
                 var grondstoffen = grondstofReader.ReadFile(GrondStoffenPath);

                if (grondstoffen == null)
                {
                    Console.WriteLine("Geen producten gevonden");
                    return null;
                }

                Console.WriteLine($"Er zijn {grondstoffen.Count} grondstoffen gevonden in CSV");

                return grondstoffen;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private  List<Stuk> GetStukken(ITextReader<Stuk> stukReader)
        {
            try
            {

                var stukken = stukReader.ReadFile(StukkenPath);

                if (stukken == null)
                {
                    Console.WriteLine("Geen producten gevonden");
                    return null;
                }

                Console.WriteLine($"Er zijn {stukken.Count} stukken in stuklijst gevonden in CSV");

                return stukken;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private  void WriteResult(List<Beam> OptimizedBeams)
        {
            var OptimizedSorted = OptimizedBeams.OrderBy(x => x.SelectedBeam.Houtsoort).ThenBy(x => x.SelectedBeam.Lengte).ToList();

            foreach (var gronstofItem in OptimizedBeams)
            {
                Console.WriteLine($"III - BALK: {gronstofItem.ToString()}");
                Console.WriteLine($"Aantal Stukken: {gronstofItem.Stukken.Count}");
                var stukkenSorted = gronstofItem.Stukken.OrderBy(x => x.Lengte).ToList();

                foreach (var stukItem in stukkenSorted)
                {
                    Console.WriteLine($"{stukItem.ToString()}");
                }

                Console.WriteLine("-----------------------------------");
            }
        }

        private  void WriteNuttigVsAfval(List<Beam> Beams)
        {
            int lengteNuttig = 0;
            int lengteAfval = 0;

            foreach (var item in Beams)
            {
                lengteNuttig += item.UsedLengte;
                lengteAfval += item.AfvalLengte;
            }

            Console.WriteLine($"Totaal nuttige lengte Materiaal: {lengteNuttig / 1000}m");
            Console.WriteLine($"Totaal Afval: {lengteAfval / 1000}m");
        }

        private  void WriteTotalBeamResult(List<BeamGroup> grouping)
        {
            foreach (var item in grouping)
            {
                Console.WriteLine($"{item.Beams.Count()} X {item.Beam.Houtsoort} - {item.Beam.Breedte}x{item.Beam.Hoogte}x{item.Beam.Lengte}");
            }
        }

        private  void WriteZaaglijstCsv(List<Beam> Beams)
        {
            var beams = Beams.OrderBy(x => x.SelectedBeam.Breedte).ThenBy(x => x.SelectedBeam.Hoogte).ThenBy(x => x.SelectedBeam.Lengte).ToList();

            var MaxStukkenPerBalk = beams.Max(x => x.Stukken.Count);

            using (StreamWriter doc = new StreamWriter($"{TargetPath}\\{ProjectName}_Zaaglijst.csv"))
            {
                var headers = $"BeamID;Houtsoort;Breedte;Hoogte;TOTALE LENGTE;GEBRUIKTE LENGTE;OVERSCHOT";

                for (int i = 1; i <= MaxStukkenPerBalk; i++)
                // Add kolom headers
                {
                    headers += $";Stuk {i}";
                }

                doc.WriteLine($"{headers}");

                for (int i = 0; i < beams.Count(); i++)
                // Alle beams aflopen met details
                {
                    var Line = $"{i};{(HoutsoortEnum)beams[i].SelectedBeam.Houtsoort};{beams[i].SelectedBeam.Breedte};{beams[i].SelectedBeam.Hoogte};{beams[i].SelectedBeam.Lengte};{beams[i].UsedLengte};{beams[i].AfvalLengte}";

                    foreach (var item in beams[i].Stukken)
                    {
                        // Vul kolommen aan per stuk
                        Line += $";{item.Lengte}";
                    }

                    doc.WriteLine(Line);
                }
            }
        }

        private  void WriteOrderListCsv(List<BeamGroup> grouping)
        {
            using (StreamWriter doc = new StreamWriter($"{TargetPath}\\{ProjectName}_OrderList.csv"))
            {
                doc.WriteLine($"AANTAL;Houtsoort;Breedte;Hoogte;Lengte;EP Excl;EP Incl;Tot Excl;Tot Incl;");

                foreach (var item in grouping)
                {
                    var aantal = item.Beams.Count().ToString();
                    var houtsoort = (HoutsoortEnum)item.Beam.Houtsoort;

                    var breedte = item.Beam.Breedte;
                    var hoogte = item.Beam.Hoogte;

                    var TotaleLengte = item.Beam.Lengte.ToString();

                    var EpExl = item.Beams.FirstOrDefault().SelectedBeam.PrijsExcl.ToString();
                    var EpIncl = item.Beams.FirstOrDefault().SelectedBeam.PrijsIncl.ToString();
                    var TotExl = (item.Beams.FirstOrDefault().SelectedBeam.PrijsExcl * item.Beams.Count()).ToString();
                    var TotIncl = (item.Beams.FirstOrDefault().SelectedBeam.PrijsIncl * item.Beams.Count()).ToString();

                    // TODO afronden naar 2 dec
                    // TODO comma wordt punt


                    doc.WriteLine($"{aantal};{houtsoort};{breedte};{hoogte};{TotaleLengte};{EpExl};{EpIncl};{TotExl};{TotIncl};");
                }
            }
        }

        private static List<BeamGroup> GetGroupedBeams(List<Beam> beams)
        // Groeperen van beams op basis van dezelfde soort, breedte, hoogte en lengte
        {
            var beamsSortedBySize = beams.OrderBy(x => x.SelectedBeam.Breedte).ThenBy(x => x.SelectedBeam.Hoogte).ThenBy(x => x.SelectedBeam.Lengte).ToList();

            return beamsSortedBySize
                .GroupBy(x => new { x.SelectedBeam.Houtsoort, x.SelectedBeam.Breedte, x.SelectedBeam.Hoogte, x.SelectedBeam.Lengte })
                .Select(g => new BeamGroup()
                {
                    Beams = g.OrderBy(x => x.SelectedBeam.Lengte).ToList()
                })
                .ToList();
        }


    }
}
