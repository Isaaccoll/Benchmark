using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkRpi
{
    class memory
    {
        double runSecs = 0.1;
        int n1;

        double xd;
        double yd;
        float xs;
        float ys;
        double secs;
        int xi;
        int yi;

        private void sampleTime()
        {
            if (secs < runSecs)
            {
                if (secs < runSecs / 8.0)
                {
                    n1 = n1 * 10;
                }
                else
                {
                    n1 = (int)(runSecs * 1.25 / secs * (double)n1 + 1);
                }
            }
        }

        public void memorySample()
        {

        }

        private void initializeProgram()
        {

        }

    }
}
