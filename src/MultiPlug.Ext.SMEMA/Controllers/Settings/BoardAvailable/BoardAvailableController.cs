using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Models.Settings.BoardAvailable;
using MultiPlug.Ext.SMEMA.Models.Components.BoardAvailable;

namespace MultiPlug.Ext.SMEMA.Controllers.Settings.BoardAvailable
{
    [Route("lane/board-available")]
    public class BoardAvailableController : SettingsApp
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
                Model = new BoardAvailableModel
                {
                    Guid = LaneSearch.Guid,
                    Properties = LaneSearch.BoardAvailable
                },
                Template = Templates.SettingsBoardAvailable
            };
        }

        public Response Post(BoardAvailableUpdateModel theModel)
        {
            var LaneSearch = Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid == theModel.Guid);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            BoardAvailableProperties Properties = new BoardAvailableProperties
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
                    Value = theModel.SMEMABoardAvailableSubscriptionAvailableValue
                },
                SMEMAFailedBoardAvailableSubscription = new Models.Exchange.Subscription
                {
                    Id = theModel.SMEMAFailedBoardAvailableSubscriptionId,
                    Value = theModel.SMEMAFailedBoardAvailableSubscriptionAvailableValue
                },
                SMEMABoardAvailableAlways = theModel.SMEMABoardAvailableAlways,
                SMEMAFailedBoardAvailableAlways = theModel.SMEMAFailedBoardAvailableAlways
            };

            LaneSearch.BoardAvailable.UpdateProperties(Properties);

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = Context.Referrer
            };
        }
    }
}
