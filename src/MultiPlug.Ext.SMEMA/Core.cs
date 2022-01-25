using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using MultiPlug.Base;
using MultiPlug.Ext.SMEMA.Components.Lane;
using MultiPlug.Ext.SMEMA.Models.Components.Lane;
using MultiPlug.Ext.SMEMA.Models.Load;

namespace MultiPlug.Ext.SMEMA
{
    internal class Core : MultiPlugBase
    {
        private static Core m_Instance = null;

        [DataMember]
        public LaneComponent[] Lanes { get; private set; } = new LaneComponent[0];

        public static Core Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new Core();
                }
                return m_Instance;
            }
        }

        private Core()
        {
        }

        internal void LaneAdd(string theLaneId, string theMachineName)
        {
            string NewLaneGuid = Guid.NewGuid().ToString();

            LaneComponent NewLane = new LaneComponent(NewLaneGuid);
            NewLane.UpdateProperties(new LaneProperties { Guid = NewLaneGuid, LaneId = theLaneId, MachineId = theMachineName });

            List<LaneComponent> LanesList = Lanes.ToList();
            LanesList.Add(NewLane);
            Lanes = LanesList.ToArray();
        }

        internal void LaneDelete(LaneComponent theLane)
        {
            List<LaneComponent> LanesList = Lanes.ToList();
            LanesList.Remove(theLane);
            Lanes = LanesList.ToArray();
        }

        internal void Load(LoadRoot theConfiguration)
        {
            if (theConfiguration.Lanes != null)
            {
                List<LaneComponent> NewLanes = new List<LaneComponent>();

                foreach (var Lane in theConfiguration.Lanes)
                {
                    if( string.IsNullOrEmpty(Lane.Guid))
                    {
                        continue;
                    }

                    LaneComponent NewLane = new LaneComponent(Lane.Guid);

                    NewLane.UpdateProperties(new LaneProperties { LaneId = Lane.LaneId, MachineId = Lane.MachineId });

                    if (Lane.BoardAvailable != null)
                    {
                        NewLane.BoardAvailable.UpdateProperties(Lane.BoardAvailable);
                    }

                    if (Lane.MachineReady != null)
                    {
                        NewLane.MachineReady.UpdateProperties(Lane.MachineReady);
                    }

                    if (Lane.Interlock != null)
                    {
                        NewLane.Interlock.UpdateProperties(Lane.Interlock);
                    }

                    NewLanes.Add(NewLane);
                }

                Lanes = NewLanes.ToArray();
            }
        }
    }
}
