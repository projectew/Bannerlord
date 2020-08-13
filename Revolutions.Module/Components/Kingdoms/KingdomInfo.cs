using System;
using Revolutions.Library.Components.Kingdoms;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Module.Components.Kingdoms
{
    [Serializable]
    public class KingdomInfo : BaseKingdomInfo
    {
        public KingdomInfo()
        {

        }

        public KingdomInfo(Kingdom kingdom) : base(kingdom)
        {

        }

        #region Reference Properties



        #endregion

        #region Virtual Objects

        #region Reference Properties



        #endregion

        #region Reference Properties Inherited



        #endregion

        #region Normal Properties



        #endregion

        #endregion

        #region Normal Properties

        public bool LuckyNation { get; set; } = false;

        public bool IsRevoltKingdom { get; set; } = false;

        public bool IsCivilWarKingdom { get; set; } = false;

        public bool HasCivilWar { get; set; } = false;

        #endregion
    }
}