using System.Drawing;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Properties;

namespace MultiPlug.Ext.SMEMA.Controllers.Assets.Images
{
    [Route("images/*")]
    public class ImagesController : AssetsController
    {
        public Response Get(string theName)
        {
            if (string.Equals(theName, "SMEMA-small.png", System.StringComparison.OrdinalIgnoreCase))
            {
                ImageConverter converter = new ImageConverter();
                return new Response { RawBytes = (byte[])converter.ConvertTo(Resources.SMEMA_small_png, typeof(byte[])), MediaType = "image/png" };
            }
            else
            {
                return new Response { StatusCode = System.Net.HttpStatusCode.NotFound };
            }
        }
    }
}
