using MultiPlug.Base.Http;
using MultiPlug.Extension.Core.Attribute;

namespace MultiPlug.Ext.SMEMA.Controllers.Apps.SMEMAMonitor
{
    [HttpEndpointType(HttpEndpointType.App)]
    [ViewAs(ViewAs.FullScreen)]
    [Name("IPC SMEMA 9851 Monitor")]
    public class SMEMAMonitorApp : Controller
    {
    }
}
