using System;
using TaleWorlds.CampaignSystem;
using KNTLibrary.Components.Factions;
using Revolutions.Components.BaseComponents.Settlements;
using Revolts;

namespace Revolutions.Components.BaseComponents.Factions
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

        public Settlement RevoltedSettlement => RevoltsManagers.Settlement.GetGameObject(this.RevoltedSettlementId);

        public SettlementInfo RevoltedSettlementInfo => RevoltsManagers.Settlement.GetInfo(this.RevoltedSettlement);

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