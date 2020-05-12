using System.Collections.Generic;
using System.IO;
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

        public HashSet<BaseBannerInfo> Infos { get; set; } = new HashSet<BaseBannerInfo>();
        
        private HashSet<BaseBannerInfo> loadedInfos { get; } = new HashSet<BaseBannerInfo>();


        public void AddBanners(string directoryPath)
        {
            string[] files = Directory.GetFiles(directoryPath);

            foreach (var file in files)
            {
                HashSet<BaseBannerInfo> info = FileHelper.Load<List<BaseBannerInfo>>(directoryPath, Path.GetFileNameWithoutExtension(file)).ToHashSet();
                foreach (var banner in info)
                {
                    loadedInfos.Add(banner);
                }
            }
        }
        
        public void CleanupDuplicatedInfos()
        {
            foreach (var loadedInfo in loadedInfos)
            {
                Infos.Add(loadedInfo);
            }
            this.Infos.Reverse();
            this.Infos = this.Infos.GroupBy(i => i.Id)
                .Select(i => i.First())
                .ToHashSet();
            this.Infos.Reverse();
        }
    }
}