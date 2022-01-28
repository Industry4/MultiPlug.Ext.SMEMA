using System.IO;
using System.Drawing;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Properties;

namespace MultiPlug.Ext.SMEMA.Controllers.Assets
{
    [Route("ico/*")]
    public class IconController : AssetsController
    {
        public Response Get(string theName)
        {
            Response Result;
            ImageConverter converter = new ImageConverter();

            switch (theName)
            {
                case "favicon.ico":
                    Result = new Response { RawBytes = IconToBytes(Resources.favicon), MediaType = "image/x-icon" };
                    break;

                default:
                    Result = new Response { StatusCode = System.Net.HttpStatusCode.NotFound };
                    break;
            }

            return Result;
        }

        private static byte[] IconToBytes(Icon icon)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                icon.Save(ms);
                return ms.ToArray();
            }
        }
    }
}
