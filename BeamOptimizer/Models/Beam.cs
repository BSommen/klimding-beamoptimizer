using BeamOptimizer.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeamOptimizer.Models
{
    public class Beam
    {
        public Beam(Grondstof selectedBeam)
        {
            SelectedBeam = selectedBeam;
            Stukken = new List<Stuk>();
        }

        public Grondstof SelectedBeam { get; set; }
        public List<Stuk> Stukken { get; set; }

        public int UsedLengte
        {
            get {
                var SumUsed = 0;

                foreach (var item in Stukken)
                {
                    SumUsed += item.TotalLengte;
                }
                return SumUsed;
            }
        }


        public int AfvalLengte
        {
            get
            {
                return SelectedBeam.Lengte - UsedLengte;
            }
        }

        public override string ToString()
        {
            return $"{(HoutsoortEnum)SelectedBeam.Houtsoort} //Aantal Stukken: {Stukken.Count} - Kooplengte: {SelectedBeam.Lengte}mm = Used: {UsedLengte}mm + Afval: {AfvalLengte}mm";
        }

    }
}
