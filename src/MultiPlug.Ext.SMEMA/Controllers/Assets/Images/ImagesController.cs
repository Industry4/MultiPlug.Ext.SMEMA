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
            Response Result;
            ImageConverter converter = new ImageConverter();

            switch (theName.ToLower())
            {
                case "smema-small.png":
                    Result = new Response { RawBytes = (byte[])converter.ConvertTo(Resources.SMEMA_small_png, typeof(byte[])), MediaType = "image/png" };
                    break;

                case "smema-logo.jpg":
                    Result = new Response { RawBytes = (byte[])converter.ConvertTo(Resources.SMEMA_logo_jpg, typeof(byte[])), MediaType = "image/jpeg" };
                    break;

                case "apple-touch-icon.png":
                    Result = new Response { RawBytes = (byte[])converter.ConvertTo(Resources.apple_touch_icon_png, typeof(byte[])), MediaType = "image/png" };
                    break;

                default:
                    Result = new Response { StatusCode = System.Net.HttpStatusCode.NotFound };
                    break;
            }

            return Result;
        }
    }
}
