using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Components.Lane;
using MultiPlug.Ext.SMEMA.Controllers.API.Utils;

namespace MultiPlug.Ext.SMEMA.Controllers.API.Interlock
{
    [Route("1/lane/interlock")]
    public class InterlockController : APIController
    {
        public Response Get(string index, string guid, string lane)
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
                GoodBoard = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoard,
                BadBoard = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoard
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
