using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Components.Lane;
using MultiPlug.Ext.SMEMA.Controllers.API.Utils;

namespace MultiPlug.Ext.SMEMA.Controllers.API.Interlock.BadBoard
{
    [Route("1/lane/interlock/bad/latch")]
    public class InterlockBadBoardLatch : APIController
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

            LaneSearch.Interlock.BoardAvailableStateMachine.BadBoardLatch = enable;

            var Result = new
            {
                Bad = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoard,
                Latched = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoardLatch,
                Diverted = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoardDivert,
                DivertedLatched = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoardDivertLatch
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
