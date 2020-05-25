using KNTLibrary.Components;
using Revolutions.Components.Parties;
using Revolutions.Components.Settlements;
using System;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.Revolts
{
    [Serializable]
    public class Revolt : IBaseInfoType, IBaseComponent<Revolt>
    {
        #region IGameComponent<InfoType>

        public bool Equals(Revolt other)
        {
            return this.Id == other.Id;
        }

        public override bool Equals(object other)
        {
            if (other is Revolt info)
            {
                return this.Id == info.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        #endregion

        public Revolt()
        {

        }

        public Revolt(string partyId, Settlement settlement, bool isMinorFaction)
        {
            this.Id = $"p_{partyId}__s_{settlement.StringId}";
            this.PartyId = partyId;
            this.SettlementId = settlement.StringId;
            this.IsMinorFaction = isMinorFaction;
        }

        #region Reference Properties

        public string Id { get; set; }

        public string PartyId { get; set; }

        public string SettlementId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties Objects

        public PartyBase Party => Managers.Party.GetGameObject(this.PartyId);

        public PartyInfo PartyInfoRevolts => Managers.Party.GetInfo(this.Party);

        public Settlement Settlement => Managers.Settlement.GetGameObject(this.SettlementId);

        public SettlementInfo SettlementInfo => Managers.Settlement.Get(this.Settlement);

        #endregion



        #endregion

        #region Normal Properties

        public bool IsMinorFaction { get; set; } = false;

        #endregion
    }
}