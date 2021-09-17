using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Models.Settings.Upstream;
using MultiPlug.Ext.SMEMA.Models.Components.Upstream;

namespace MultiPlug.Ext.SMEMA.Controllers.Settings.Upstream
{
    [Route("lane/upstream")]
    public class UpstreamController : SettingsApp
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
                Model = new UpstreamModel
                {
                    Guid = LaneSearch.Guid,
                    Properties = LaneSearch.Upstream
                },
                Template = Templates.SettingsUpstream
            };
        }

        public Response Post(UpstreamUpdateModel theModel)
        {
            var LaneSearch = Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid == theModel.Guid);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            ushort SMEMABoardAvailableSubscriptionSubjectIndex = 0;
            ushort.TryParse(theModel.SMEMABoardAvailableSubscriptionSubjectIndex, out SMEMABoardAvailableSubscriptionSubjectIndex);

            ushort SMEMAFailedBoardAvailableSubscriptionSubjectIndex = 0;
            ushort.TryParse(theModel.SMEMAFailedBoardAvailableSubscriptionSubjectIndex, out SMEMAFailedBoardAvailableSubscriptionSubjectIndex);

            UpstreamProperties Properties = new UpstreamProperties
            {
                SMEMAMachineReadyEvent = new Base.Exchange.Event
                {
                    Id = theModel.SMEMAMachineReadyEventId,
                    Description = theModel.SMEMAMachineReadyEventDescription,
                    Subjects = new string[] { theModel.SMEMAMachineReadyEventSubject }
                },
                SMEMABoardAvailableSubscription = new Models.Exchange.Subscription
                {
                    Id = theModel.SMEMABoardAvailableSubscriptionId,
                    Subjects = new ushort[] { SMEMABoardAvailableSubscriptionSubjectIndex }
                },
                SMEMAFailedBoardAvailableSubscription = new Models.Exchange.Subscription
                {
                    Id = theModel.SMEMAFailedBoardAvailableSubscriptionId,
                    Subjects = new ushort[] { SMEMAFailedBoardAvailableSubscriptionSubjectIndex }
                }
            };

            LaneSearch.Upstream.UpdateProperties(Properties);

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = Context.Referrer
            };
        }
    }
}
