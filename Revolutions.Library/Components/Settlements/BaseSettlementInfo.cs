using System;
using System.Linq;
using Revolutions.Library.Components.Factions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.Library.Components.Settlements
{
    [Serializable]
    public class BaseSettlementInfo : IBaseInfoType, IBaseComponent<BaseSettlementInfo>
    {
        #region IGameComponent<InfoType>

        public bool Equals(BaseSettlementInfo other)
        {
            return Id == other?.Id;
        }

        public override bool Equals(object other)
        {
            if (other is BaseSettlementInfo info)
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

        public BaseSettlementInfo()
        {

        }

        public BaseSettlementInfo(Settlement settlement)
        {
            Id = settlement.StringId;
            InitialCultureId = settlement.Culture.StringId;
            InitialFactionId = settlement.MapFaction.StringId;
            CurrentFactionId = settlement.MapFaction.StringId;
            PreviousFactionId = settlement.MapFaction.StringId;
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

        public Settlement Settlement => BaseManagers.Settlement.GetGameObject(Id);

        public CultureObject InitialCulture => Game.Current.ObjectManager.GetObject<CultureObject>(InitialCultureId);

        public IFaction InitialFaction => BaseManagers.Faction.GetGameObject(InitialFactionId);

        public BaseFactionInfo InitialFactionBaseInfo => BaseManagers.Faction.Get(InitialFactionId);

        public IFaction CurrentFaction => BaseManagers.Faction.GetGameObject(CurrentFactionId);

        public BaseFactionInfo CurrentFactionBaseInfo => BaseManagers.Faction.Get(CurrentFactionId);

        public IFaction PreviousFaction => BaseManagers.Faction.GetGameObject(PreviousFactionId);

        public BaseFactionInfo PreviousFactionBaseInfo => BaseManagers.Faction.Get(PreviousFactionId);

        #endregion

        public PartyBase Garrison => Settlement.Parties?.FirstOrDefault(party => party.IsGarrison)?.Party;

        public PartyBase Militia => Settlement.Parties?.FirstOrDefault(party => party.IsMilitia)?.Party;

        public bool IsOfImperialCulture => Settlement.Culture.Name.ToString().ToLower().Contains("empire");

        public bool IsInitialFactionOfImperialCulture => BaseManagers.Faction.GetGameObject(InitialFactionId).Name.ToString().ToLower().Contains("empire");

        public bool IsCurrentFactionOfImperialCulture => BaseManagers.Faction.GetGameObject(CurrentFactionId).Name.ToString().ToLower().Contains("empire");

        public bool IsPreviousFactionOfImperialCulture => BaseManagers.Faction.GetGameObject(PreviousFactionId).Name.ToString().ToLower().Contains("empire");

        #endregion

        #region Normal Properties



        #endregion
    }
}