using KNTLibrary.Components.Banners;
using Revolutions.Components.Settlements;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.Components.Banners
{
    internal class BannerManager : BaseBannerManager
    {
        #region Singleton

        static BannerManager()
        {
            Instance = new BannerManager();
        }

        public static new BannerManager Instance { get; private set; }

        #endregion

        public BaseBannerInfo GetRevolutionsBannerBySettlementInfo(SettlementInfo settlementInfo)
        {
            var availableBannerInfos = new List<BaseBannerInfo>();
            BaseBannerInfo bannerInfo = null;

            foreach (var info in this.Infos)
            {
                if (info.Used)
                {
                    continue;
                }

                if (info.Settlement == settlementInfo.Settlement.Name.ToString() && info.Culture == settlementInfo.Settlement.Culture.StringId)
                {
                    availableBannerInfos.Add(info);
                    break;
                }

                if (info.Faction == settlementInfo.LoyalFaction.StringId)
                {
                    availableBannerInfos.Add(info);
                    break;
                }

                if (info.Culture == settlementInfo.Settlement.Culture.StringId)
                {
                    availableBannerInfos.Add(info);
                    break;
                }
            }

            if (availableBannerInfos.Count() > 0)
            {
                bannerInfo = availableBannerInfos.GetRandomElement();
            }

            if (bannerInfo != null)
            {
                bannerInfo.Used = true;
            }

            return bannerInfo;
        }

        public BaseBannerInfo GetRevolutionsBannerBySettlement(Settlement settlement)
        {
            var settlementInfo = Managers.Settlement.Get(settlement);
            if (settlementInfo == null)
            {
                return null;
            }

            return this.GetRevolutionsBannerBySettlementInfo(settlementInfo);
        }
    }
}