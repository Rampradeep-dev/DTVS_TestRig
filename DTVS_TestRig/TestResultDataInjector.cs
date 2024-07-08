using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTVS_TestRig
{
    public class TestResultDataInjector
    {
        public string Slno { get; set; }
        public string StepNo { get; set; }
        public string TestPhase { get; set; }
        public string TestDescription { get; set; }
        public string Speed { get; set; }
        public string RailPressure { get; set; }
        public string RailPressure_FeedBack { get; set; }
        public string InletTemp { get;set; }
        public string Pulse { get; set; }
        public string ScaleLength { get; set; }
        public string Volume { get; set; }
        public string LogDate { get; set; }

    }
}
