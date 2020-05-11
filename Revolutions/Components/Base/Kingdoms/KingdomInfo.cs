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



        #endregion

        #region Normal Properties

        public bool LuckyNation { get; set; } = false;

        #endregion
    }
}