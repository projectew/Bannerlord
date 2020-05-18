using Helpers;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Revolutions.Components.CivilWars.CampaignBehaviors
{
    internal class CivilWarsBehavior : CampaignBehaviorBase
    {
        internal CivilWarsBehavior(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddBehavior(new CivilWarsDailyBehavior());
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoadedEvent));
            CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.KingdomDestroyedEvent));
            CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.OnClanDestroyedEvent));
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, bool, bool>(this.ClanChangedKingdom));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void OnGameLoadedEvent(CampaignGameStarter obj)
        {
            DataStorage.LoadCivilWarData();
        }

        private void KingdomDestroyedEvent(Kingdom kingdom)
        {
            var kingdomInfo = Managers.Kingdom.GetInfo(kingdom);
            if (kingdomInfo != null && kingdomInfo.IsCivilWarKingdom)
            {
                Managers.Kingdom.RemoveKingdom(kingdom);
            }
        }

        private void OnClanDestroyedEvent(Clan clan)
        {
            var clanInfo = Managers.Clan.GetInfo(clan);
            if (clanInfo != null && clanInfo.IsCivilWarClan)
            {
                Managers.Clan.RemoveClan(clan);
            }
        }

        private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, bool byRebellion, bool showNotification)
        {
            if(oldKingdom == null)
            {
                return;
            }

            var kingdomInfo = Managers.Kingdom.GetInfo(oldKingdom);
            if(kingdomInfo == null || !kingdomInfo.IsCivilWarKingdom)
            {
                return;
            }

            var clans = oldKingdom.Clans.Where(go => !go.IsUnderMercenaryService && !go.IsClanTypeMercenary);
            if(clans.Count() > 0)
            {
                return;
            }

            foreach (var currentClan in oldKingdom.Clans.ToList())
            {
                foreach (var enemyFaction in Campaign.Current.Factions.Where(go => go.IsAtWarWith(currentClan)))
                {
                    if (clan.IsAtWarWith(enemyFaction))
                    {
                        FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction(enemyFaction, currentClan);
                        FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction(currentClan, enemyFaction);
                    }
                }

                clan.ClanLeaveKingdom(false);
            }

            Managers.Kingdom.DestroyKingdom(oldKingdom);
        }
    }
}