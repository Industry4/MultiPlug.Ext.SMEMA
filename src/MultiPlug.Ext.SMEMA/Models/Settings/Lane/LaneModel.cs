using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlug.Ext.SMEMA.Models.Settings.Lane
{
    public class LaneModel
    {
        public string Guid { get; set; }
        public string LaneId { get; set; }
        public string MachineId { get; set; }
        public int LoggingLevel { get; set; }
        public string Log { get; set; }
    }
}
