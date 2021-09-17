using System.Runtime.Serialization;

namespace MultiPlug.Ext.SMEMA.Models.Load
{
    public class LoadRoot
    {
        [DataMember]
        public LoadLaneComponent[] Lanes { get; set; }
    }
}
