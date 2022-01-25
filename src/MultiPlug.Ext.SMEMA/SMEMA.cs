using MultiPlug.Ext.SMEMA.Controllers;
using MultiPlug.Ext.SMEMA.Models.Load;
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
                    new RazorTemplate(Templates.SettingsNavigation,Resources.SettingsNavigation),
                    new RazorTemplate(Templates.SettingsHome, Resources.SettingsHome),
                    new RazorTemplate(Templates.SettingsLane, Resources.SettingsLane),
                    new RazorTemplate(Templates.SettingsBoardAvailable, Resources.SettingsBoardAvailable),
                    new RazorTemplate(Templates.SettingsInterlock, Resources.SettingsInterlock),
                    new RazorTemplate(Templates.SettingsMachineReady, Resources.SettingsMachineReady),
                    new RazorTemplate(Templates.SettingsAbout, Resources.SettingsAbout),
                };
            }
        }

        public override object Save()
        {
            return Core.Instance;
        }

        public void Load(LoadRoot theConfiguration)
        {
            Core.Instance.Load(theConfiguration);
        }
    }
}
