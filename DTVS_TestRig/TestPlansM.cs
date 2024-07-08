using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTVS_TestRig
{
    public class TestPlansM
    {
        public TestPlansM()
        {
            this.testplans = new TestDetailsArray();
        }
       
        public TestDetailsArray testplans { get; set; }

    }
}
