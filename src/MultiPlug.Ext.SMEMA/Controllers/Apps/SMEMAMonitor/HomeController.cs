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
                    new Subscription { Guid = "USReady", Id = LaneSearch.BoardAvailable.SMEMAMachineReadyEvent.Id },
                    new Subscription { Guid = "USGood", Id = LaneSearch.BoardAvailable.SMEMABoardAvailableSubscription.Id },
                    new Subscription { Guid = "USBad", Id = LaneSearch.BoardAvailable.SMEMAFailedBoardAvailableSubscription.Id },
                    new Subscription { Guid = "USFlip", Id = LaneSearch.BoardAvailable.SMEMAFlipBoardSubscription.Id },
                    new Subscription { Guid = "ILReady", Id = LaneSearch.Interlock.MachineReadyEvent.Id },
                    new Subscription { Guid = "ILGood", Id = LaneSearch.Interlock.GoodBoardEvent.Id },
                    new Subscription { Guid = "ILBad", Id = LaneSearch.Interlock.BadBoardEvent.Id },
                    new Subscription { Guid = "ILFlip", Id = LaneSearch.Interlock.FlipBoardEvent.Id },
                    new Subscription { Guid = "DSReady", Id = LaneSearch.MachineReady.SMEMAMachineReadySubscription.Id },
                    new Subscription { Guid = "DSGood", Id = LaneSearch.MachineReady.SMEMABoardAvailableEvent.Id },
                    new Subscription { Guid = "DSBad", Id = LaneSearch.MachineReady.SMEMAFailedBoardAvailableEvent.Id },
                    new Subscription { Guid = "DSFlip", Id = LaneSearch.MachineReady.SMEMAFlipBoardEvent.Id }
                },
                Model = new Models.Apps.SMEMAMonitor
                {
                    LaneGuid = LaneSearch.Guid,
                    MachineName = LaneSearch.MachineId,
                    LaneName = LaneSearch.LaneId,
                    RightToLeft = LaneSearch.RightToLeft,
                    UIEnabled = LaneSearch.Interlock.PermissionInterfaceUI,

                    Lanes = Core.Instance.Lanes.Select(Lane => new Models.Apps.LaneUrl { Guid = Lane.Guid, MachineName = Lane.MachineId, LaneName = Lane.LaneId }).ToArray(),

                    SMEMABoardAvailableAlways = LaneSearch.BoardAvailable.SMEMABoardAvailableAlways.Value,
                    SMEMAFailedBoardAvailableAlways = LaneSearch.BoardAvailable.SMEMAFailedBoardAvailableAlways.Value,
                    SMEMAFlipBoardAlways = LaneSearch.BoardAvailable.SMEMAFlipBoardAlways.Value,
                    SMEMAMachineReadyAlways = LaneSearch.MachineReady.SMEMAMachineReadyAlways.Value,

                    SubscriptionTrueValues = new Models.Apps.TrueValues
                    {
                        USReady = LaneSearch.BoardAvailable.SMEMAMachineReadyEvent.HighValue,
                        USGood = LaneSearch.BoardAvailable.SMEMABoardAvailableSubscription.Value,
                        USBad = LaneSearch.BoardAvailable.SMEMAFailedBoardAvailableSubscription.Value,
                        USFlip = LaneSearch.BoardAvailable.SMEMAFlipBoardSubscription.Value,
                        DSReady = LaneSearch.MachineReady.SMEMAMachineReadySubscription.Value,
                        DSGood = LaneSearch.MachineReady.SMEMABoardAvailableEvent.HighValue,
                        DSBad = LaneSearch.MachineReady.SMEMAFailedBoardAvailableEvent.HighValue,
                        DSFlip = LaneSearch.MachineReady.SMEMAFlipBoardEvent.HighValue
                    },
                    SMEMABoardAvailableStates = new Models.Apps.ComponentStates
                    {
                        MachineReady = LaneSearch.BoardAvailable.StateMachine.MachineReadyState,
                        GoodBoard = LaneSearch.BoardAvailable.StateMachine.GoodBoardAvailableState,
                        BadBoard = LaneSearch.BoardAvailable.StateMachine.BadBoardAvailableState,
                        FlipBoard = LaneSearch.BoardAvailable.StateMachine.FlipBoardState

                    },
                    SMEMAInterlockStates = new Models.Apps.ComponentStates
                    {
                        MachineReady = LaneSearch.Interlock.MachineReadyStateMachine.MachineReady,
                        GoodBoard = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoard,
                        BadBoard = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoard,
                        FlipBoard = LaneSearch.Interlock.BoardAvailableStateMachine.FlipBoard
                    },
                    SMEMAInterlockLatchedStates = new Models.Apps.ComponentStates
                    {
                        MachineReady = LaneSearch.Interlock.MachineReadyStateMachine.Latch,
                        GoodBoard = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoardLatch,
                        BadBoard = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoardLatch,
                        FlipBoard = LaneSearch.Interlock.BoardAvailableStateMachine.FlipBoardLatch
                    },

                    SMEMAInterlockDivertStates = new Models.Apps.ComponentStates
                    {
                        GoodBoard = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoardDivert,
                        BadBoard = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoardDivert
                    },

                    SMEMAInterlockDivertLatchedStates = new Models.Apps.ComponentStates
                    {
                        GoodBoard = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoardDivertLatch,
                        BadBoard = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoardDivertLatch
                    },

                    SMEMAMachineReadyStates = new Models.Apps.ComponentStates
                    {
                        MachineReady = LaneSearch.MachineReady.StateMachine.MachineReadyState,
                        GoodBoard = LaneSearch.MachineReady.StateMachine.GoodBoardAvailableState,
                        BadBoard = LaneSearch.MachineReady.StateMachine.BadBoardAvailableState,
                        FlipBoard = LaneSearch.MachineReady.StateMachine.FlipBoardState,
                    }
                },
                Template = Templates.AppsSMEMAMonitorHome
            };
        }
    }
}
