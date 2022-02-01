using System;
using System.Linq;
using MultiPlug.Ext.SMEMA.Components.Lane;

namespace MultiPlug.Ext.SMEMA.Controllers.API.Utils
{
    internal static class GetLane
    {
        internal static LaneComponent ByIndex( string theIndex )
        {
            if( string.IsNullOrEmpty(theIndex))
            {
                return null;
            }

            int index = -1;

            if(int.TryParse(theIndex, out index) )
            {
                if(index > -1 && index < Core.Instance.Lanes.Length)
                {
                    return Core.Instance.Lanes[index];
                }
            }

            return null;
        }

        internal static LaneComponent ByGuid(string theGuid)
        {
            return Core.Instance.Lanes.FirstOrDefault(Lane => Lane.Guid.Equals(theGuid, StringComparison.OrdinalIgnoreCase));
        }

        internal static LaneComponent Invoke(string theIndex, string theGuid)
        {
            var Result = ByIndex(theIndex);

            if( Result == null)
            {
                Result = ByGuid(theGuid);
            }

            return Result;
        }
    }
}
