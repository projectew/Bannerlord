using KNTLibrary.Helpers;
using Revolutions.Components.Base.Characters;
using Revolutions.Components.CivilWars.Events.Plotting;
using Revolutions.Components.CivilWars.Events.War;
using Revolutions.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using KNTLibrary.Components.Events;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Revolutions.Components.CivilWars.CampaignBehaviors
{
    internal class CivilWarsDailyBehavior : CampaignBehaviorBase
    {
        private int _daysSinceConsider = 7;

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTickEvent));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void DailyTickEvent()
        {
            var considerableClans = Campaign.Current.Clans.Where(c => c.Kingdom != null && c.Leader.StringId != c.Kingdom.Leader.StringId && !c.IsBanditFaction && !c.IsClanTypeMercenary && !c.IsMafia && !c.IsMinorFaction && !c.IsNomad && !c.IsOutlaw && !c.IsRebelFaction && !c.IsSect && !c.IsUnderMercenaryService);
            foreach (var kingdomWithClans in considerableClans.GroupBy(c => c.Kingdom.StringId, (key, clans) => new { KingdomId = key, Clans = clans.ToList() }))
            {
                var kingdomInfo = Managers.Kingdom.GetInfo(kingdomWithClans.KingdomId);

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

                Hero clanLeader;
                CharacterInfo clanLeaderInfo;

                #region Check PlotState

                foreach (var clan in kingdomWithClans.Clans)
                {
                    if (clan.Leader.StringId == Hero.MainHero.StringId)
                    {
                        continue;
                    }

                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Revolutions.CivilWars: {clan.Name} ({clan.Leader})", ColorHelper.Gray));
                    }

                    clanLeader = clan.Leader;
                    clanLeaderInfo = Managers.Character.GetInfo(clanLeader.CharacterObject);

                    var relationDifference = clanLeader.GetRelation(kingdomInfo.Kingdom.Leader);

                    if (clanLeaderInfo.PlotState == PlotState.IsPlotting && relationDifference > RevolutionsSettings.Instance.CivilWarsPositiveRelationshipTreshold)
                    {
                        clanLeaderInfo.PlotState = PlotState.IsLoyal;

                        if (RevolutionsSettings.Instance.DebugMode)
                        {
                            InformationManager.DisplayMessage(new InformationMessage($"Revolutions: Is now loyal.", ColorHelper.Green));
                        }

                        var clanLeaderPlottingFriends = kingdomWithClans.Clans.Select(go => go.Leader)
                                                                              .Where(go => go.IsFriend(clanLeader) && go.StringId != Hero.MainHero.StringId)
                                                                              .Select(go => Managers.Character.GetInfo(go.CharacterObject))
                                                                              .Where(info => info.PlotState == PlotState.IsPlotting)
                                                                              .ToList();

                        foreach (var friend in clanLeaderPlottingFriends)
                        {
                            if (!this.WillPlotting(friend.Character.HeroObject))
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

                    if (this.WillPlotting(clanLeader))
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

                #endregion

                #region Update PlotState

                foreach (var clan in kingdomWithClans.Clans)
                {
                    clanLeader = clan.Leader;
                    clanLeaderInfo = Managers.Character.GetInfo(clanLeader.CharacterObject);

                    if (clanLeaderInfo.PlotState != PlotState.WillPlotting)
                    {
                        continue;
                    }

                    clanLeaderInfo.PlotState = PlotState.IsPlotting;

                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Revolutions.CivilWars: {clanLeader.Name} ({clanLeader.Clan.Name}) is plotting.", ColorHelper.Gray));
                    }

                    clanLeader = null;
                    clanLeaderInfo = null;
                }

                #endregion

                if (kingdomInfo.Id == Hero.MainHero.Clan.Kingdom?.StringId && kingdomInfo.Kingdom.Leader != Hero.MainHero)
                {
                    if (EventManager.Instance.InEvent)
                    {
                        continue;
                    }
                    
                    if (this._daysSinceConsider >= RevolutionsSettings.Instance.CivilWarsPlottingConsiderTime && 
                        (Managers.Character.GetInfo(Hero.MainHero.CharacterObject).PlotState == PlotState.Considering ||
                        Managers.Character.GetInfo(Hero.MainHero.CharacterObject).PlotState == PlotState.IsLoyal) &&
                        Managers.Character.GetInfo(Hero.MainHero.CharacterObject).DecisionMade == DecisionMade.No)
                    {
                        var plottingEvent = new PlottingEvent();
                        this._daysSinceConsider = 0;
                    }
                    else
                    {
                        this._daysSinceConsider++;
                    }
                }

                #region Check War

                var loyalClans = new List<Clan>();
                var plottingClans = new List<Clan>();

                foreach (var clan in kingdomWithClans.Clans)
                {
                    clanLeader = clan.Leader;
                    clanLeaderInfo = Managers.Character.GetInfo(clanLeader.CharacterObject);

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
                        InformationManager.DisplayMessage(new InformationMessage($"Revolutions.CivilWars: There are no plotting clans.", ColorHelper.Gray));
                    }

                    continue;
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
                    continue;
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
                        continue;;
                    }
                    
                    loyalClans.Clear();
                    plottingClans.Clear();
                    foreach (var clan in kingdomWithClans.Clans)
                    {
                        clanLeader = clan.Leader;
                        clanLeaderInfo = Managers.Character.GetInfo(clanLeader.CharacterObject);

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
                            InformationManager.DisplayMessage(new InformationMessage($"Revolutions.CivilWars: There are no plotting clans.", ColorHelper.Gray));
                        }

                        continue;
                    }

                    Managers.Character.GetInfo(plottingLeader.CharacterObject).IsCivilWarKingdomLeader = true;

                    var settlementInfo = Managers.Settlement.GetInfo(plotLeadingClan.HomeSettlement);
                    var bannerInfo = Managers.Banner.GetRevolutionsBannerBySettlementInfo(settlementInfo);

                    var plotKingdom = Managers.Kingdom.CreateKingdom(plotLeadingClan.Leader, new TextObject($"Kingdom of {plottingLeader.Clan.Name}"), new TextObject($"Kingdom of {plottingLeader.Clan.Name}"), bannerInfo != null ? new Banner(bannerInfo.BannerId) : plotLeadingClan.Banner, false);
                    Managers.Kingdom.GetInfo(plotKingdom).IsCivilWarKingdom = true;

                    ChangeRelationAction.ApplyRelationChangeBetweenHeroes(plottingLeader, kingdomLeader, -(int)(RevolutionsSettings.Instance.CivilWarsRelationshipChange * (RevolutionsSettings.Instance.CivilWarsRelationshipChangeMultiplier * 2)), false);

                    foreach (var clan1 in kingdomWithClans.Clans.Where(c1 => c1.StringId != plotLeadingClan.StringId))
                    {
                        var clanLeader1 = Managers.Character.GetInfo(clan1.Leader.CharacterObject);
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

                        foreach (var clan2 in kingdomWithClans.Clans.Where(c2 => c2.StringId != clan1.StringId && c2.StringId != plotLeadingClan.StringId))
                        {
                            var clanLeader2 = Managers.Character.GetInfo(clan2.Leader.CharacterObject);

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

                    try
                    {
                        InformationManager.AddNotice(new WarMapNotification(plotKingdom, kingdomInfo.Kingdom, new TextObject($"Civil War: {kingdomInfo.Kingdom.Name} vs. {plotKingdom.Name}")));
                        InformationManager.AddQuickInformation(new TextObject($"A Civil War has broken out in {kingdomInfo.Kingdom.Name}! It's lead by the mighty {plotLeadingClan.Leader.Name} of {plotLeadingClan.Name}."));
                    }
                    catch (Exception)
                    {
                        InformationManager.AddQuickInformation(new TextObject($"A Civil War has broken out in {kingdomInfo.Kingdom.Name}! It's lead by the mighty {plotLeadingClan.Leader.Name} of {plotLeadingClan.Name}."));
                    }

                    var otherPlottingClans = plottingClans.Where(go => go.StringId != plotLeadingClan.StringId);
                    if (otherPlottingClans.Count() == 0)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Seems like {plotLeadingClan.Leader.Name} of {plotLeadingClan.Name} will be on his own.", ColorHelper.Orange));
                    }

                    foreach (var plottingClan in otherPlottingClans)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"{plottingClan.Leader.Name} of {plottingClan.Name} will be with the conspiracy leader!", ColorHelper.Orange));
                        ChangeKingdomAction.ApplyByJoinToKingdom(plottingClan, plotKingdom, false);
                    }

                    DeclareWarAction.Apply(plotKingdom, kingdomInfo.Kingdom);
                    
                    if (kingdomInfo.Id == Hero.MainHero.Clan.Kingdom?.StringId)
                    {
                        var warEvent = new WarEvent(plotKingdom, kingdomInfo.Kingdom); 
                    }
                    
                    if (RevolutionsSettings.Instance.CivilWarsKeepExistingWars)
                    {
                        foreach (var enemyFaction in FactionManager.GetEnemyFactions(kingdomInfo.Kingdom).ToList())
                        {
                            //for some reason sometimes kingdoms are considered to be at war with themselves
                            if (!enemyFaction.Equals(plotKingdom))
                            {
                                DeclareWarAction.Apply(plotKingdom, enemyFaction);
                            }
                        }
                    }

                    Managers.CivilWar.CivilWars.Add(new CivilWar(kingdomInfo.Kingdom, kingdomWithClans.Clans));
                    kingdomInfo.HasCivilWar = true;
                }

                #endregion
            }
        }

        internal bool WillPlotting(Hero clanLeader)
        {
            var kingdomLeader = clanLeader.Clan?.Kingdom?.Leader;
            if (kingdomLeader == null)
            {
                return false;
            }

            var kingdomLeaderHonor = kingdomLeader.GetHeroTraits().Honor;
            var clanLeaderHonor = clanLeader.GetHeroTraits().Honor;

            var kingdomClanLeaders = Campaign.Current.Clans.Where(w => w.Kingdom?.StringId == clanLeader.Clan.Kingdom?.StringId && w.Kingdom.Leader.StringId != clanLeader.StringId).Select(s => s.Leader).ToList();
            var clanLeaderFriends = kingdomClanLeaders.Where(w => w.IsFriend(clanLeader) && Managers.Character.GetInfo(w.CharacterObject).PlotState == PlotState.IsPlotting).ToList();

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
    }
}