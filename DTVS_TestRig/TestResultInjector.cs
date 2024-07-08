using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTVS_TestRig
{
    public class TestResultInjector
    {
        public TestResultInjector()
        {
            this.customerDetails = new TestResultCustomerDetailsInjector();
            this.partDetails = new TestResultPartDetailsInjector();
            this.dataPump = new List<TestResultDataInjector>();
            this.status = new TestResultStatus();
        }
        public TestResultCustomerDetailsInjector customerDetails { get; set; }
        public TestResultPartDetailsInjector partDetails { get; set; }
        public List<TestResultDataInjector> dataPump { get; set; }
        public TestResultStatus status { get; set; }
    }
}
