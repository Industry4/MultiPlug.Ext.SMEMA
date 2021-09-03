using MultiPlug.Ext.SMEMA.Controllers;
using MultiPlug.Ext.SMEMA.Properties;
using MultiPlug.Extension.Core;
using MultiPlug.Extension.Core.Http;

namespace MultiPlug.Ext.SMEMA
{
    public class SMEMA : MultiPlugExtension
    {
        public override RazorTemplate[] RazorTemplates
        {
            get
            {
                return new RazorTemplate[]
                {
                    new RazorTemplate(Templates.SettingsHome, Resources.SettingsHome),
                };
            }
        }
    }
}
