using System;

namespace KNTLibrary.Components.Banners
{
    [Serializable]
    public class BaseBannerInfo
    {
        public BaseBannerInfo()
        {
            
        }
        
        public BaseBannerInfo(string id, string bannerId)
        {
            Id = id;
            BannerId = bannerId;
        }
        
        public string Id { get; set; }
        public string BannerId { get; set; }
    }
}