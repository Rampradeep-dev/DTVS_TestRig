using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTVS_TestRig
{
    public class Payload
    {
        public Payload() 
        {
            this.payload = new List<TestM>();

        }
        public List<TestM> payload { get; set; }
        public string is_new_config { get; set; }
        public string correlation_id { get; set; }
    }
}
