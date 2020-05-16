using System;
using TaleWorlds.CampaignSystem;

namespace KNTLibrary.Components.Parties
{
    [Serializable]
    public class BasePartyInfo : IBaseInfoType, IBaseComponent<BasePartyInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(BasePartyInfo other)
        {
            return this.Id == other.Id;
        }

        public override bool Equals(object other)
        {
            if (other is BasePartyInfo info)
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

        public BasePartyInfo()
        {

        }

        public BasePartyInfo(PartyBase party)
        {
            this.Id = party.Id;
        }

        #region Reference Properties

        public string Id { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public PartyBase Party => BaseManagers.Party.GetGameObject(this.Id);

        #endregion



        #endregion

        #region Normal Properties

        public bool IsCustomParty { get; set; } = false;

        #endregion
    }
}