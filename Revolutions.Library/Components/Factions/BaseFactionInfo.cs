using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Library.Components.Factions
{
    [Serializable]
    public class BaseFactionInfo : IBaseInfoType, IBaseComponent<BaseFactionInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(BaseFactionInfo other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object other)
        {
            if (other is BaseFactionInfo info)
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

        public BaseFactionInfo()
        {

        }

        public BaseFactionInfo(IFaction faction)
        {
            Id = faction.StringId;
            InitialTownsCount = CurrentTownsCount;
        }


        #region Reference Properties

        public string Id { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public IFaction Faction => BaseManagers.Faction.GetGameObject(Id);

        #endregion

        public int CurrentTownsCount => Faction.Settlements.Count(settlement => settlement.IsTown);

        #endregion

        #region Normal Properties

        public int InitialTownsCount { get; set; }

        #endregion
    }
}