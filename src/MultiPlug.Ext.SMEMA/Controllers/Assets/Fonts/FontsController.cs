using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Properties;

namespace MultiPlug.Ext.SMEMA.Controllers.Assets
{
    [Route("fonts/*")]
    public class FontsController : AssetsController
    {
        public Response Get(string theName)
        {
            byte[] ResponseBytes = new byte[0];

            switch (theName.ToLower())
            {
                case "roboto-medium.woff2":
                    ResponseBytes = Resources.Roboto_Medium_woff2;
                    break;

                case "fa-solid-900.woff2":
                    ResponseBytes = Resources.fa_solid_900_woff2;
                    break;

                case "roboto-light.woff2":
                    ResponseBytes = Resources.Roboto_Light_woff2;
                    break;

                case "roboto-regular.woff":
                    ResponseBytes = Resources.Roboto_Regular_woff;
                    break;

                case "roboto-regular.woff2":
                    ResponseBytes = Resources.Roboto_Regular_woff2;
                    break;

                case "roboto-regular.ttf":
                    ResponseBytes = Resources.Roboto_Regular_ttf;
                    break;

                case "roboto-bold.woff":
                    ResponseBytes = Resources.Roboto_Bold_woff;
                    break;

                case "roboto-bold.woff2":
                    ResponseBytes = Resources.Roboto_Bold_woff2;
                    break;

                case "roboto-bold.ttf":
                    ResponseBytes = Resources.Roboto_Bold_ttf;
                    break;

                default:
                    break;
            }

            if (ResponseBytes.Length == 0)
            {
                return new Response { StatusCode = System.Net.HttpStatusCode.NotFound };
            }
            else
            {
                string MediaType = string.Empty;

                if( theName.EndsWith(".eot"))
                {
                    MediaType = "application/vnd.ms-fontobject";
                }
                else if(theName.EndsWith(".ttf"))
                {
                    MediaType = "application/x-font-opentype";
                }
                else if (theName.EndsWith(".woff"))
                {
                    MediaType = "font/woff";
                }
                else if (theName.EndsWith(".woff2"))
                {
                    MediaType = "font/woff2";
                }

                return new Response { RawBytes = ResponseBytes, MediaType = MediaType };
            }
        }
    }
}
