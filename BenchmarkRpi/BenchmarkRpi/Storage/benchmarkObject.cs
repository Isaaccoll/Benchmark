using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkRpi.Storage
{
    [DataContract]
    public class benchmarkObject
    {
        [DataMember]
        public float MIPS { get; set; }
        
        [DataMember]
        public float MFLOPS { get; set; }

        [DataMember]
        public List<DataStored> DataBase { get; set; }

    }
    [DataContract]
    public class DataStored
    {
        [DataMember]
        public string OperationName { get; set; }

        [DataMember]
        public String Version { get; set; }
    }
}
