using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Models.Settings.Home;

namespace MultiPlug.Ext.SMEMA.Controllers.Settings.Home
{
    public class HomeController : SettingsApp
    {
        [Route("")]
        public Response Get()
        {
            return new Response
            {
                Model = new HomeModel
                {
                    Lanes = Core.Instance.Lanes.Select(Lane => new LaneModel { Guid = Lane.Guid, LaneId = Lane.LaneId, MachineId = Lane.MachineId } ).ToArray()
                },
                Template = Templates.SettingsHome
            };
        }


        public Response Post(NewLanesModel theModel)
        {
            if( theModel != null 
                && theModel.LaneId != null 
                && theModel.MachineName != null
                && theModel.LaneId.Length == theModel.MachineName.Length)
            {

                for (int Index = 0; Index < theModel.LaneId.Length; Index++)
                {
                    Core.Instance.LaneAdd(theModel.LaneId[Index], theModel.MachineName[Index]);
                }
            }

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = Context.Referrer
            };
        }
    }
}
