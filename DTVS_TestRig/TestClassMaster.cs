using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTVS_TestRig
{
    public class TestClassMaster
    {
        public TestClassMaster()
        {
           // this.part_data = new List<PartData>();
            this.testplans = new List<TestPlan>();
           // this.alarm_configuration=new List<AlarmConfig>();
        }
     //   public List<PartData> part_data { get; set; }
        public List<TestPlan> testplans{ get; set; }
      //  public List<AlarmConfig> alarm_configuration { get; set; }


    }
}
