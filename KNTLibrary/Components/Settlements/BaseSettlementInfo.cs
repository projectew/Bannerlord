using KNTLibrary.Components.Factions;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace KNTLibrary.Components.Settlements
{
    [Serializable]
    public class BaseSettlementInfo : IBaseInfoType, IBaseComponent<BaseSettlementInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(BaseSettlementInfo other)
        {
            return this.Id == other.Id;
        }

        public override bool Equals(object other)
        {
            if (other is BaseSettlementInfo info)
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

        public BaseSettlementInfo()
        {

        }

        public BaseSettlementInfo(Settlement settlement)
        {
            this.Id = settlement.StringId;
            this.InitialCultureId = settlement.Culture.StringId;
            this.InitialFactionId = settlement.MapFaction.StringId;
            this.CurrentFactionId = settlement.MapFaction.StringId;
            this.PreviousFactionId = settlement.MapFaction.StringId;
        }

        #region Reference Properties

        public string Id { get; set; }

        public string InitialCultureId { get; set; }

        public string InitialFactionId { get; set; }

        public string CurrentFactionId { get; set; }

        public string PreviousFactionId { get; set; }

        #endregion

        #region Virtual Objects

        #region Reference Properties

        public Settlement Settlement => BaseManagers.Settlement.GetGameObject(this.Id);

        public CultureObject InitialCulture => Game.Current.ObjectManager.GetObject<CultureObject>(this.InitialCultureId);

        public IFaction InitialFaction => BaseManagers.Faction.GetGameObject(this.InitialFactionId);

        public BaseFactionInfo InitialFactionBaseInfo => BaseManagers.Faction.Get(this.InitialFactionId);

        public IFaction CurrentFaction => BaseManagers.Faction.GetGameObject(this.CurrentFactionId);

        public BaseFactionInfo CurrentFactionBaseInfo => BaseManagers.Faction.Get(this.CurrentFactionId);

        public IFaction PreviousFaction => BaseManagers.Faction.GetGameObject(this.PreviousFactionId);

        public BaseFactionInfo PreviousFactionBaseInfo => BaseManagers.Faction.Get(this.PreviousFactionId);

        #endregion

        public PartyBase Garrision => this.Settlement.Parties?.FirstOrDefault(party => party.IsGarrison)?.Party;

        public PartyBase Militia => this.Settlement.Parties?.FirstOrDefault(party => party.IsMilitia)?.Party;

        public bool IsOfImperialCulture => this.Settlement.Culture.Name.ToString().ToLower().Contains("empire");

        public bool IsInitialFactionOfImperialCulture => BaseManagers.Faction.GetGameObject(this.InitialFactionId).Name.ToString().ToLower().Contains("empire");

        public bool IsCurrentFactionOfImperialCulture => BaseManagers.Faction.GetGameObject(this.CurrentFactionId).Name.ToString().ToLower().Contains("empire");

        public bool IsPreviousFactionOfImperialCulture => BaseManagers.Faction.GetGameObject(this.PreviousFactionId).Name.ToString().ToLower().Contains("empire");

        #endregion

        #region Normal Properties



        #endregion
    }
}