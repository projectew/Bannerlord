﻿using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Revolutions.Library.Components.Events;
using Revolutions.Library.Components.Plots;
using Revolutions.Library.Helpers;
using Revolutions.Module.Components.Characters;
using Revolutions.Module.Components.CivilWars.Events.Plotting;
using Revolutions.Module.Components.CivilWars.Events.War;
using Revolutions.Module.Components.Kingdoms;
using Revolutions.Module.Settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using GameTexts = Revolutions.Module.Components.CivilWars.Localization.GameTexts;

namespace Revolutions.Module.Components.CivilWars.CampaignBehaviors
{
    internal class CivilWarsBehavior : CampaignBehaviorBase
    {
        private int _daysSinceConsider = 7;

        internal CivilWarsBehavior(CampaignGameStarter campaignGameStarter)
        {

        }

        public override void RegisterEvents()
        {
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, ClanChangedKingdom);
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, DailyTickEvent);
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

                clan.ClanLeaveKingdom();
            }

            Managers.Kingdom.DestroyKingdom(oldKingdom);
        }

        private void DailyTickEvent()
        {
            var considerableClans = Campaign.Current.Clans.Where(c => c.Kingdom != null && c.Leader != null && c.Leader.StringId != c.Kingdom.Leader.StringId && !c.IsBanditFaction && !c.IsClanTypeMercenary && !c.IsMafia && !c.IsMinorFaction && !c.IsNomad && !c.IsOutlaw && !c.IsRebelFaction && !c.IsSect && !c.IsUnderMercenaryService);
            foreach (var kingdomWithClans in considerableClans.GroupBy(c => c.Kingdom.StringId, (key, clans) => new { KingdomId = key, Clans = clans.ToList() }))
            {
                var kingdomInfo = Managers.Kingdom.Get(kingdomWithClans.KingdomId);
                if (kingdomInfo == null)
                {
                    continue;
                }

                if (Managers.CivilWar.GetCivilWarByKingdomId(kingdomWithClans.KingdomId) != null || kingdomInfo.HasCivilWar)
                {
                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Revolutions.CivilWars: {kingdomInfo.Kingdom.Name} has already a running Civil War.", ColorHelper.Gray));
                    }

                    continue;
                }

                if (kingdomInfo.Kingdom.Clans.Count < 2)
                {
                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Revolutions.CivilWars: {kingdomInfo.Kingdom.Name} has not enough clans.", ColorHelper.Gray));
                    }

                    continue;
                }

                if (RevolutionsSettings.Instance.DebugMode)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Revolutions.CivilWars: {kingdomInfo.Kingdom.Name} ({kingdomWithClans.Clans.Count} Clans)", ColorHelper.Gray));
                }

                CheckPlotState(kingdomInfo, kingdomWithClans.Clans);

                UpdatePlotState(kingdomInfo, kingdomWithClans.Clans);

                if (CheckPlayerPlotState(kingdomInfo))
                {
                    continue;
                }

                if (CheckWarState(kingdomInfo, kingdomWithClans.Clans))
                {
                }
            }
        }

        private void CheckPlotState(KingdomInfo kingdomInfo, List<Clan> clans)
        {
            foreach (var clan in clans)
            {
                if (clan.Leader.StringId == Hero.MainHero.StringId)
                {
                    continue;
                }

                if (RevolutionsSettings.Instance.DebugMode)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Revolutions.CivilWars: {clan.Name} ({clan.Leader})", ColorHelper.Gray));
                }

                var clanLeader = clan.Leader;
                var clanLeaderInfo = Managers.Character.Get(clanLeader.CharacterObject);

                var relationDifference = clanLeader.GetRelation(kingdomInfo.Kingdom.Leader);

                if (clanLeaderInfo.PlotState == PlotState.IsPlotting && relationDifference > RevolutionsSettings.Instance.CivilWarsPositiveRelationshipTreshold)
                {
                    clanLeaderInfo.PlotState = PlotState.IsLoyal;

                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("Revolutions: Is now loyal.", ColorHelper.Green));
                    }

                    var clanLeaderPlottingFriends = clans.Select(go => go.Leader)
                                                                          .Where(go => go.IsFriend(clanLeader) && go.StringId != Hero.MainHero.StringId)
                                                                          .Select(go => Managers.Character.Get(go.CharacterObject))
                                                                          .Where(info => info.PlotState == PlotState.IsPlotting)
                                                                          .ToList();

                    foreach (var friend in clanLeaderPlottingFriends)
                    {
                        if (!WillPlotting(friend.Character.HeroObject))
                        {
                            friend.PlotState = PlotState.IsLoyal;

                            if (RevolutionsSettings.Instance.DebugMode)
                            {
                                InformationManager.DisplayMessage(new InformationMessage($"Revolutions.CivilWars: Friend {friend.Character.Name} ({friend.Character.HeroObject.Clan.Name}) is now loyal.", ColorHelper.Gray));
                            }
                        }
                    }
                }

                if (relationDifference > RevolutionsSettings.Instance.CivilWarsNegativeRelationshipTreshold)
                {
                    continue;
                }

                if (WillPlotting(clanLeader))
                {
                    clanLeaderInfo.PlotState = PlotState.WillPlotting;

                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Revolutions: {clanLeader.Name} ({clanLeader.Clan.Name}) will be plotting.", ColorHelper.Gray));
                    }
                }
                else
                {
                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Revolutions: {clanLeader.Name} ({clanLeader.Clan.Name}) will not be plotting.", ColorHelper.Gray));
                    }
                }

                clanLeader = null;
                clanLeaderInfo = null;
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

            var kingdomClanLeaders = Campaign.Current.Clans.Where(w => w.Kingdom?.StringId == clanLeader.Clan.Kingdom?.StringId && w.Kingdom.Leader.StringId != clanLeader.StringId).Select(s => s.Leader).ToList();
            var clanLeaderFriends = kingdomClanLeaders.Where(w => w.IsFriend(clanLeader) && Managers.Character.Get(w.CharacterObject).PlotState == PlotState.IsPlotting).ToList();

            float personalityTraits = kingdomLeaderHonor + clanLeaderHonor;
            var personalityWeight = MathF.Pow(RevolutionsSettings.Instance.CivilWarsPlottingPersonalityMultiplier, -personalityTraits);
            float friendModifier = clanLeaderFriends.Count <= 0 ? 1 : clanLeaderFriends.Count;
            var friendWeight = MathF.Pow(RevolutionsSettings.Instance.CivilWarsPlottingFriendMultiplier, friendModifier);

            var plotChance = RevolutionsSettings.Instance.CivilWarsPlottingBaseChance * personalityWeight * friendWeight;

            if (RevolutionsSettings.Instance.DebugMode)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions.CivilWars: Plotting Chance is {plotChance}%.", ColorHelper.Gray));
            }

            return plotChance > new Random().Next(0, 100);
        }

        private void UpdatePlotState(KingdomInfo kingdomInfo, List<Clan> clans)
        {
            foreach (var clan in clans)
            {
                var clanLeader = clan.Leader;
                var clanLeaderInfo = Managers.Character.Get(clanLeader.CharacterObject);

                if (clanLeaderInfo.PlotState != PlotState.WillPlotting)
                {
                    continue;
                }

                clanLeaderInfo.PlotState = PlotState.IsPlotting;

                if (RevolutionsSettings.Instance.DebugMode)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Revolutions.CivilWars: {clanLeader.Name} ({clanLeader.Clan.Name}) is plotting.", ColorHelper.Gray));
                }
            }
        }

        private bool CheckPlayerPlotState(KingdomInfo kingdomInfo)
        {
            if (kingdomInfo.Id == Hero.MainHero.Clan.Kingdom?.StringId && kingdomInfo.Kingdom.Leader != Hero.MainHero)
            {
                if (EventManager.Instance.InEvent)
                {
                    return true;
                }

                var mainHeroInfo = Managers.Character.Get(Hero.MainHero.CharacterObject);
                if (_daysSinceConsider >= RevolutionsSettings.Instance.CivilWarsPlottingConsiderTime
                    && mainHeroInfo.PlotState == PlotState.Considering && mainHeroInfo.DecisionMade == DecisionMade.No)
                {
                    new PlottingEvent();
                    _daysSinceConsider = 0;
                }
                else
                {
                    _daysSinceConsider++;
                }
            }

            return false;
        }

        private bool CheckWarState(KingdomInfo kingdomInfo, List<Clan> clans)
        {
            var loyalClans = new List<Clan>();
            var plottingClans = new List<Clan>();

            foreach (var clan in clans)
            {
                var clanLeader = clan.Leader;
                var clanLeaderInfo = Managers.Character.Get(clanLeader.CharacterObject);

                if (clanLeaderInfo.PlotState == PlotState.IsLoyal)
                {
                    loyalClans.Add(clan);
                }
                else if (clanLeaderInfo.PlotState == PlotState.IsPlotting)
                {
                    plottingClans.Add(clan);
                }
            }

            if (plottingClans.Count == 0)
            {
                if (RevolutionsSettings.Instance.DebugMode)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Revolutions.CivilWars: There are no plotting clans.", ColorHelper.Gray));
                }

                return true;
            }

            var loyalTroopWeight = kingdomInfo.Kingdom.RulingClan.TotalStrength;
            foreach (var loyalClan in loyalClans)
            {
                loyalTroopWeight += loyalClan.TotalStrength;
            }

            var plottersTroopWeight = 0f;
            foreach (var plotterClan in plottingClans)
            {
                plottersTroopWeight += plotterClan.TotalStrength;
            }

            var plotLeadingClan = plottingClans.OrderByDescending(go => go.Tier).FirstOrDefault();
            if (plotLeadingClan == null)
            {
                return true;
            }

            var kingdomLeader = kingdomInfo.Kingdom.Leader;
            var plottingLeader = plotLeadingClan.Leader;

            var kingdomLeaderGenerosity = kingdomLeader.GetHeroTraits().Generosity;
            var kingdomLeaderMercy = kingdomLeader.GetHeroTraits().Mercy;
            var kingdomLeaderValor = kingdomLeader.GetHeroTraits().Valor;
            var kingdomLeaderCalculating = kingdomLeader.GetHeroTraits().Calculating;

            var plottingLeaderGenerosity = plottingLeader.GetHeroTraits().Generosity;
            var plottingLeaderMercy = plottingLeader.GetHeroTraits().Mercy;
            var plottingLeaderValor = plottingLeader.GetHeroTraits().Valor;
            var plottingLeaderCalculating = plottingLeader.GetHeroTraits().Calculating;

            float personalityTraits = plottingLeaderGenerosity + kingdomLeaderGenerosity + plottingLeaderMercy + kingdomLeaderMercy;
            var personalityWeight = MathF.Pow(RevolutionsSettings.Instance.CivilWarsWarPersonalityMultiplier, -personalityTraits);
            var troopWeight = plottersTroopWeight / loyalTroopWeight;
            float valorModifier = 1 + (plottingLeaderValor <= 0 ? 1 : plottingLeaderValor * 2);
            float clanCountModifier = plottingClans.Count / (loyalClans.Count + 1);
            float calculatingModifier = 1 + (plottingLeaderCalculating <= 0 ? 1 : plottingLeaderCalculating);

            var warChance = RevolutionsSettings.Instance.CivilWarsWarBaseChance * personalityWeight * (troopWeight * valorModifier) * MathF.Pow(clanCountModifier, calculatingModifier);

            if (RevolutionsSettings.Instance.DebugMode)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Revolutions.CivilWars: War Chance is {warChance}%.", ColorHelper.Gray));
            }

            if (warChance > new Random().Next(0, 100))
            {
                if (kingdomInfo.Id == Hero.MainHero.Clan.Kingdom?.StringId && EventManager.Instance.InEvent)
                {
                    return true;
                }

                if (kingdomInfo.Id == Hero.MainHero.Clan.Kingdom?.StringId && kingdomLeader.StringId != Hero.MainHero.StringId)
                {
                    new WarEvent();
                }

                loyalClans.Clear();
                plottingClans.Clear();
                foreach (var clan in clans)
                {
                    var clanLeader = clan.Leader;
                    var clanLeaderInfo = Managers.Character.Get(clanLeader.CharacterObject);

                    if (clanLeaderInfo.PlotState == PlotState.IsLoyal)
                    {
                        loyalClans.Add(clan);
                    }
                    else if (clanLeaderInfo.PlotState == PlotState.IsPlotting)
                    {
                        plottingClans.Add(clan);
                    }
                }

                if (plottingClans.Count == 0)
                {
                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("Revolutions.CivilWars: There are no plotting clans.", ColorHelper.Gray));
                    }

                    return true;
                }

                Managers.Character.Get(plottingLeader.CharacterObject).IsCivilWarKingdomLeader = true;

                var settlementInfo = Managers.Settlement.Get(plotLeadingClan.HomeSettlement);
                var bannerInfo = Managers.Banner.GetBanner(settlementInfo);

                var textObject = new TextObject(GameTexts.CivilWarsKingdom);
                textObject.SetTextVariable("CLAN", plottingLeader.Name);
                var plotKingdom = Managers.Kingdom.CreateKingdom(plotLeadingClan.Leader, textObject, textObject, bannerInfo != null ? new Banner(bannerInfo.BannerId) : plotLeadingClan.Banner);
                Managers.Kingdom.Get(plotKingdom).IsCivilWarKingdom = true;

                ChangeRelationAction.ApplyRelationChangeBetweenHeroes(plottingLeader, kingdomLeader, -(int)(RevolutionsSettings.Instance.CivilWarsRelationshipChange * (RevolutionsSettings.Instance.CivilWarsRelationshipChangeMultiplier * 2)), false);

                foreach (var clan1 in clans.Where(c1 => c1.StringId != plotLeadingClan.StringId))
                {
                    var clanLeader1 = Managers.Character.Get(clan1.Leader.CharacterObject);
                    if (clanLeader1.PlotState == PlotState.IsLoyal)
                    {
                        ChangeRelationAction.ApplyRelationChangeBetweenHeroes(clan1.Leader, kingdomLeader, (int)(RevolutionsSettings.Instance.CivilWarsRelationshipChange * RevolutionsSettings.Instance.CivilWarsRelationshipChangeMultiplier), false);
                        ChangeRelationAction.ApplyRelationChangeBetweenHeroes(clan1.Leader, plottingLeader, -(int)(RevolutionsSettings.Instance.CivilWarsRelationshipChange * RevolutionsSettings.Instance.CivilWarsRelationshipChangeMultiplier), false);
                    }
                    else if (clanLeader1.PlotState == PlotState.IsPlotting)
                    {
                        ChangeRelationAction.ApplyRelationChangeBetweenHeroes(clan1.Leader, plottingLeader, (int)(RevolutionsSettings.Instance.CivilWarsRelationshipChange * RevolutionsSettings.Instance.CivilWarsRelationshipChangeMultiplier), false);
                        ChangeRelationAction.ApplyRelationChangeBetweenHeroes(clan1.Leader, kingdomLeader, -(int)(RevolutionsSettings.Instance.CivilWarsRelationshipChange * RevolutionsSettings.Instance.CivilWarsRelationshipChangeMultiplier), false);
                    }

                    foreach (var clan2 in clans.Where(c2 => c2.StringId != clan1.StringId && c2.StringId != plotLeadingClan.StringId))
                    {
                        var clanLeader2 = Managers.Character.Get(clan2.Leader.CharacterObject);

                        if (clanLeader1.PlotState == clanLeader2.PlotState)
                        {
                            ChangeRelationAction.ApplyRelationChangeBetweenHeroes(clan1.Leader, clan2.Leader, RevolutionsSettings.Instance.CivilWarsRelationshipChange, false);
                        }
                        else
                        {
                            ChangeRelationAction.ApplyRelationChangeBetweenHeroes(clan1.Leader, clan2.Leader, -RevolutionsSettings.Instance.CivilWarsRelationshipChange, false);
                        }
                    }
                }

                textObject = new TextObject(GameTexts.CivilWarsWarMapNotification);
                textObject.SetTextVariable("PLOTKINGDOM", plotKingdom.Name);
                textObject.SetTextVariable("KINGDOM", kingdomInfo.Kingdom.Name);
                InformationManager.AddNotice(new WarMapNotification(plotKingdom, kingdomInfo.Kingdom, textObject));

                textObject = new TextObject(GameTexts.CivilWarsQuickNotification);
                textObject.SetTextVariable("KINGDOM", kingdomInfo.Kingdom.Name);
                textObject.SetTextVariable("LEADER", plotLeadingClan.Leader.Name);
                textObject.SetTextVariable("CLAN", plotLeadingClan.Name);
                InformationManager.AddQuickInformation(textObject);

                var otherPlottingClans = plottingClans.Where(go => go.StringId != plotLeadingClan.StringId);
                if (otherPlottingClans.Count() == 0)
                {
                    textObject = new TextObject(GameTexts.CivilWarsLeaderAlone);
                    textObject.SetTextVariable("LEADER", plotLeadingClan.Leader.Name);
                    textObject.SetTextVariable("CLAN", plotLeadingClan.Name);
                    InformationManager.DisplayMessage(new InformationMessage(textObject.ToString(), ColorHelper.Orange));
                }

                foreach (var plottingClan in otherPlottingClans)
                {
                    textObject = new TextObject(GameTexts.CivilWarsJoinsConspirators);
                    textObject.SetTextVariable("LEADER", plottingClan.Leader.Name);
                    textObject.SetTextVariable("CLAN", plottingClan.Name);
                    InformationManager.DisplayMessage(new InformationMessage(textObject.ToString(), ColorHelper.Orange));

                    ChangeKingdomAction.ApplyByJoinToKingdom(plottingClan, plotKingdom, false);
                }

                foreach (var loyalClan in loyalClans)
                {
                    textObject = new TextObject(GameTexts.CivilWarsJoinsConspirators);
                    textObject.SetTextVariable("LEADER", loyalClan.Leader.Name);
                    textObject.SetTextVariable("CLAN", loyalClan.Name);
                    InformationManager.DisplayMessage(new InformationMessage(textObject.ToString(), ColorHelper.Orange));
                }

                DeclareWarAction.Apply(plotKingdom, kingdomInfo.Kingdom);

                if (RevolutionsSettings.Instance.CivilWarsKeepExistingWars)
                {
                    foreach (var enemyFaction in FactionManager.GetEnemyFactions(kingdomInfo.Kingdom).ToList())
                    {
                        if (!enemyFaction.Equals(plotKingdom))
                        {
                            DeclareWarAction.Apply(plotKingdom, enemyFaction);
                        }
                    }
                }

                Managers.CivilWar.CivilWars.Add(new CivilWar(kingdomInfo.Kingdom, clans));
                kingdomInfo.HasCivilWar = true;
            }

            return false;
        }
    }
}