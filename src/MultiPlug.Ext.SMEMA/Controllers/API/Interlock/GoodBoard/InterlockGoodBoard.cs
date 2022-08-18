using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Components.Lane;
using MultiPlug.Ext.SMEMA.Controllers.API.Utils;

namespace MultiPlug.Ext.SMEMA.Controllers.API.Interlock.GoodBoard
{
    [Route("1/lane/interlock/good")]
    public class InterlockGoodBoard : APIController
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

            if (LaneSearch.Interlock.PermissionInterfaceREST == true)
            {
                LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoard = enable;
            }

            var Result = new
            {
                Good = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoard,
                Latched = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoardLatch,
                Diverted = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoardDivert,
                DivertedLatched = LaneSearch.Interlock.BoardAvailableStateMachine.GoodBoardDivertLatch
            };

            return new Response
            {
                StatusCode = LaneSearch.Interlock.PermissionInterfaceREST == true ? System.Net.HttpStatusCode.OK : System.Net.HttpStatusCode.Forbidden,
                Model = Result,
                MediaType = "application/json"
            };
        }
    }
}
