using System.Collections.Generic;
using System.Linq;
using Revolutions.Library.Components.Banners;
using Revolutions.Module.Components.Settlements;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.Module.Components.Banners
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

        public BaseBannerInfo GetBanner(SettlementInfo settlementInfo)
        {
            var availableBannerInfos = new List<BaseBannerInfo>();
            BaseBannerInfo bannerInfo = null;

            foreach (var info in Infos.Where(i => !i.Used))
            {
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

            if (availableBannerInfos.Count > 0)
            {
                bannerInfo = availableBannerInfos.GetRandomElement();
            }

            if (bannerInfo == null)
            {
                bannerInfo = GetBaseBanner(settlementInfo);
            }

            if (bannerInfo != null)
            {
                bannerInfo.Used = true;
            }

            return bannerInfo;
        }

        public BaseBannerInfo GetBanner(Settlement settlement)
        {
            var settlementInfo = Managers.Settlement.Get(settlement);
            return settlementInfo == null ? null : GetBanner(settlementInfo);
        }
    }
}