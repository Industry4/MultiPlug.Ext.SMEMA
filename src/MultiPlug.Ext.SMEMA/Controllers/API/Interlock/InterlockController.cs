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
                Open = new
                {
                    Ready = LaneSearch.Interlock.MachineReadyStateMachine.MachineReady,
                    Good = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoard,
                    Bad = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoard,
                    Flip = LaneSearch.Interlock.BoardAvailableStateMachine.FlipBoard
                },
                Latched = new
                {
                    Ready = LaneSearch.Interlock.MachineReadyStateMachine.Latch,
                    Good = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoardLatch,
                    Bad = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoardLatch,
                    Flip = LaneSearch.Interlock.BoardAvailableStateMachine.FlipBoardLatch
                },
                Diverted = new
                {
                    Good = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoardDivert,
                    Bad = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoardDivert
                },
                DivertedLatched = new
                {
                    Good = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoardDivertLatch,
                    Bad = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoardDivertLatch
                },
                Blocked = new
                {
                    Ready = LaneSearch.Interlock.MachineReadyStateMachine.Blocked,
                    Good = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoardBlocked,
                    Bad = LaneSearch.Interlock.BoardAvailableStateMachine.BadBoardBlocked,
                    Flip = LaneSearch.Interlock.BoardAvailableStateMachine.FlipBoardBlocked
                }
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
