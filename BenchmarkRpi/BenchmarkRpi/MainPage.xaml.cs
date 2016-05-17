using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BenchmarkRpi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            execute_Linpack_Benchmark();

            execute_Whetstone_Benchmark();
        }

        private void execute_Linpack_Benchmark()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

         Linpack linpackResult = new Linpack();
         cpuInfo computerInfomation = new cpuInfo();
         //busSpeed piBusSpeed = new busSpeed();

         computerInfomation.systemInfomation();

        var results = linpackResult.run_benchmark();

           // var busResults = piBusSpeed.calcPass();

            listView.Items.Add("Linpack Calulations : Double Precision 100x100 compiled at 32 bits");
            listView.Items.Add("");
            listView.Items.Add("Number of logical processers: " + computerInfomation.CpuInfomation);
            listView.Items.Add("Operating System " + computerInfomation.OperationSystem + " " + computerInfomation.VersionNumber);
        
            listView.Items.Add("Linpack MFlops/s per single cpu = " + results.MFlops);
            listView.Items.Add("Number of MFlops/s per multi cpus = " + (results.MFlops * computerInfomation.CpuInfomation));
            listView.Items.Add("Normal Res: " + results.NormRes);
            listView.Items.Add("Time: " + results.Time);
            listView.Items.Add("Return the Precision time = " + results.Precision);
            listView.Items.Add("");
            listView.Items.Add("Whetstone Calulations :");

            
           
        }

        private void execute_Whetstone_Benchmark()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            Whetstone whetstoneResult = new Whetstone();

            whetstoneResult.ITERATIONS = 100;
            whetstoneResult.NumberOfCycles = 100;

            int numberOfRuns = 10;
            float elapsedTime = 0;
            float meanTime = 0;
            float rating = 0;
            float meanRating = 0;
            int intRating = 0;

            for (int runNumber = 1; runNumber <= numberOfRuns; runNumber++)
            {
                // Call the Whetstone benchmark procedure
                // compute elapsed time
                elapsedTime = (float)(whetstoneResult.StartCalc() / 1000);
                this.listView.Items.Add(string.Format("{0}.Test (time for {1} cycles): {2} millisec.", runNumber, whetstoneResult.NumberOfCycles, whetstoneResult.EndTime - whetstoneResult.BeginTime));
                
                // sum time in milliseconds per cycle
                meanTime = meanTime + (elapsedTime * 1000 / whetstoneResult.NumberOfCycles);
                // Calculate the Whetstone rating based on the time for
                // the numbers of cycles just executed
                rating = (1000 * whetstoneResult.NumberOfCycles) / elapsedTime;
                // Sum Whetstone rating
                meanRating = meanRating + rating;
                intRating = (int)rating;
                // Reset no_of_cycles for the next run using ten cycles more
                whetstoneResult.NumberOfCycles += 10;
            }
            meanTime = meanTime / numberOfRuns;
            meanRating = meanRating / numberOfRuns;
            intRating = (int)meanRating;

            this.listView.Items.Add(string.Format("Number of Runs {0}", numberOfRuns));
            this.listView.Items.Add(string.Format("Average time per cycle {0} millisec.", meanTime));
            this.listView.Items.Add(string.Format("Average Whetstone Rating {0} KWIPS", intRating));
            this.listView.Items.Add(string.Format("Average Whetstone Rating {0} MWIPS", intRating / 1000));

        }
    }
}
