
namespace MultiPlug.Ext.SMEMA.Models.Settings.About
{
    public class AboutModel
    {
        public string Guid { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Copyright { get; set; }
        public string Description { get; set; }
        public string Product { get; set; }
        public string Trademark { get; set; }
        public string Version { get; internal set; }
    }
}
