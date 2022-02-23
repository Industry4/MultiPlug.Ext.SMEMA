using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.SMEMA.Controllers.Settings.Lane
{
    [Route("lane/delete")]
    public class LaneDeleteController : SettingsApp
    {
        public Response Post(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }

            if( Core.Instance.LaneDelete(id))
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.Moved,
                    Location = Context.Referrer
                };
            }
            else
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }
        }
    }
}
