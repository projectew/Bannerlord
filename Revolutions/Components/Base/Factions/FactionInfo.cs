using System;
using TaleWorlds.CampaignSystem;
using KNTLibrary.Components.Factions;
using Revolutions.Components.Base.Settlements;

namespace Revolutions.Components.Base.Factions
{
    [Serializable]
    public class FactionInfo : BaseFactionInfo
    {
        public FactionInfo() : base()
        {

        }

        public FactionInfo(IFaction faction) : base(faction)
        {

        }

        #region Reference Properties

        public string RevoltedSettlementId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public Settlement RevoltedSettlement => RevolutionsManagers.Settlement.GetGameObject(this.RevoltedSettlementId);

        public SettlementInfo RevoltedSettlementInfo => RevolutionsManagers.Settlement.GetInfo(this.RevoltedSettlement);

        #endregion

        #region Reference Properties Inherited



        #endregion



        #endregion

        #region Normal Properties

        public bool CanRevolt { get; set; } = false;

        public int DaysSinceLastRevolt { get; set; } = 0;

        public bool SuccessfullyRevolted { get; set; } = false;

        #endregion
    }
}