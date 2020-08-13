using System;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Library.Components.Kingdoms
{
    [Serializable]
    public class BaseKingdomInfo : IBaseInfoType, IBaseComponent<BaseKingdomInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(BaseKingdomInfo other)
        {
            return Id == other?.Id;
        }

        public override bool Equals(object other)
        {
            if (other is BaseKingdomInfo info)
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

        public BaseKingdomInfo()
        {

        }

        public BaseKingdomInfo(Kingdom kingdom)
        {
            Id = kingdom.StringId;
        }

        #region Reference Properties

        public string Id { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public Kingdom Kingdom => BaseManagers.Kingdom.GetGameObject(Id);

        #endregion



        #endregion

        #region Normal Properties

        public bool IsCustomKingdom { get; set; } = false;

        #endregion
    }
}