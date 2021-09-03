using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.SMEMA.Controllers.Settings.Home
{
    public class HomeController : SettingsApp
    {
        [Route("")]
        public Response Get()
        {
            return new Response
            {
                Model = new Models.Settings.Home(),
                Template = Templates.SettingsHome
            };
        }
    }
}
