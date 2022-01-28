using System.Linq;
using MultiPlug.Base.Exchange;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Components.Lane;
using System;

namespace MultiPlug.Ext.SMEMA.Controllers.Apps.SMEMAMonitor
{
    public class HomeController : SMEMAMonitorApp
    {
        public HomeController()
        {
        }

        public Response Get(string id)
        {
            LaneComponent LaneSearch = null;

            if (Core.Instance.Lanes.Length == 0)
            {
                return new Response
                {
                    Subscriptions = new Subscription[0],
                    Model = new Models.Apps.SMEMAMonitor
                    {
                        Lanes = new Models.Apps.LaneUrl[0],
                    },
                    Template = Templates.AppsSMEMAMonitorNotSetup
                };
            }

            if (string.IsNullOrEmpty(id))
            {
                LaneSearch = Core.Instance.Lanes[0];
            }
            else
            {
                LaneSearch = Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid == id);
            }

            if (LaneSearch == null)
            {
                return new Response
                {
                    Subscriptions = new Subscription[0],
                    Model = new Models.Apps.SMEMAMonitor
                    {
                        Lanes = Core.Instance.Lanes.Select(Lane => new Models.Apps.LaneUrl { Guid = Lane.Guid, MachineName = Lane.MachineId, LaneName = Lane.LaneId }).ToArray(),
                    },
                    Template = Templates.AppsSMEMAMonitorLaneNotFound
                };
            }

            return new Response
            {
                Subscriptions = new Subscription[]
                {
                    new Subscription { Guid = Guid.NewGuid().ToString(), Id = LaneSearch.MachineReady.SMEMAMachineReadySubscription.Id },
                    new Subscription { Guid = Guid.NewGuid().ToString(), Id = LaneSearch.BoardAvailable.SMEMABoardAvailableSubscription.Id },
                    new Subscription { Guid = Guid.NewGuid().ToString(), Id = LaneSearch.BoardAvailable.SMEMAFailedBoardAvailableSubscription.Id }
                },
                Model = new Models.Apps.SMEMAMonitor
                {
                    LaneGuid = LaneSearch.Guid,
                    MachineName = LaneSearch.MachineId,
                    LaneName = LaneSearch.LaneId,

                    Lanes = Core.Instance.Lanes.Select(Lane => new Models.Apps.LaneUrl { Guid = Lane.Guid, MachineName = Lane.MachineId, LaneName = Lane.LaneId }).ToArray(),

                    SMEMAMachineReadyState = LaneSearch.MachineReady.StateMachine.MachineReadyState,
                    SMEMAGoodBoardState = LaneSearch.BoardAvailable.StateMachine.GoodBoardAvailableState,
                    SMEMABadBoardState = LaneSearch.BoardAvailable.StateMachine.BadBoardAvailableState,

                    SMEMAMachineReadySubscriptionId = LaneSearch.MachineReady.SMEMAMachineReadySubscription.Id,
                    SMEMABoardAvailableSubscriptionId = LaneSearch.BoardAvailable.SMEMABoardAvailableSubscription.Id,
                    SMEMAFailedBoardAvailableSubscriptionId = LaneSearch.BoardAvailable.SMEMAFailedBoardAvailableSubscription.Id,
                },
                Template = Templates.AppsSMEMAMonitorHome
            };
        }
    }
}
