using BeamOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BeamOptimizer.Enums;

namespace BeamOptimizer.Optimizer
{
    public class BeamOptimizerLogic
    {
        private List<Stuk> stukken;

        private List<Grondstof> gronstoffen;

        public List<Beam> GetBeams(List<Stuk> stukkenUnSorted, List<Grondstof> grondstoffenUnsorted)
        {
            stukken = stukkenUnSorted.OrderByDescending(x => x.Lengte).ToList();

            gronstoffen = grondstoffenUnsorted.OrderByDescending(x => x.Lengte).ToList();

            var aantal = stukken.Count;

            var ResultBeams = new List<Beam>();

            foreach (var item in stukken)
            {
                if (item.Used)
                {
                    continue;
                }

                // Zoek naar dichtsbijzijnde grondstof lengte
                var startBeam = GetStartBeam(item);
                item.Used = true;

                // Aanmaken resultaat beam met nesting eerste stuk
                var beam = new Beam(startBeam);
                beam.Stukken.Add(item);

                Stuk nextStuk = null;

                // Vul overige lengte van beam
                do
                {
                    var rest = beam.AfvalLengte;

                    nextStuk = stukken.Where(x => x.Hoogte == item.Hoogte && x.Breedte == item.Breedte && x.TotalLengte <= rest && x.Used == false && x.Houtsoort == item.Houtsoort).FirstOrDefault();

                    if (nextStuk != null)
                    // Er is nog ruimte voor een extra stuk
                    {
                        beam.Stukken.Add(nextStuk);

                        nextStuk.Used = true;
                    }
                } while (nextStuk != null);

                var smallerBeam = GetSmallerBeam(beam);

                if (smallerBeam != null)
                {
                    beam.SelectedBeam = smallerBeam;
                }

                ResultBeams.Add(beam);
            }

            return ResultBeams;

        }

        private Grondstof GetStartBeam(Stuk stuk)
        // Zoek naar een grondstof dat van lengte het dichtste bij de benodige lengte zit
        {
            var result = gronstoffen.Where(x => x.Hoogte == stuk.Hoogte && x.Breedte == stuk.Breedte && x.Lengte >= stuk.TotalLengte && x.Houtsoort == stuk.Houtsoort).FirstOrDefault();

            if (result == null)
            {
                throw new Exception($"Er is geen grondstof van type {((HoutsoortEnum)stuk.Houtsoort).ToString()}, met gewenste lengte:{stuk.Lengte}mm");
            }
            else
            {
                return result;
            }
        }

        private Grondstof GetSmallerBeam(Beam origin)
        {
            return gronstoffen.Where(x => x.Breedte == origin.SelectedBeam.Breedte && x.Hoogte == origin.SelectedBeam.Hoogte && x.Lengte >= origin.UsedLengte && x.Lengte != origin.SelectedBeam.Lengte && x.Houtsoort == origin.SelectedBeam.Houtsoort).FirstOrDefault();
        }
    }
}

