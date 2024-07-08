using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTVS_TestRig
{
    public class InjectorResultData
    {
        public string SlNo { get;set; }
        public string StepNo { get;set;}
        public string Test_Phase { get;set;}
        public string Test_Description { get;set;}
        public string Speed { get; set;}
        public string Rail_Pressure { get; set;}
        public string Rail_Pressure_Feedback { get; set;}
        public string Inlet_Temp { get; set;}
        public string Pulse { get; set;}
        public string Scale_Length { get;set;}
        public string Volume { get; set;}
        public string LogDate { get; set;}
    }
}
