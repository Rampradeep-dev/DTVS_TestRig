using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTVS_TestRig
{
    public class InjectorFullTestPlan
    {
        public string SlNo { get; set; }
        public string Test_Phase { get; set; }
        public string Test_Description { get; set; }
        public string Step_No { get; set; }
        public string Speed { get; set; }
        public string Tolerance_rpm { get; set; }
        public string Rail_Pressure { get; set; }
        public string Tolerance_bar_RL { get; set; }
        public string Duration { get; set; }
        public string Pulse_Min { get; set; }
        public string Pulse_Max { get; set; }
        public string Increment { get; set; }
        public string Frequency { get; set; }
        public string Shots { get; set; }
        public string Resistance { get; set; }
        public string Tolerance_ohm { get; set; }
        public string Inductance { get; set; }
        public string Tolerance_H { get; set; }
        public string Delivery_Flow_Min { get; set; }
        public string Delivery_Flow_Max { get; set; }
        public string DelFlow_OffSet { get; set; }
        public string Inlet_Pressure { get; set; }
        public string Tol_bar_IL { get; set; }
        public string Interval { get; set; }
        public string BL_Flow_Min { get; set; }
        public string BL_Flow_Max { get; set; }
        public string BLFlow_OffSet { get; set; }
        public string IL_Flow_Min { get; set; }
        public string IL_Flow_Max { get; set; }
        public string ILFlow_OffSet { get; set; }
        public string IMV_Current { get; set; }
        public string Tolerance_mA { get; set; }
        public string Ramp_Time { get; set; }
        public string Sample_AVG { get; set; }
        public string Sample_Interval { get; set; }
    }
}
