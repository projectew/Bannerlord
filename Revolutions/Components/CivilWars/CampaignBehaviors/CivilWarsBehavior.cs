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
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, bool, bool>(this.ClanChangedKingdom));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, bool byRebellion, bool showNotification)
        {
            if (oldKingdom == null)
            {
                return;
            }

            var kingdomInfo = Managers.Kingdom.Get(oldKingdom);
            if (kingdomInfo == null || !kingdomInfo.IsCivilWarKingdom)
            {
                return;
            }

            var clans = oldKingdom.Clans.Where(go => !go.IsUnderMercenaryService && !go.IsClanTypeMercenary);
            if (clans.Count() > 0)
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