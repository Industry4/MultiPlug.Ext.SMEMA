using System.Text;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Properties;

namespace MultiPlug.Ext.SMEMA.Controllers.Assets
{
    [Route("js/*")]
    public class JavaScriptsController : AssetsController
    {
        public Response Get(string theName)
        {
            string Result = string.Empty;

            if (theName.StartsWith("smemaio.") && theName.EndsWith(".js"))
            {
                Result = Resources.smemaio_min_js;
            }
            else
            {
                switch (theName)
                {
                    case "lanes.js":
                        Result = Resources.lanes_js;
                        break;

                    case "jquery-3.4.1.min.js":
                        Result = Resources.jquery_3_4_1_min_js;
                        break;

                    case "popper.min.js":
                        Result = Resources.popper_min_js;
                        break;

                    case "bootstrap.min.js":
                        Result = Resources.bootstrap_min_js;
                        break;

                    case "mdb.min.js":
                        Result = Resources.mdb_min_js;
                        break;

                    case "all.min.js":
                        Result = Resources.all_min_js;
                        break;

                    default:
                        break;
                }
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
