using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTVS_TestRig
{
    public class TestDetailsArray
    {
        public TestDetailsArray()
        {
            this.Part_data = new PartData();
            this.alarm_configuration = new AlarmConfig();
            this.test_plan = new List<TestPlan>();
        }
        public PartData Part_data { get; set; }
        public AlarmConfig alarm_configuration { get; set; }
        public List<TestPlan> test_plan { get; set; }
    }
}
