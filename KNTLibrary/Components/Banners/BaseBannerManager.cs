using System.Collections.Generic;
using System.Linq;
using KNTLibrary.Helpers;

namespace KNTLibrary.Components.Banners
{
    public class BaseBannerManager
    {
        static BaseBannerManager()
        {
            BaseBannerManager.Instance = new BaseBannerManager();
        }
        
        public static BaseBannerManager Instance { get; private set; }
        public HashSet<BaseBannerInfo> BannerInfos { get; private set; } = new HashSet<BaseBannerInfo>();

        public void AddBanners(string directoryPath, string fileName)
        {
            HashSet<BaseBannerInfo> info = FileHelper.Load<List<BaseBannerInfo>>(directoryPath, fileName).ToHashSet();
            foreach (var banner in info)
            {
                BannerInfos.Add(banner);
            }
        }
    }
}