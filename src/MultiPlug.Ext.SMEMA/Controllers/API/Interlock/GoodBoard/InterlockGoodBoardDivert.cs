using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Controllers.API.Utils;
using MultiPlug.Ext.SMEMA.Components.Lane;

namespace MultiPlug.Ext.SMEMA.Controllers.API.Interlock.GoodBoard
{
    [Route("1/lane/interlock/goodboard/divert")]
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

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.OK,
            };
        }
    }
}
