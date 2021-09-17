using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Components.Lane;
using MultiPlug.Ext.SMEMA.Models.Settings.Lane;

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
                    LoggingLevel = 0        // TODO
                },
                Template = Templates.SettingsLane,
               // Subscriptions = new Subscription[] { new Subscription { Id = LaneSearch.LogEventId } }
            };
        }
    }
}
