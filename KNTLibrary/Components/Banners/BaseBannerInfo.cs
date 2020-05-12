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
            this.Id = id;
            this.BannerId = bannerId;
        }

        #region Reference Properties

        public string Id { get; set; }

        public string BannerId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties



        #endregion



        #endregion

        #region Normal Properties



        #endregion
    }
}