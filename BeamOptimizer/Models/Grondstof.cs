using BeamOptimizer.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeamOptimizer.Models
{
    public class Grondstof
    {
        public Grondstof(LeverancierEnum leverancier, HoutsoortEnum houtsoort, int hoogte, int breedte, int lengte, double prijsExcl, double prijsIncl)
        {
            Leverancier = leverancier;
            Houtsoort = houtsoort;
            Hoogte = hoogte;
            Breedte = breedte;
            Lengte = lengte;
            PrijsExcl = prijsExcl;
            PrijsIncl = prijsIncl;
        }

        public LeverancierEnum Leverancier { get; set; }

        public HoutsoortEnum Houtsoort { get; set; }

        public int Hoogte { get; set; }

        public int Breedte { get; set; }

        public int Lengte { get; set; }

        public double PrijsExcl { get; set; }

        public double PrijsIncl { get; set; }

        public override string ToString()
        {
            return $"{Houtsoort} - H:{Hoogte} x B:{Breedte} x L:{Lengte}";
        }
    }
}
