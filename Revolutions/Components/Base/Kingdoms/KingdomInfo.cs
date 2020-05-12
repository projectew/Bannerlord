using KNTLibrary.Components.Kingdoms;
using System;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.Base.Kingdoms
{
    [Serializable]
    public class KingdomInfo : BaseKingdomInfo
    {
        public KingdomInfo() : base()
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