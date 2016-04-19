using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace BenchmarkRpi
{
    class configTime
    {
        double resolution =0.0;      
        double startSecs = 0.0;
        double seconds;
        uint milliseconds = 0;

        public double Seconds
        {
            get { return seconds;}
            set {seconds = value;}
        }

        public uint Milliseconds
        {
            get { return milliseconds;}
            set { milliseconds = value;}
        }

        public void local_time()
        {
            var localtime = DateTime.Now;       
            return;
        }

       public void getSecs()
        {
            // clock_gettime(CLOCK_REALTIME, &tp1);
            resolution = 1E9 / Stopwatch.Frequency;
            return;
        }

       public void start_time()
        {
            getSecs();
            startSecs = resolution;
            return;
        }

        public void end_time()
        {
            getSecs();
            seconds = resolution - startSecs;
            Milliseconds = (uint)(1000.0 * Seconds);
            return;
        }

    }
}
