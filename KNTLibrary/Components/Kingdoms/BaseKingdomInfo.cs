using System;
using TaleWorlds.CampaignSystem;

namespace KNTLibrary.Components.Kingdoms
{
    [Serializable]
    public class BaseKingdomInfo : IBaseComponent<BaseKingdomInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(BaseKingdomInfo other)
        {
            return this.KingdomId == other.KingdomId;
        }

        public override bool Equals(object other)
        {
            if (other is BaseKingdomInfo kingdomInfo)
            {
                return this.KingdomId == kingdomInfo.KingdomId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.KingdomId.GetHashCode();
        }

        #endregion

        public BaseKingdomInfo()
        {

        }

        public BaseKingdomInfo(Kingdom kingdom)
        {
            this.KingdomId = kingdom.StringId;
        }

        #region Reference Properties

        public string KingdomId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public Kingdom Kingdom => BaseManagers.Kingdom.GetGameObject(this.KingdomId);

        #endregion



        #endregion

        #region Normal Properties

        public bool IsCustomKingdom { get; set; } = false;

        #endregion
    }
}