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
            
        }
    }
}
