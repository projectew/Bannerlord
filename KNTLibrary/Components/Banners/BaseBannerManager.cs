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

        private HashSet<BaseBannerInfo> _loadedInfos { get; } = new HashSet<BaseBannerInfo>();


        public void AddBanners(string directoryPath)
        {
            string[] files = Directory.GetFiles(directoryPath);

            foreach (var file in files)
            {
                var infos = FileHelper.Load<List<BaseBannerInfo>>(directoryPath, Path.GetFileNameWithoutExtension(file)).ToHashSet();
                foreach (var info in infos)
                {
                    this._loadedInfos.Add(info);
                }
            }
        }

        public void CleanupDuplicatedInfos()
        {
            foreach (var info in this._loadedInfos)
            {
                this.Infos.Add(info);
            }

            this.Infos.Reverse();
            this.Infos = this.Infos.GroupBy(i => i.Id)
                .Select(i => i.First())
                .ToHashSet();
            this.Infos.Reverse();
        }
    }
}