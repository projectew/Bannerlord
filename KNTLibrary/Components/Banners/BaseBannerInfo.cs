using System;
using System.Security.Permissions;

namespace KNTLibrary.Components.Banners
{
    [Serializable]
    public class BaseBannerInfo
    {
        public BaseBannerInfo()
        {

        }

        #region Reference Properties

        public string Id { get; set; } = string.Empty;
		
        public string ModName { get; set; } = string.Empty;
		
        public string Culture { get; set; } = string.Empty;
		
        public string Settlement { get; set; } = string.Empty;
		
        public string Faction { get; set; } = string.Empty;
		
        public string BannerId { get; set; } = string.Empty;
		
        public string Kingdom { get; set; } = string.Empty;
		
        public string Character { get; set; } = string.Empty;
		
        public string Credit { get; set; } = string.Empty;
		
        public bool Used { get; set; } = false;

        #endregion

        #region Virtual Objects

        #region Reference Properties



        #endregion



        #endregion

        #region Normal Properties



        #endregion
    }
}