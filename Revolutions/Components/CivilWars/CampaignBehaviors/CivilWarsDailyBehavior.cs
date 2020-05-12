using Revolutions.Components.Base.Characters;
using Revolutions.Components.Base.Clans;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Revolutions.Components.CivilWars.CampaignBehaviors
{
    internal class CivilWarsDailyBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTickEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void DailyTickEvent()
        {
            //Check if clans will be plotting
            foreach (var clan in Campaign.Current.Clans)
            {
                var clanLeader = clan.Leader;
                if (clanLeader == null)
                {
                    continue;
                }

                var kingdomLeader = clan.Kingdom?.Leader;
                if (kingdomLeader == null)
                {
                    continue;
                }

                var clanLeaderInfo = RevolutionsManagers.Character.GetInfo(clanLeader.CharacterObject);
                var clanInfo = RevolutionsManagers.Clan.GetInfo(clanLeader.Clan);

                var clanLeadersOfKingdom = Campaign.Current.Clans.Where(w => w.Kingdom?.StringId == clanLeader.Clan.Kingdom?.StringId).Select(s => s.Leader).ToList();
                var clanLeaderFriendsInsideKingdom = clanLeadersOfKingdom.Where(w => w.IsFriend(clanLeader)).ToList();

                var relationBetweenClanLeaderAndKingdomLeader = clanLeader.GetRelation(kingdomLeader);

                if (clanLeaderInfo.PlotState == PlotState.IsPlotting && relationBetweenClanLeaderAndKingdomLeader > Settings.Instance.CivilWarsLoyalRelationshipTreshold)
                {
                    clanLeaderInfo.PlotState = PlotState.IsLoyal;

                    foreach (var clanLeaderFriendInfo in clanLeaderFriendsInsideKingdom.Select(s => RevolutionsManagers.Character.GetInfo(s.CharacterObject)).Where(w => w.PlotState == PlotState.IsPlotting))
                    {
                        var clanLeaderFriend = clanLeaderFriendInfo.Character.HeroObject;
                        var friendWillPlotting = this.WillPlotting(clanLeaderFriend);
                        if (!friendWillPlotting)
                        {
                            RevolutionsManagers.Character.GetInfo(clanLeaderFriend.CharacterObject).PlotState = PlotState.IsLoyal;
                        }
                    }

                    continue;
                }

                if (relationBetweenClanLeaderAndKingdomLeader > Settings.Instance.CivilWarsPlottingRelationshipTreshold)
                {
                    continue;
                }

                if (this.WillPlotting(clanLeader))
                {
                    RevolutionsManagers.Character.GetInfo(clanLeader.CharacterObject).PlotState = PlotState.WillPlotting;
                }
            }

            //Check clans who are willed to plot and set IsPlotting
            foreach (var clan in Campaign.Current.Clans)
            {
                var clanLeaderInfo = RevolutionsManagers.Character.GetInfo(clan.Leader.CharacterObject);
                if (clanLeaderInfo.PlotState != PlotState.WillPlotting)
                {
                    continue;
                }

                clanLeaderInfo.PlotState = PlotState.IsPlotting;
            }

            foreach (var kingdom in Campaign.Current.Kingdoms)
            {
                var civilWar = RevolutionsManagers.CivilWar.GetCivilWarByKingdom(kingdom);
                if (civilWar != null)
                {
                    continue;
                }

                var kingdomClans = Campaign.Current.Clans.Where(w => w.Kingdom?.StringId == kingdom.StringId).ToList();
                var kingdomLoyalClans = new List<ClanInfo>();
                var kingdomPlottingClans = new List<ClanInfo>();
                foreach (var clan in kingdomClans)
                {
                    var clanInfo = RevolutionsManagers.Clan.GetInfo(clan);
                    var characterInfo = RevolutionsManagers.Character.GetInfo(clan.Leader.CharacterObject);

                    if (characterInfo.PlotState == PlotState.IsLoyal)
                    {
                        kingdomLoyalClans.Add(clanInfo);
                    }
                    else if (characterInfo.PlotState == PlotState.IsPlotting)
                    {
                        kingdomPlottingClans.Add(clanInfo);
                    }
                }

                if (kingdomPlottingClans.Count == 0)
                {
                    continue;
                }

                var loyalTroopWeight = 0f;
                foreach (var loyalClan in kingdomLoyalClans)
                {
                    loyalTroopWeight += loyalClan.Clan.TotalStrength;
                }

                var plottersTroopWeight = 0f;
                foreach (var plotterClan in kingdomPlottingClans)
                {
                    plottersTroopWeight += plotterClan.Clan.TotalStrength;
                }

                var plotLeader = kingdomPlottingClans.OrderByDescending(o => o.Clan.Tier).FirstOrDefault();

                var kingdomLeader = kingdom.Leader;
                var plottingLeader = plotLeader.Clan.Leader;

                var kingdomLeaderGenerosity = kingdomLeader.GetHeroTraits().Generosity;
                var kingdomLeaderMercy = kingdomLeader.GetHeroTraits().Mercy;
                var kingdomLeaderValor = kingdomLeader.GetHeroTraits().Valor;
                var kingdomLeaderCalculating = kingdomLeader.GetHeroTraits().Calculating;

                var plottingLeaderGenerosity = plottingLeader.GetHeroTraits().Generosity;
                var plottingLeaderMercy = plottingLeader.GetHeroTraits().Mercy;
                var plottingLeaderValor = plottingLeader.GetHeroTraits().Valor;
                var plottingLeaderCalculating = plottingLeader.GetHeroTraits().Calculating;

                float personalityTraits = plottingLeaderGenerosity + kingdomLeaderGenerosity + plottingLeaderMercy + kingdomLeaderMercy;
                float personalityWeight = MathF.Pow(Settings.Instance.CivilWarsWarPersonalityMultiplier, -personalityTraits);
                float troopWeight = plottersTroopWeight / loyalTroopWeight;
                float valorModifier = 1 + (plottingLeaderValor <= 0 ? 1 : plottingLeaderValor * 2);
                float clanCountModifier = kingdomPlottingClans.Count / kingdomLoyalClans.Count;
                float calculatingModifier = 1 + (plottingLeaderCalculating <= 0 ? 1 : plottingLeaderCalculating);

                var warChance = Settings.Instance.CivilWarsWarBaseChance * personalityWeight * (troopWeight * valorModifier) * MathF.Pow(clanCountModifier, calculatingModifier);

                if (warChance > new Random().Next(0, 100))
                {
                    var newKingdom = RevolutionsManagers.Kingdom.CreateKingdom(plottingLeader, new TextObject($"Kingdom of {plottingLeader.Clan.Name}"), new TextObject($"Kingdom of {plottingLeader.Clan.Name}"));
                    ChangeKingdomAction.ApplyByLeaveKingdom(plotLeader.Clan, false);

                    foreach (var plottingClan in kingdomPlottingClans)
                    {
                        ChangeKingdomAction.ApplyByLeaveWithRebellionAgainstKingdom(plottingClan.Clan, newKingdom, true);
                    }

                    FactionManager.DeclareWar(newKingdom, kingdom);
                    Campaign.Current.FactionManager.RegisterCampaignWar(newKingdom, kingdom);

                    RevolutionsManagers.CivilWar.CivilWars.Add(new CivilWar(kingdom, kingdomClans));
                }
            }
        }

        private bool WillPlotting(Hero clanLeader)
        {
            var kingdomLeader = clanLeader.Clan?.Kingdom?.Leader;
            if (kingdomLeader == null)
            {
                return false;
            }

            var kingdomLeaderHonor = kingdomLeader.GetHeroTraits().Honor;
            var clanLeaderHonor = clanLeader.GetHeroTraits().Honor;

            var kingdomClanLeaders = Campaign.Current.Clans.Where(w => w.Kingdom?.StringId == clanLeader.Clan.Kingdom?.StringId).Select(s => s.Leader).ToList();
            var clanLeaderFriends = kingdomClanLeaders.Where(w => w.IsFriend(clanLeader)).ToList();

            float personalityTraits = kingdomLeaderHonor + clanLeaderHonor;
            float personalityWeight = MathF.Pow(Settings.Instance.CivilWarsPlottingPersonalityMultiplier, -personalityTraits);
            float friendWeight = MathF.Pow(Settings.Instance.CivilWarsPlottingBaseChance, clanLeaderFriends.Count);

            return Settings.Instance.CivilWarsPlottingBaseChance * personalityWeight * friendWeight > new Random().Next(0, 100);
        }
    }
}