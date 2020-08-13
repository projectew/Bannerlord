using System;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Library.Components.Clans
{
    [Serializable]
    public class BaseClanInfo : IBaseInfoType, IBaseComponent<BaseClanInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(BaseClanInfo other)
        {
            return Id == other?.Id;
        }

        public override bool Equals(object other)
        {
            if (other is BaseClanInfo info)
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

        public BaseClanInfo()
        {

        }

        public BaseClanInfo(Clan clan)
        {
            Id = clan.StringId;
        }

        #region Reference Properties

        public string Id { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties Objects

        public Clan Clan => BaseManagers.Clan.GetGameObject(Id);

        #endregion



        #endregion

        #region Normal Properties

        public bool IsCustomClan { get; set; } = false;

        #endregion
    }
}