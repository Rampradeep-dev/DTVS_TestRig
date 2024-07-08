using System.Collections.Generic;

namespace DTVS_TestRig
{
    public class TestDetailsArrayInj
    {
        public TestDetailsArrayInj()
        {
            this.Part_data = new PartData();
            this.alarm_configuration = new AlarmConfig();
            this.test_plan = new List<InjectorFullTestPlan>();
        }
        public PartData Part_data { get; set; }
        public AlarmConfig alarm_configuration { get; set; }
        public List<InjectorFullTestPlan> test_plan { get; set; }
    }
}