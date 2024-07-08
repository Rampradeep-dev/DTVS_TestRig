using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTVS_TestRig
{
    public class AlarmConfig
    {
        public string il_pressure_min { get; set; }
        public string il_pressure_max { get; set; }
        public string bl_pressure_min { get; set; }
        public string bl_pressure_max { get; set; }
        public string il_temp_min { get; set; }
        public string il_temp_max { get; set; }
        public string bl_temp_min { get; set; }
        public string bl_temp_max { get; set; }
        public string cw_temp_min { get; set; }
        public string cw_temp_max { get; set; }
        public string dl_temp_min { get; set; }
        public string dl_temp_max { get; set; }
        public string dt_temp_min { get; set; }
        public string dt_temp_max { get; set; }
        public string tp_min { get; set; }
        public string tp_max { get; set; }
        public string to_temp_min { get; set; }
        public string to_temp_max { get; set; }
        public string rotation { get; set; }
    }
}
