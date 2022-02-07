using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Components.Lane;
using MultiPlug.Ext.SMEMA.Controllers.API.Utils;

namespace MultiPlug.Ext.SMEMA.Controllers.API.Interlock.MachineReady
{
    [Route("1/lane/interlock/ready")]
    public class InterlockMachineReady : APIController
    {
        public Response Get(string index, string guid)
        {
            LaneComponent LaneSearch = GetLane.Invoke(index, guid);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            var Result = new
            {
                MachineReady = LaneSearch.Interlock.MachineReadyStateMachine.MachineReady,
                Latch = LaneSearch.Interlock.MachineReadyStateMachine.Latch
            };

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Model = Result,
                MediaType = "application/json"
            };
        }

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

            LaneSearch.Interlock.MachineReadyStateMachine.MachineReady = enable;

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.OK,
            };
        }
    }
}
