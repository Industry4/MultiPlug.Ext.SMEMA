using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Models.Settings.Interlock;
using MultiPlug.Ext.SMEMA.Models.Components.Interlock;
using MultiPlug.Ext.SMEMA.Components.Lane;

namespace MultiPlug.Ext.SMEMA.Controllers.Settings.Interlock
{
    [Route("lane/interlock")]
    public class InterlockController : SettingsApp
    {
        public Response Get(string id)
        {
            var LaneSearch = Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid == id);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            return new Response
            {
                Model = new InterlockModel
                {
                    Guid = LaneSearch.Guid,
                    Properties = LaneSearch.Interlock
                },
                Template = Templates.SettingsInterlock
            };
        }

        public Response Post(InterlockUpdateModel theModel)
        {
            LaneComponent LaneSearch = Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid == theModel.Guid);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }


            InterlockProperties Properties = new InterlockProperties
            {
                StartupMachineReady = theModel.StartupMachineReady,
                StartupGoodBoard = theModel.StartupGoodBoard,
                StartupBadBoard = theModel.StartupBadBoard,
                StartupFlipBoard = theModel.StartupFlipBoard,
                PermissionInterfaceUI = theModel.PermissionInterfaceUI,
                PermissionInterfaceREST = theModel.PermissionInterfaceREST,
                PermissionInterfaceSubscriptions = theModel.PermissionInterfaceSubscriptions,
                TriggerBlockGoodBoardOnMachineNotReady = theModel.TriggerBlockGoodBoardOnMachineNotReady,
                TriggerBlockBadBoardOnMachineNotReady = theModel.TriggerBlockBadBoardOnMachineNotReady,
                TriggerBlockFlipBoardOnMachineNotReady = theModel.TriggerBlockFlipBoardOnMachineNotReady,
                TriggerBlockFlipBoardOnGoodBoardNotAvailable = theModel.TriggerBlockFlipBoardOnGoodBoardNotAvailable,
                TriggerBlockFlipBoardOnBadBoardNotAvailable = theModel.TriggerBlockFlipBoardOnBadBoardNotAvailable,
                DelayFlipThenBoardAvailable = theModel.DelayFlipThenBoardAvailable,
                MachineReadyInterlockSubscription = new Models.Exchange.MachineReadyAndFlipInterlockSubscription
                {
                    Id = theModel.MachineReadyInterlockSubscriptionId,
                    Block = theModel.MachineReadyInterlockSubscriptionBlock,
                    Unblock = theModel.MachineReadyInterlockSubscriptionUnblock,
                    LatchOn = theModel.MachineReadyInterlockSubscriptionLatchOn,
                    LatchOff = theModel.MachineReadyInterlockSubscriptionLatchOff
                },
                GoodBoardInterlockSubscription = new Models.Exchange.GoodBadInterlockSubscription
                {
                    Id = theModel.GoodBoardInterlockSubscriptionId,
                    Block = theModel.GoodBoardInterlockSubscriptionBlock,
                    Unblock = theModel.GoodBoardInterlockSubscriptionUnblock,
                    LatchOn = theModel.GoodBoardInterlockSubscriptionLatchOn,
                    LatchOff = theModel.GoodBoardInterlockSubscriptionLatchOff,
                    DivertOn = theModel.GoodBoardInterlockSubscriptionDivertOn,
                    DivertOff = theModel.GoodBoardInterlockSubscriptionDivertOff,
                    DivertLatchOn = theModel.GoodBoardInterlockSubscriptionDivertLatchOn,
                    DivertLatchOff = theModel.GoodBoardInterlockSubscriptionDivertLatchOff,
                    UnblockFlipOn = theModel.GoodBoardInterlockSubscriptionUnblockFlipOn,
                    BlockFlipOff = theModel.GoodBoardInterlockSubscriptionBlockFlipOff,
                    DivertOnFlipOn = theModel.GoodBoardInterlockSubscriptionDivertOnFlipOn,
                    DivertOffFlipOff = theModel.GoodBoardInterlockSubscriptionDivertOffFlipOff
                },
                BadBoardInterlockSubscription = new Models.Exchange.GoodBadInterlockSubscription
                {
                    Id = theModel.BadBoardInterlockSubscriptionId,
                    Block = theModel.BadBoardInterlockSubscriptionBlock,
                    Unblock = theModel.BadBoardInterlockSubscriptionUnblock,
                    LatchOn = theModel.BadBoardInterlockSubscriptionLatchOn,
                    LatchOff = theModel.BadBoardInterlockSubscriptionLatchOff,
                    DivertOn = theModel.BadBoardInterlockSubscriptionDivertOn,
                    DivertOff = theModel.BadBoardInterlockSubscriptionDivertOff,
                    DivertLatchOn = theModel.BadBoardInterlockSubscriptionDivertLatchOn,
                    DivertLatchOff = theModel.BadBoardInterlockSubscriptionDivertLatchOff,
                    UnblockFlipOn = theModel.BadBoardInterlockSubscriptionUnblockFlipOn,
                    BlockFlipOff = theModel.BadBoardInterlockSubscriptionBlockFlipOff,
                    DivertOnFlipOn = theModel.BadBoardInterlockSubscriptionDivertOnFlipOn,
                    DivertOffFlipOff = theModel.BadBoardInterlockSubscriptionDivertOffFlipOff
                },
                FlipBoardInterlockSubscription = new Models.Exchange.MachineReadyAndFlipInterlockSubscription
                {
                    Id = theModel.FlipBoardInterlockSubscriptionId,
                    Block = theModel.FlipBoardInterlockSubscriptionBlock,
                    Unblock = theModel.FlipBoardInterlockSubscriptionUnblock,
                    LatchOn = theModel.FlipBoardInterlockSubscriptionLatchOn,
                    LatchOff = theModel.FlipBoardInterlockSubscriptionLatchOff
                },
                MachineReadyBlockEvent = new Models.Exchange.Event
                {
                    Id = theModel.MachineReadyBlockEventId,
                    Description = theModel.MachineReadyBlockEventDescription,
                    Subjects = new string[] {theModel.MachineReadyBlockEventSubject, "smema"},
                    BlockedEnabled = theModel.MachineReadyBlockEventBlockedEnabled,
                    BlockedValue = theModel.MachineReadyBlockEventBlockedValue,
                    UnblockedEnabled = theModel.MachineReadyBlockEventUnblockedEnabled,
                    UnblockedValue = theModel.MachineReadyBlockEventUnblockedValue
                },
                GoodBoardBlockEvent = new Models.Exchange.Event
                {
                    Id = theModel.GoodBoardBlockEventId,
                    Description = theModel.GoodBoardBlockEventDescription,
                    Subjects = new string[] { theModel.GoodBoardBlockEventSubject, "smema"},
                    BlockedEnabled = theModel.GoodBoardBlockEventBlockedEnabled,
                    BlockedValue = theModel.GoodBoardBlockEventBlockedValue,
                    UnblockedEnabled = theModel.GoodBoardBlockEventUnblockedEnabled,
                    UnblockedValue = theModel.GoodBoardBlockEventUnblockedValue
                },
                BadBoardBlockEvent = new Models.Exchange.Event
                {
                    Id = theModel.BadBoardBlockEventId,
                    Description = theModel.BadBoardBlockEventDescription,
                    Subjects = new string[] { theModel.BadBoardBlockEventSubject, "smema" },
                    BlockedEnabled = theModel.BadBoardBlockEventBlockedEnabled,
                    BlockedValue = theModel.BadBoardBlockEventBlockedValue,
                    UnblockedEnabled = theModel.BadBoardBlockEventUnblockedEnabled,
                    UnblockedValue = theModel.BadBoardBlockEventUnblockedValue
                },
                FlipBoardBlockEvent = new Models.Exchange.Event
                {
                    Id = theModel.FlipBoardBlockEventId,
                    Description = theModel.FlipBoardBlockEventDescription,
                    Subjects = new string[] { theModel.FlipBoardBlockEventSubject, "smema" },
                    BlockedEnabled = theModel.FlipBoardBlockEventBlockedEnabled,
                    BlockedValue = theModel.FlipBoardBlockEventBlockedValue,
                    UnblockedEnabled = theModel.FlipBoardBlockEventUnblockedEnabled,
                    UnblockedValue = theModel.FlipBoardBlockEventUnblockedValue
                }

            };

            LaneSearch.Interlock.UpdateProperties(Properties);

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = Context.Referrer
            };
        }


    }
}
