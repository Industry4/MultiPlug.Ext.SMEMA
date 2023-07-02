using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Components.Lane;
using MultiPlug.Ext.SMEMA.Controllers.API.Utils;

namespace MultiPlug.Ext.SMEMA.Controllers.API.SMEMA
{
    [Route("1/lane/smema")]
    public class SMEMAController : APIController
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
                Upline = new
                {
                    Ready = LaneSearch.BoardAvailable.StateMachine.MachineReadyState,
                    Good = LaneSearch.BoardAvailable.StateMachine.GoodBoardAvailableState,
                    Bad = LaneSearch.BoardAvailable.StateMachine.BadBoardAvailableState,
                    Flip = LaneSearch.BoardAvailable.StateMachine.FlipBoardState
                },
                Downline = new
                {
                    Ready = LaneSearch.MachineReady.StateMachine.MachineReadyState,
                    Good = LaneSearch.MachineReady.StateMachine.GoodBoardAvailableState,
                    Bad = LaneSearch.MachineReady.StateMachine.BadBoardAvailableState,
                    Flip = LaneSearch.MachineReady.StateMachine.FlipBoardState
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
