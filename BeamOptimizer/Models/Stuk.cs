using BeamOptimizer.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeamOptimizer.Models
{
   public class Stuk
    {
        public Stuk(SpeeltuigTypeEnum speeltuigType, int hoogte, int breedte, int lengte, HoutsoortEnum houtsoort, int zaagverlies)
        {
            SpeeltuigType = speeltuigType;
            Hoogte = hoogte;
            Breedte = breedte;
            Lengte = lengte;
            Houtsoort = houtsoort;
            Zaagverlies = zaagverlies;
        }

        public SpeeltuigTypeEnum SpeeltuigType { get; set; }

        public int Hoogte { get; set; }

        public int Breedte { get; set; }

        public int Lengte { get; set; }

        public int Zaagverlies { get; set; }

        public int TotalLengte
        {
            // Lengte + Zaagsnede(5mm) 
            get { return Lengte + Zaagverlies; }
        }

        public bool Used { get; set; }


        public HoutsoortEnum Houtsoort { get; set; }

        public override string ToString()
        {
            return $"{Houtsoort} - H:{Hoogte} x B:{Breedte} x L:{Lengte} -- Used = {Used}";
        }
    }
}
