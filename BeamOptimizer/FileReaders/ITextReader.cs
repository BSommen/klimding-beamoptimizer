using System;
using System.Collections.Generic;
using System.Text;

namespace BeamOptimizer.FileReaders
{
  public interface ITextReader<ReturnObject>
    {
        List<ReturnObject> ReadFile(string path);
    }
}
