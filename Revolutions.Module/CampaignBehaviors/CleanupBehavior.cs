using System.Collections.Generic;
using System.Linq;
using Revolutions.Module.Components.Kingdoms;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Module.CampaignBehaviors
{
    internal class CleanupBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, DailyTickEvent);

            CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, KingdomDestroyedEvent);
            CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, OnClanDestroyedEvent);
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, MobilePartyDestroyed);
            CampaignEvents.OnPartyRemovedEvent.AddNonSerializedListener(this, OnPartyRemovedEvent);
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void DailyTickEvent()
        {
            DestroyGhostKingdoms();
            RemoveInvalidInfos();
        }

        private void KingdomDestroyedEvent(Kingdom kingdom)
        {
            var kingdomInfo = Managers.Kingdom.Get(kingdom);
            if (kingdomInfo != null && kingdomInfo.IsCustomKingdom)
            {
                Managers.Kingdom.RemoveKingdom(kingdom);
            }
        }

        private void OnClanDestroyedEvent(Clan clan)
        {
            var clanInfo = Managers.Clan.Get(clan);
            if (clanInfo != null && clanInfo.IsCustomClan)
            {
                Managers.Clan.RemoveClan(clan);
            }
        }

        private void MobilePartyDestroyed(MobileParty mobileParty, PartyBase party)
        {
            var partyInfo = Managers.Party.GetInfo(party) ?? Managers.Party.GetInfo(mobileParty.Party);
            if (partyInfo != null && partyInfo.IsCustomParty)
            {
                Managers.Party.Remove(mobileParty.Party.Id);
            }
        }

        private void OnPartyRemovedEvent(PartyBase party)
        {
            var partyInfo = Managers.Party.GetInfo(party);
            if (partyInfo != null && partyInfo.IsCustomParty)
            {
                Managers.Party.Remove(party.Id);
            }
        }

        private void DestroyGhostKingdoms()
        {
            var kingdomsToRemove = new List<KingdomInfo>();
            var kingdomsToDestroy = new List<Kingdom>();

            foreach (var kingdomInfo in Managers.Kingdom.Infos.Where(i => i.IsCustomKingdom))
            {
                if (kingdomInfo.Kingdom == null)
                {
                    kingdomsToRemove.Add(kingdomInfo);
                }
                else if (kingdomInfo.Kingdom.Leader == null || !kingdomInfo.Kingdom.Settlements.Any() &&
                    (kingdomInfo.Kingdom.Parties == null || !kingdomInfo.Kingdom.Parties.Any() || kingdomInfo.Kingdom.Leader.IsDead || kingdomInfo.Kingdom.Leader.IsPrisoner))
                {
                    kingdomsToDestroy.Add(kingdomInfo.Kingdom);
                }
            }

            foreach (var kingdomInfo in kingdomsToRemove)
            {
                Managers.Kingdom.Remove(kingdomInfo.Id);
            }

            foreach (var kingdom in kingdomsToDestroy)
            {
                Managers.Kingdom.DestroyKingdom(kingdom);
            }
        }

        private void RemoveInvalidInfos()
        {
            Managers.Faction.RemoveInvalids();
            Managers.Kingdom.RemoveInvalids();
            Managers.Clan.RemoveInvalids();
            Managers.Party.RemoveInvalids();
            Managers.Character.RemoveInvalids();
            Managers.Settlement.RemoveInvalids();
        }
    }
}