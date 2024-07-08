using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTVS_TestRig
{
    public  class TestMInj
    {
        public TestMInj() 
        { 
            this.test= new TestPlansMInj();
        }
        public TestPlansMInj test { get; set; } 
        public string error { get; set; }
    }
}
