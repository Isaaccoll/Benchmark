using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BenchmarkRpi
{
    class Whetstone
    {

        private const double t = 0.499975;
        private const double t1 = 0.50025;
        private const double t2 = 2.0;

        private int i, j, k, l, n1, n2, n3, n4, n6, n7, n8, n9, n10, n11;
        private double x1, x2, x3, x4, x, y;
        private double[] e1 = new double[4];
        private double[] z = new double[1];
        private int cycleNo;
        private int iterations;
        private int numberOfCycles;
        private long beginTime;
        private long endTime;
        private Stopwatch sw = new Stopwatch();

       

        public long BeginTime
        {
            get { return beginTime; }
            set { beginTime = value; }
        }

        public long EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        public int ITERATIONS
        {
            get { return iterations; }
            set { iterations = value; }
        }

        public int NumberOfCycles
        {
            get { return numberOfCycles; }
            set { numberOfCycles = value; }
        }


        public double StartCalc()
        {
            /* set values of module weights */
            n1 = 0 * ITERATIONS;
            n2 = 12 * ITERATIONS;
            n3 = 14 * ITERATIONS;
            n4 = 345 * ITERATIONS;
            n6 = 210 * ITERATIONS;
            n7 = 32 * ITERATIONS;
            n8 = 899 * ITERATIONS;
            n9 = 616 * ITERATIONS;
            n10 = 0 * ITERATIONS;
            n11 = 93 * ITERATIONS;

            beginTime = 0;
            sw.Reset();
            sw.Start();

            for (cycleNo = 1; cycleNo <= numberOfCycles; cycleNo++)
            {
                /* MODULE 1: simple identifiers */
                x1 = 1.0;
                x2 = x3 = x4 = -1.0;
                for (i = 1; i <= n1; i += 1)
                {
                    x1 = (x1 + x2 + x3 - x4) * t;
                    x2 = (x1 + x2 - x3 + x4) * t; // correction: x2 = ( x1 + x2 - x3 - x4 ) * t;
                    x3 = (x1 - x2 + x3 + x4) * t; // correction: x3 = ( x1 - x2 + x3 + x4 ) * t;
                    x4 = (-x1 + x2 + x3 + x4) * t;
                }

                /* MODULE 2: array elements */
                e1[0] = 1.0;
                e1[1] = e1[2] = e1[3] = -1.0;
                for (i = 1; i <= n2; i += 1)
                {
                    e1[0] = (e1[0] + e1[1] + e1[2] - e1[3]) * t;
                    e1[1] = (e1[0] + e1[1] - e1[2] + e1[3]) * t;
                    e1[2] = (e1[0] - e1[1] + e1[2] + e1[3]) * t;
                    e1[3] = (-e1[0] + e1[1] + e1[2] + e1[3]) * t;
                }

                /* MODULE 3: array as parameter */
                for (i = 1; i <= n3; i += 1)
                    pa(e1);

                /* MODULE 4: conditional jumps */
                j = 1;
                for (i = 1; i <= n4; i += 1)
                {
                    if (j == 1)
                        j = 2;
                    else
                        j = 3;
                    if (j > 2)
                        j = 0;
                    else
                        j = 1;
                    if (j < 1)
                        j = 1;
                    else
                        j = 0;
                }

                /* MODULE 5: omitted */
                /* MODULE 6: integer arithmetic */
                j = 1;
                k = 2;
                l = 3;
                for (i = 1; i <= n6; i += 1)
                {
                    j = j * (k - j) * (l - k);
                    k = l * k - (l - j) * k;
                    l = (l - k) * (k + j);
                    e1[l - 2] = j + k + l; /* C arrays are zero based */
                    e1[k - 2] = j * k * l;
                }

                /* MODULE 7: trig. functions */
                x = y = 0.5;
                for (i = 1; i <= n7; i += 1)
                {
                    x = t * Math.Atan(t2 * Math.Sin(x) * Math.Cos(x) / (Math.Cos(x + y) + Math.Cos(x - y) - 1.0));
                    y = t * Math.Atan(t2 * Math.Sin(y) * Math.Cos(y) / (Math.Cos(x + y) + Math.Cos(x - y) - 1.0));
                }

                /* MODULE 8: procedure calls */
                x = y = z[0] = 1.0;
                for (i = 1; i <= n8; i += 1)
                    p3(x, y, z);

                /* MODULE9: array references */
                j = 0;
                k = 1;
                l = 2;
                e1[0] = 1.0;
                e1[1] = 2.0;
                e1[2] = 3.0;
                for (i = 1; i <= n9; i++)
                    p0();

                /* MODULE10: integer arithmetic */
                j = 2;
                k = 3;
                for (i = 1; i <= n10; i += 1)
                {
                    j = j + k;
                    k = j + k;
                    j = k - j;
                    k = k - j - j;
                }

                /* MODULE11: standard functions */
                x = 0.75;
                for (i = 1; i <= n11; i += 1)
                    x = Math.Sqrt(Math.Exp(Math.Log(x) / t1));

            }

            sw.Stop();
            endTime = sw.ElapsedMilliseconds;

            return (endTime - beginTime);
        }

       
    

        private void p0()
        {
            e1[j] = e1[k];
            e1[k] = e1[l];
            e1[l] = e1[j];
        }

        private void p3(double x, double y, double[] z)
        {
            x = t * (x + y);
            y = t * (x + y);
            z[0] = (x + y) / t2;
        }

        private void pa(double[] e)
        {
            int j;
            j = 0;
            do
            {
                e[0] = (e[0] + e[1] + e[2] - e[3]) * t;
                e[1] = (e[0] + e[1] - e[2] + e[3]) * t;
                e[2] = (e[0] - e[1] + e[2] + e[3]) * t;
                e[3] = (-e[0] + e[1] + e[2] + e[3]) / t2;
                j += 1;
            }
            while (j < 6);
        }

     

       
    }
}
