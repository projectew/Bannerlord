using System;
using TaleWorlds.CampaignSystem;

namespace KNTLibrary.Components.Clans
{
     [Serializable]
    public class BaseClanInfo : IBaseComponent<BaseClanInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(BaseClanInfo other)
        {
            return this.ClanId == other.ClanId;
        }

        public override bool Equals(object other)
        {
            if (other is BaseClanInfo clanInfo)
            {
                return this.ClanId == clanInfo.ClanId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.ClanId.GetHashCode();
        }

        #endregion

        public BaseClanInfo()
        {

        }

        public BaseClanInfo(Clan clan)
        {
            this.ClanId = clan.StringId;
        }

        #region Reference Properties

        public string ClanId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties Objects

        public Clan Clan => BaseManagers.Clan.GetGameObject(this.ClanId);

        #endregion



        #endregion

        #region Normal Properties

        public bool IsCustomClan { get; set; } = false;

        public bool CanJoinOtherKingdoms { get; set; } = true;

        #endregion
    }
}