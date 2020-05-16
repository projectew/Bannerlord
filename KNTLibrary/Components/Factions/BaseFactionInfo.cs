using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace KNTLibrary.Components.Factions
{
    [Serializable]
    public class BaseFactionInfo : IBaseInfoType, IBaseComponent<BaseFactionInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(BaseFactionInfo other)
        {
            return this.Id == other.Id;
        }

        public override bool Equals(object other)
        {
            if (other is BaseFactionInfo info)
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

        public BaseFactionInfo()
        {

        }

        public BaseFactionInfo(IFaction faction)
        {
            this.Id = faction.StringId;
            this.InitialTownsCount = this.CurrentTownsCount;
        }


        #region Reference Properties

        public string Id { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public IFaction Faction => BaseManagers.Faction.GetGameObject(this.Id);

        #endregion

        public int CurrentTownsCount => this.Faction.Settlements.Where(settlement => settlement.IsTown).Count();

        #endregion

        #region Normal Properties

        public int InitialTownsCount { get; set; }

        #endregion
    }
}