using System.Text;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Properties;

namespace MultiPlug.Ext.SMEMA.Controllers.Assets
{
    [Route("css/*")]
    public class CSSController : AssetsController
    {
        public Response Get(string theName)
        {
            string Result = string.Empty;

            if (theName.StartsWith("style.min.") && theName.EndsWith(".css"))
            {
                Result = Resources.style_min_css;
            }
            else
            {
                switch (theName)
                {
                    case "all.min.css":
                        Result = Resources.all_min_css;
                        break;

                    case "bootstrap.min.css":
                        Result = Resources.bootstrap_min_css;
                        break;

                    case "mdb.min.css":
                        Result = Resources.mdb_min_css;
                        break;

                    default:
                        //m_Logger.WriteLine("ERROR Style missing:               " + theName);
                        break;
                }
            }

            if (string.IsNullOrEmpty(Result))
            {
                return new Response { StatusCode = System.Net.HttpStatusCode.NotFound };
            }
            else
            {
                return new Response { MediaType = "text/css", RawBytes = Encoding.ASCII.GetBytes(Result) };
            }
        }
    }
}
