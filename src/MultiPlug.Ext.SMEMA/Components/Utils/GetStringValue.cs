
namespace MultiPlug.Ext.SMEMA.Components.Utils
{
    internal static class GetStringValue
    {
        internal const string Disabled = "0";
        internal const string Enabled = "1";

        internal static string Invoke(bool thevalue)
        {
            return thevalue ? Enabled : Disabled;
        }
    }
}
