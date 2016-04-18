using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BenchmarkRpi
{
    class configTime
    {

       public void local_time()
        {
         
            DateTime t = new DateTime();
          
           var localtime = t.
            sprintf(timeday, "%s", asctime(localtime(&t)));
            return;
        }

       public  void getSecs()
        {
            clock_gettime(CLOCK_REALTIME, &tp1);

            theseSecs = tp1.tv_sec + tp1.tv_nsec / 1e9;
            return;
        }

       public  void start_time()
        {
            getSecs();
            startSecs = theseSecs;
            return;
        }

        public void end_time()
        {
            getSecs();
            secs = theseSecs - startSecs;
            millisecs = (int)(1000.0 * secs);
            return;
        }

    }
}
