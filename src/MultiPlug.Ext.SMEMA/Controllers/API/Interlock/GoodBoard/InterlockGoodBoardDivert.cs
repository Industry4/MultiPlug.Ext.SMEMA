using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Controllers.API.Utils;
using MultiPlug.Ext.SMEMA.Components.Lane;

namespace MultiPlug.Ext.SMEMA.Controllers.API.Interlock.GoodBoard
{
    [Route("1/lane/interlock/good/divert")]
    public class InterlockGoodBoardDivert : APIController
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

            LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoardDivert = enable;

            var Result = new
            {
                Good = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoard,
                Latched = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoardLatch,
                Diverted = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoardDivert,
                DivertedLatched = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoardDivertLatch
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
