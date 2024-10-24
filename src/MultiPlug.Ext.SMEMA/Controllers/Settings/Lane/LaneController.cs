﻿using System.Linq;
using System.Collections.Generic;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Components.Lane;
using MultiPlug.Ext.SMEMA.Models.Settings.Lane;
using MultiPlug.Ext.SMEMA.Models.Components.Lane;
using MultiPlug.Ext.SMEMA.Models.Components.BeaconTower;
using MultiPlug.Ext.SMEMA.Models.Exchange;

namespace MultiPlug.Ext.SMEMA.Controllers.Settings.Lane
{
    [Route("lane")]
    public class LaneController : SettingsApp
    {
        public Response Get(string id)
        {
            LaneComponent LaneSearch = Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid == id);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            return new Response
            {
                Model = new LaneModel
                {
                    Guid = id,
                    LaneId = LaneSearch.LaneId,
                    MachineId = LaneSearch.MachineId,
                    Log = string.Empty,     // TODO
                    LoggingLevel = 0,       // TODO
                    RightToLeft = LaneSearch.RightToLeft,
                    BeaconTowerEvents = LaneSearch.BeaconTower.BeaconTowerEvents
                },
                Template = Templates.SettingsLane,
               // Subscriptions = new Subscription[] { new Subscription { Id = LaneSearch.LogEventId } }
            };
        }

        public Response Post(LanePost theModel)
        {
            var LaneSearch = Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid == theModel.Guid);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            if (theModel.NewBeaconTowerEventId != null)
            {
                var NewEvents = new List<BeaconTowerEvent>();

                for (int i = 0; i < theModel.NewBeaconTowerEventId.Length; i++)
                {
                    NewEvents.Add(new BeaconTowerEvent
                    {
                        Id = theModel.NewBeaconTowerEventId[i],
                        Description = theModel.NewBeaconTowerEventDescription[i],
                        Subjects = new string[] { "value" },
                        Group = theModel.Guid
                    });
                }

                LaneSearch.BeaconTower.UpdateProperties(new BeaconTowerProperties
                {
                    BeaconTowerEvents = NewEvents.ToArray()
                });
            }

            var LaneProps = new LaneProperties
            {
                LaneId = theModel.LaneId,
                MachineId = theModel.MachineId,
                LoggingLevel = theModel.LoggingLevel,
                RightToLeft = theModel.RightToLeft
            };

            LaneSearch.UpdateProperties(LaneProps);

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = Context.Referrer
            };
        }
    }
}
