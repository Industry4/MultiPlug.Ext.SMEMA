using System.Linq;
using System.Net;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Components.Lane;

namespace MultiPlug.Ext.SMEMA.Controllers.Settings.ProgrammableEvent
{
    [Route("lane/programmable-event/delete")]
    public class ProgrammableEventDeleteController : SettingsApp
    {
        public Response Post(string id, string eventguid)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(eventguid))
            {
                return new Response
                {
                    StatusCode = (HttpStatusCode)422
                };
            }

            LaneComponent LaneSearch = Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid == id);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = (HttpStatusCode)422
                };
            }

            bool Result = LaneSearch.BeaconTower.DeleteEvent(eventguid);

            if(Result)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            else
            {
                return new Response
                {
                    StatusCode = (HttpStatusCode)422
                };
            }
        }
    }
}
