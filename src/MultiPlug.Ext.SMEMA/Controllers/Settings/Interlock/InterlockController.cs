using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SMEMA.Models.Settings.Interlock;
using MultiPlug.Ext.SMEMA.Models.Components.Interlock;
using MultiPlug.Ext.SMEMA.Components.Lane;

namespace MultiPlug.Ext.SMEMA.Controllers.Settings.Interlock
{
    [Route("lane/interlock")]
    public class InterlockController : SettingsApp
    {
        public Response Get(string id)
        {
            var LaneSearch = Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid == id);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            return new Response
            {
                Model = new InterlockModel
                {
                    Guid = LaneSearch.Guid,
                    Properties = LaneSearch.Interlock
                },
                Template = Templates.SettingsInterlock
            };
        }

        public Response Post(InterlockUpdateModel theModel)
        {
            LaneComponent LaneSearch = Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid == theModel.Guid);

            if (LaneSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }


            ushort InterlockSubscriptionSubjectIndex = 0;
            ushort.TryParse(theModel.InterlockSubscriptionSubjectIndex, out InterlockSubscriptionSubjectIndex);

            InterlockProperties Properties = new InterlockProperties
            {
                InterlockSubscription = new Models.Exchange.Subscription
                {
                    Id = theModel.InterlockSubscriptionId,
                    Subjects = new ushort[] { InterlockSubscriptionSubjectIndex },
                    Value = theModel.InterlockSubscriptionReadyValue
                }
            };

            LaneSearch.Interlock.UpdateProperties(Properties);

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = Context.Referrer
            };
        }


    }
}
