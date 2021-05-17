using System;
using System.Collections.Generic;
using System.Text;

namespace BeamOptimizer.Models
{
    public class BeamGroup
    {
        public Grondstof Beam
        {
            get
            {
                if (Beams == null)
                {
                    return null;
                }

                if (Beams.Count == 0)
                {
                    return null;
                }

                return Beams[0].SelectedBeam;
            }
        }

        public List<Beam> Beams { get; set; }
    }
}
