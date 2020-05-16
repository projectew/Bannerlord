using System;
using TaleWorlds.CampaignSystem;

namespace KNTLibrary.Components.Clans
{
    [Serializable]
    public class BaseClanInfo : IBaseInfoType, IBaseComponent<BaseClanInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(BaseClanInfo other)
        {
            return this.Id == other.Id;
        }

        public override bool Equals(object other)
        {
            if (other is BaseClanInfo info)
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

        public BaseClanInfo()
        {

        }

        public BaseClanInfo(Clan clan)
        {
            this.Id = clan.StringId;
        }

        #region Reference Properties

        public string Id { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties Objects

        public Clan Clan => BaseManagers.Clan.GetGameObject(this.Id);

        #endregion



        #endregion

        #region Normal Properties

        public bool IsCustomClan { get; set; } = false;

        #endregion
    }
}