using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SMEMA.Controllers;
using MultiPlug.Ext.SMEMA.Models.Load;
using MultiPlug.Ext.SMEMA.Properties;
using MultiPlug.Extension.Core;
using MultiPlug.Extension.Core.Http;

namespace MultiPlug.Ext.SMEMA
{
    public class SMEMA : MultiPlugExtension
    {
        public SMEMA()
        {
            Core.Instance.SubscriptionsUpdated += Instance_SubscriptionsUpdated;
            Core.Instance.EventsUpdated += Instance_EventsUpdated;
        }

        private void Instance_EventsUpdated()
        {
            MultiPlugActions.Extension.Updates.Events();
        }

        private void Instance_SubscriptionsUpdated()
        {
            MultiPlugActions.Extension.Updates.Subscriptions();
        }

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

                    new RazorTemplate(Templates.AppsSMEMAMonitorShutdownModal, Resources.AppsSMEMAMonitorShutdownModal),
                    new RazorTemplate(Templates.AppsSMEMAMonitorWebSocketReconnectModal, Resources.AppsSMEMAMonitorWebSocketReconnectModal),
                    new RazorTemplate(Templates.AppsSMEMAMonitorSelectLaneModal, Resources.AppsSMEMAMonitorSelectLaneModal),
                    new RazorTemplate(Templates.AppsSMEMAMonitorNavBar, Resources.AppsSMEMAMonitorNavBar),
                    new RazorTemplate(Templates.AppsSMEMAMonitorNavBarSimple, Resources.AppsSMEMAMonitorNavBarSimple),
                    new RazorTemplate(Templates.AppsSMEMAMonitorSMEMAIOContainer, Resources.AppsSMEMAMonitorSMEMAIOContainer),
                    new RazorTemplate(Templates.AppsSMEMAMonitorNotSetup, Resources.AppsSMEMAMonitorNotSetup),
                    new RazorTemplate(Templates.AppsSMEMAMonitorLaneNotFound, Resources.AppsSMEMAMonitorLaneNotFound),
                    new RazorTemplate(Templates.AppsSMEMAMonitorHome, Resources.AppsSMEMAMonitorHome),
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

        public override Event[] Events
        {
            get
            {
                return Core.Instance.Events;
            }
        }

        public override Subscription[] Subscriptions
        {
            get
            {
                return Core.Instance.Subscriptions;
            }
        }
    }
}
