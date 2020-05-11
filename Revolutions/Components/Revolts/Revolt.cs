using System;
using TaleWorlds.CampaignSystem;
using KNTLibrary.Components;
using Revolutions.Components.Base.Settlements;
using Revolutions.Components.Base.Parties;
using Revolts;

namespace Revolutions.Components.Revolts
{
    [Serializable]
    public class Revolt : IBaseComponent<Revolt>
    {
        #region IGameComponent<InfoType>

        public bool Equals(Revolt other)
        {
            return this.PartyId == other.PartyId && this.SettlementId == other.SettlementId;
        }

        public override bool Equals(object other)
        {
            if (other is Revolt Revolt)
            {
                return this.PartyId == Revolt.PartyId && this.SettlementId == Revolt.SettlementId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (this.PartyId, this.SettlementId).GetHashCode();
        }

        #endregion

        public Revolt()
        {

        }

        public Revolt(string partyId, Settlement settlement, bool isMinorFaction)
        {
            this.PartyId = partyId;
            this.SettlementId = settlement.StringId;
            this.IsMinorFaction = isMinorFaction;
        }

        #region Reference Properties

        public string PartyId { get; set; }

        public string SettlementId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties Objects

        public PartyBase Party => RevoltsManagers.Party.GetGameObject(this.PartyId);

        public PartyInfo PartyInfoRevolts => RevoltsManagers.Party.GetInfo(this.Party);

        public Settlement Settlement => RevoltsManagers.Settlement.GetGameObject(this.SettlementId);

        public SettlementInfo SettlementInfo => RevoltsManagers.Settlement.GetInfo(this.Settlement);

        #endregion



        #endregion

        #region Normal Properties

        public bool IsMinorFaction { get; set; } = false;

        #endregion
    }
}