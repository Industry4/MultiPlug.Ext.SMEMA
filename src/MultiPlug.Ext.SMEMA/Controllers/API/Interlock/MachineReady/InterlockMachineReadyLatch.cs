using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Components.Lane;
using MultiPlug.Ext.SMEMA.Controllers.API.Utils;

namespace MultiPlug.Ext.SMEMA.Controllers.API.Interlock.MachineReady
{
    [Route("1/lane/interlock/ready/latch")]
    public class InterlockMachineReadyLatch : APIController
    {
        public Response Post(string index, string guid, bool enable)
        {
            LaneComponent LaneSearch = GetLane.Invoke(index, guid);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            LaneSearch.Interlock.MachineReadyStateMachine.Latch = enable;

            var Result = new
            {
                Ready = LaneSearch.Interlock.MachineReadyStateMachine.MachineReady,
                Latched = LaneSearch.Interlock.MachineReadyStateMachine.Latch
            };

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Model = Result,
                MediaType = "application/json"
            };
        }
    }
}
