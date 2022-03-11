using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.SMEMA.Controllers.API.Power
{
    [Route("power/shutdown/")]
    public class ShutdownController : APIController
    {
        public Response Post()
        {
            if (Core.Instance.DoShutdown())
            {
                var Result = new
                {
                };

                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Model = Result,
                    MediaType = "application/json"
                };
            }
            else
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.Unauthorized,
                };
            }
        }
    }
}
