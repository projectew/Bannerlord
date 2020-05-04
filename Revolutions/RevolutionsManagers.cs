﻿using ModLibrary.Components.Kingdoms;
using ModLibrary.Components.Factions;
using ModLibrary.Components.Clans;
using ModLibrary.Components.Parties;
using ModLibrary.Components.Characters;
using ModLibrary.Components.Settlements;
using Revolutions.Components.Kingdoms;
using Revolutions.Components.Factions;
using Revolutions.Components.Clans;
using Revolutions.Components.Parties;
using Revolutions.Components.Characters;
using Revolutions.Components.Settlements;
using Revolutions.Components.Revolutions;

namespace Revolutions
{
    public static class RevolutionsManagers
    {
        public static KingdomManager<KingdomInfoRevolutions> KingdomManager { get; } = KingdomManager<KingdomInfoRevolutions>.Instance;

        public static FactionManager<FactionInfoRevolutions> FactionManager { get; } = FactionManager<FactionInfoRevolutions>.Instance;

        public static ClanManager<ClanInfoRevolutions> ClanManager { get; } = ClanManager<ClanInfoRevolutions>.Instance;

        public static PartyManager<PartyInfoRevolutions> PartyManager { get; } = PartyManager<PartyInfoRevolutions>.Instance;

        public static CharacterManager<CharacterInfoRevolutions> CharacterManager { get; } = CharacterManager<CharacterInfoRevolutions>.Instance;

        public static SettlementManager<SettlementInfoRevolutions> SettlementManager { get; } = SettlementManager<SettlementInfoRevolutions>.Instance;

        public static RevolutionManager RevolutionManager { get; } = RevolutionManager.Instance;
    }
}