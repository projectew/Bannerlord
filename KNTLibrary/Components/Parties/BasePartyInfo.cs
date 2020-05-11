using System;
using TaleWorlds.CampaignSystem;

namespace KNTLibrary.Components.Parties
{
    [Serializable]
    public class BasePartyInfo : IBaseComponent<BasePartyInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(BasePartyInfo other)
        {
            return this.PartyId == other.PartyId;
        }

        public override bool Equals(object other)
        {
            if (other is BasePartyInfo partyInfo)
            {
                return this.PartyId == partyInfo.PartyId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.PartyId.GetHashCode();
        }

        #endregion

        public BasePartyInfo()
        {

        }

        public BasePartyInfo(PartyBase party)
        {
            this.PartyId = party.Id;
        }

        #region Reference Properties

        public string PartyId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public PartyBase Party => BaseManagers.Party.GetGameObject(this.PartyId);

        #endregion



        #endregion

        #region Normal Properties

        public bool IsCustomParty { get; set; } = false;

        #endregion
    }
}