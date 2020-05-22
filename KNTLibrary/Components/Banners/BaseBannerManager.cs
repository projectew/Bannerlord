using KNTLibrary.Components.Settlements;
using KNTLibrary.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace KNTLibrary.Components.Banners
{
    public class BaseBannerManager
    {
        #region Singleton

        static BaseBannerManager()
        {
            Instance = new BaseBannerManager();
        }

        public static BaseBannerManager Instance { get; private set; }

        #endregion

        public HashSet<BaseBannerInfo> Infos { get; set; } = new HashSet<BaseBannerInfo>();


        public void InitializeInfos()
        {
            var directoryPath = BasePath.Name + "Modules/Revolutions/ModuleData/Banners";

            foreach (var file in Directory.GetFiles(directoryPath))
            {
                var loadedInfos = FileHelper.Load<List<BaseBannerInfo>>(directoryPath, Path.GetFileNameWithoutExtension(file));
                foreach (var info in loadedInfos)
                {
                    this.Infos.Add(info);
                }
            }
        }

        public void CleanupDuplicatedInfos()
        {
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
            var settlementInfo = BaseManagers.Settlement.Get(settlement);
            return settlementInfo == null ? null : this.GetBaseBannerBySettlementInfo(settlementInfo);
        }
    }
}