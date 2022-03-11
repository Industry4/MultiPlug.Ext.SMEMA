using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.SMEMA.Controllers.API.Power
{
    [Route("power/restart/")]
    public class RestartController : APIController
    {
        public Response Post()
        {
            if (Core.Instance.DoRestart())
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
