using System.Collections.Generic;
using System.Linq;
using KNTLibrary.Helpers;

namespace KNTLibrary.Components.Banners
{
    public class BaseBannerManager
    {
        #region Singleton

        static BaseBannerManager()
        {
            BaseBannerManager.Instance = new BaseBannerManager();
        }

        public static BaseBannerManager Instance { get; private set; }

        #endregion

        public HashSet<BaseBannerInfo> Infos { get; private set; } = new HashSet<BaseBannerInfo>();

        public void AddBanners(string directoryPath, string fileName)
        {
            var infos = FileHelper.Load<List<BaseBannerInfo>>(directoryPath, fileName).ToHashSet();
            foreach (var info in infos)
            {
                this.Infos.Add(info);
            }
        }
    }
}