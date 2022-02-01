using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;

namespace MultiPlug.Ext.SMEMA.Controllers.API
{
    [Route("1/lanes")]
    public class LanesController : APIController
    {
        public Response Get()
        {
            int Index = -1;

            var Lanes = Core.Instance.Lanes.Select( Lane =>
            {
                Index++;

                return new
                {
                    Index = Index,
                    Guid = Lane.Guid,
                    Lane = Lane.LaneId,
                    Machine = Lane.MachineId
                };
            });

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Model = Lanes,
                MediaType = "application/json"
            };
        }
    }
}
