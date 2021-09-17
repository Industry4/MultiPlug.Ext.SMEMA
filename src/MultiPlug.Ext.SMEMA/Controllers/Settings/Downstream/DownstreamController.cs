using System.Linq;
using MultiPlug.Base.Http;
using MultiPlug.Base.Attribute;
using MultiPlug.Ext.SMEMA.Models.Settings.Downstream;
using MultiPlug.Ext.SMEMA.Models.Components.Downstream;

namespace MultiPlug.Ext.SMEMA.Controllers.Settings.Downstream
{
    [Route("lane/downstream")]
    public class DownstreamController : SettingsApp
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
                Model = new DownstreamModel
                {
                    Guid = LaneSearch.Guid,
                    Properties = LaneSearch.Downstream,
                },
                Template = Templates.SettingsDownstream
            };
        }

        public Response Post(DownstreamUpdateModel theModel)
        {
            var LaneSearch = Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid == theModel.Guid);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }


            ushort SMEMAMachineReadySubscriptionSubjectIndex = 0;
            ushort.TryParse(theModel.SMEMAMachineReadySubscriptionSubjectIndex, out SMEMAMachineReadySubscriptionSubjectIndex);

            DownstreamProperties Properties = new DownstreamProperties
            {
                SMEMAMachineReadySubscription = new Models.Exchange.Subscription
                {
                    Id = theModel.SMEMAMachineReadySubscriptionId,
                    Subjects = new ushort[] { SMEMAMachineReadySubscriptionSubjectIndex },
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

            };

            LaneSearch.Downstream.UpdateProperties(Properties);

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = Context.Referrer
            };
        }
    }
}
