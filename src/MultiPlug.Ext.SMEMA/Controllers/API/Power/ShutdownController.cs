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
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
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
