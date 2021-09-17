using System.Text;
using MultiPlug.Base.Http;
using MultiPlug.Base.Attribute;
using MultiPlug.Ext.SMEMA.Properties;

namespace MultiPlug.Ext.SMEMA.Controllers.Assets.JavaScripts
{
    [Route("js/*")]
    public class JavaScriptsController : AssetsController
    {
        public Response Get(string theName)
        {

            string Result = string.Empty;

            switch (theName)
            {
                case "lanes.js":
                    Result = Resources.LanesJS;
                    break;

                default:
                    break;
            }

            if (string.IsNullOrEmpty(Result))
            {
                return new Response { StatusCode = System.Net.HttpStatusCode.NotFound };
            }
            else
            {
                return new Response { MediaType = "text/javascript", RawBytes = Encoding.ASCII.GetBytes(Result) };
            }
        }
    }
}
