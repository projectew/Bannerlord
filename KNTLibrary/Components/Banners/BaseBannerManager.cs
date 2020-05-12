using KNTLibrary.Components.Settlements;
using KNTLibrary.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.CampaignSystem;

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

        private HashSet<BaseBannerInfo> LoadedInfos { get; } = new HashSet<BaseBannerInfo>();


        public void AddBanners(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath);

            foreach (var file in files)
            {
                var infos = FileHelper.Load<List<BaseBannerInfo>>(directoryPath, Path.GetFileNameWithoutExtension(file)).ToHashSet();
                foreach (var info in infos)
                {
                    this.LoadedInfos.Add(info);
                }
            }
        }

        public void CleanupDuplicatedInfos()
        {
            foreach (var info in this.LoadedInfos)
            {
                this.Infos.Add(info);
            }

            this.Infos.Reverse();
            this.Infos = this.Infos.GroupBy(i => i.Id)
                .Select(i => i.First())
                .ToHashSet();
            this.Infos.Reverse();
        }

        public BaseBannerInfo GetBaseBannerBySettlementInfo(BaseSettlementInfo settlementInfo)
        {
            BaseBannerInfo bannerInfo = null;

            foreach (var info in this.Infos)
            {
                if (info.Used)
                {
                    continue;
                }

                if (info.Settlement == settlementInfo.Settlement.Name.ToString() && info.Culture == settlementInfo.Settlement.Culture.StringId)
                {
                    bannerInfo = info;
                    break;
                }
                else if (info.Culture == settlementInfo.Settlement.Culture.StringId)
                {
                    bannerInfo = info;
                    break;
                }
                else if (info.Faction == settlementInfo.CurrentFaction.StringId)
                {
                    bannerInfo = info;
                    break;
                }
            }

            return bannerInfo;
        }

        public BaseBannerInfo GetBaseBannerBySettlement(Settlement settlement)
        {
            var settlementInfo = BaseManagers.Settlement.GetInfo(settlement);
            if (settlementInfo == null)
            {
                return null;
            }

            return this.GetBaseBannerBySettlementInfo(settlementInfo);
        }
    }
}