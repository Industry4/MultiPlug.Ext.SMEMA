using System.Linq;
using MultiPlug.Base.Http;
using MultiPlug.Base.Attribute;
using MultiPlug.Ext.SMEMA.Models.Settings.MachineReady;
using MultiPlug.Ext.SMEMA.Models.Components.MachineReady;

namespace MultiPlug.Ext.SMEMA.Controllers.Settings.MachineReady
{
    [Route("lane/machine-ready")]
    public class MachineReadyController : SettingsApp
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
                Model = new MachineReadyModel
                {
                    Guid = LaneSearch.Guid,
                    Properties = LaneSearch.MachineReady,
                },
                Template = Templates.SettingsMachineReady
            };
        }

        public Response Post(MachineReadyUpdateModel theModel)
        {
            var LaneSearch = Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid == theModel.Guid);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            MachineReadyProperties Properties = new MachineReadyProperties
            {
                SMEMAMachineReadySubscription = new Models.Exchange.Subscription
                {
                    Id = theModel.SMEMAMachineReadySubscriptionId,
                    Value = theModel.SMEMAMachineReadySubscriptionReadyValue
                },
                SMEMABoardAvailableEvent = new Base.Exchange.Event
                {
                    Id = theModel.SMEMABoardAvailableEventId,
                    Description = theModel.SMEMABoardAvailableEventDescription,
                    Subjects = new string[] { theModel.SMEMABoardAvailableEventSubject }
                },
                SMEMAFailedBoardAvailableEvent = new Base.Exchange.Event
                {
                    Id = theModel.SMEMAFailedBoardAvailableEventId,
                    Description = theModel.SMEMAFailedBoardAvailableEventDescription,
                    Subjects = new string[] { theModel.SMEMAFailedBoardAvailableEventSubject }
                },
                SMEMAMachineReadyAlways = theModel.SMEMAMachineReadyAlways
            };

            LaneSearch.MachineReady.UpdateProperties(Properties);

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = Context.Referrer
            };
        }
    }
}
