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
                InterlockSubscription = new Models.Exchange.Subscription
                {
                    Id = theModel.InterlockSubscriptionId,
                    Value = theModel.InterlockSubscriptionReadyValue
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
