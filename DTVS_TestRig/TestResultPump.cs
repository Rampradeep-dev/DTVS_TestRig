using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTVS_TestRig
{
    public class TestResultPump
    {
        public TestResultPump() 
        { 
            this.customerDetails=new TestResultCustomerDetailsPump();
            this.partDetails=new TestResultPartDetailsPump();
            this.dataPump = new List<TestResultDataPump>();
            this.status=new TestResultStatus();
        }
        public TestResultCustomerDetailsPump customerDetails { get; set; }
        public TestResultPartDetailsPump partDetails { get; set; }
        public List<TestResultDataPump> dataPump { get; set; }
        public TestResultStatus status { get; set; }
    }
}
