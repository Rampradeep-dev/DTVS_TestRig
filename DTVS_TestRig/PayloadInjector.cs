using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTVS_TestRig
{
    public class PayloadInjector
    {
        public PayloadInjector()
        {
            this.payload = new List<TestMInj>();
        }
        public List<TestMInj> payload { get; set; }
        public string is_new_config { get; set; }
        public string correlation_id { get; set; }
    }
}
