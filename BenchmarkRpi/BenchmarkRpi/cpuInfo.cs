using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace BenchmarkRpi
{
    class cpuInfo
    {

        private int _cpuInfomation;
        private String _operationSystem;
        private String _versionNumber;
      

        public int CpuInfomation
        {
            get
            {
                return _cpuInfomation;
            }
        
        }

        public string OperationSystem
        {
            get{ return _operationSystem;}
        }

        public string VersionNumber
        {
            get { return _versionNumber;}
        }

        public void systemInfomation()
        {
            
            var deviceInfo = new EasClientDeviceInformation();
            _cpuInfomation = Environment.ProcessorCount;
            _operationSystem = deviceInfo.OperatingSystem;
            _versionNumber = deviceInfo.SystemHardwareVersion;
        }
    }
}
