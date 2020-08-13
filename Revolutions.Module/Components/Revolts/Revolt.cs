using System;
using Revolutions.Library.Components;
using Revolutions.Module.Components.Parties;
using Revolutions.Module.Components.Settlements;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Module.Components.Revolts
{
    [Serializable]
    public class Revolt : IBaseInfoType, IBaseComponent<Revolt>
    {
        #region IGameComponent<InfoType>

        public bool Equals(Revolt other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object other)
        {
            if (other is Revolt info)
            {
                return Id == info.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion

        public Revolt()
        {

        }

        public Revolt(string partyId, Settlement settlement, bool isMinorFaction)
        {
            Id = $"p_{partyId}__s_{settlement.StringId}";
            PartyId = partyId;
            SettlementId = settlement.StringId;
            IsMinorFaction = isMinorFaction;
        }

        #region Reference Properties

        public string Id { get; set; }

        public string PartyId { get; set; }

        public string SettlementId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties Objects

        public PartyBase Party => Managers.Party.GetGameObject(PartyId);

        public PartyInfo PartyInfoRevolts => Managers.Party.GetInfo(Party);

        public Settlement Settlement => Managers.Settlement.GetGameObject(SettlementId);

        public SettlementInfo SettlementInfo => Managers.Settlement.Get(Settlement);

        #endregion



        #endregion

        #region Normal Properties

        public bool IsMinorFaction { get; set; }

        #endregion
    }
}