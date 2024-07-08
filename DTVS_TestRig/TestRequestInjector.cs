using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTVS_TestRig
{
    class TestRequestInjector
    {
        public String Customer { get; set; }
        public String JobCode { get; set; }
        public String EmailID { get; set; }
        public String Mobile { get; set; }
        public String OEM { get; set; }
        public String Model { get; set; }
        public String PartNumber { get; set; }
        public String InjectorType { get; set; }
        public String NumberOfInjectors { get; set; }
        public String SerialNumberInj1{ get; set; }
        public String SerialNumberInj2 { get; set; }
        public String SerialNumberInj3 { get; set; }
        public String SerialNumberInj4 { get; set; }
        public String ExistingI3CInj1 { get; set; }
        public String ExistingI3CInj2 { get; set; }
        public String ExistingI3CInj3 { get; set; }
        public String ExistingI3CInj4 { get; set; }
        public String TestType { get; set; }
    }
}
