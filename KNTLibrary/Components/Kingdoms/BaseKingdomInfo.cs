using System;
using TaleWorlds.CampaignSystem;

namespace KNTLibrary.Components.Kingdoms
{
    [Serializable]
    public class BaseKingdomInfo : IBaseInfoType, IBaseComponent<BaseKingdomInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(BaseKingdomInfo other)
        {
            return this.Id == other.Id;
        }

        public override bool Equals(object other)
        {
            if (other is BaseKingdomInfo info)
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

        public BaseKingdomInfo()
        {

        }

        public BaseKingdomInfo(Kingdom kingdom)
        {
            this.Id = kingdom.StringId;
        }

        #region Reference Properties

        public string Id { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public Kingdom Kingdom => BaseManagers.Kingdom.GetGameObject(this.Id);

        #endregion



        #endregion

        #region Normal Properties

        public bool IsCustomKingdom { get; set; } = false;

        #endregion
    }
}