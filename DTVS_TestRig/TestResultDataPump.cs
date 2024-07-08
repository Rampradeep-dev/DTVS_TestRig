using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTVS_TestRig
{
    public class TestResultDataPump
    {
        public string Slno { get; set; }
        public string StepNo { get; set; }
        public string TestPhase { get; set; }
        public string TestDescription { get; set; }
        public string Speed { get; set; }
        public string RailPressure { get; set; }
        public string IMVCurrent { get; set; }
        public string InletPressure { get; set; }
        public string InletFlow { get; set; }
        public string InletTemp { get; set; }
        public string BLPressure { get; set; }
        public string BLFlow { get; set; }
        public string BLTemp { get; set; }
        public string DeliveryFlow { get; set; }
        public string DeliveryTemp { get; set; }
        public string TransPressure { get; set; }
        public string VenturiPressure { get; set; }
        public string LubeOilPressure { get; set; }
        public string LubeoildTemp { get; set; }
        public string SkinTemp { get; set; }
        public string HPVCurrent { get; set; }
        public string LogDate { get; set; }
    }
}
