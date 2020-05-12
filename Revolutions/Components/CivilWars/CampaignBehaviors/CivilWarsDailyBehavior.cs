using Revolutions.Components.Base.Characters;
using Revolutions.Components.Base.Clans;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Revolutions.Settings;
using TaleWorlds.Core;
using KNTLibrary.Helpers;
using KNTLibrary.Components.Banners;
using Revolutions.Components.Base.Settlements;

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
            var considerableClans = Campaign.Current.Clans.Where(c => !c.IsUnderMercenaryService && c.Kingdom != null && c.Leader.StringId != c.Kingdom.Leader.StringId);

            foreach (var kingdomWithClans in considerableClans.GroupBy(c => c.Kingdom.StringId, (key, clans) => new { KingdomId = key, Clans = clans.ToList() }))
            {
                if (Managers.CivilWar.GetCivilWarByKingdomId(kingdomWithClans.KingdomId) != null)
                {
                    continue;
                }

                var kingdomInfo = Managers.Kingdom.GetInfo(kingdomWithClans.KingdomId);
                if (kingdomInfo.HasCivilWar || kingdomInfo.Kingdom.Clans.Count < 2)
                {
                    continue;
                }

                #region Check PlotState

                Hero clanLeader;
                CharacterInfo clanLeaderInfo;

                foreach (var clan in kingdomWithClans.Clans)
                {
                    clanLeader = clan.Leader;
                    clanLeaderInfo = Managers.Character.GetInfo(clanLeader.CharacterObject);

                    var relationDifference = clanLeader.GetRelation(kingdomInfo.Kingdom.Leader);

                    if (clanLeaderInfo.PlotState == PlotState.IsPlotting && relationDifference > RevolutionsSettings.Instance.CivilWarsPositiveRelationshipTreshold)
                    {
                        clanLeaderInfo.PlotState = PlotState.IsLoyal;

                        var clanLeaderPlottingFriends = kingdomWithClans.Clans.Select(go => go.Leader)
                                                                              .Where(go => go.IsFriend(clanLeader))
                                                                              .Select(go => Managers.Character.GetInfo(go.CharacterObject))
                                                                              .Where(info => info.PlotState == PlotState.IsPlotting)
                                                                              .ToList();

                        foreach (var friend in clanLeaderPlottingFriends)
                        {
                            if (!this.WillPlotting(friend.Character.HeroObject))
                            {
                                friend.PlotState = PlotState.IsLoyal;
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

                    clanLeader = null;
                    clanLeaderInfo = null;
                }

                #endregion

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
                    continue;
                }

                var loyalTroopWeight = 0f;
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

                float personalityTraits = plottingLeaderGenerosity + kingdomLeaderGenerosity + plottingLeaderMercy + kingdomLeaderMercy + 0f;
                float personalityWeight = MathF.Pow(RevolutionsSettings.Instance.CivilWarsWarPersonalityMultiplier, -personalityTraits + 0f);
                float troopWeight = (plottersTroopWeight + 0f) / (loyalTroopWeight + 0f);
                float valorModifier = 1f + (plottingLeaderValor + 0f <= 0f ? 1f : plottingLeaderValor + 0f * 2f);
                float clanCountModifier = (plottingClans.Count + 0f) / (loyalClans.Count + 0f);
                float calculatingModifier = 1f + (plottingLeaderCalculating + 0f <= 0f ? 1f : plottingLeaderCalculating + 0f);

                var warChance = RevolutionsSettings.Instance.CivilWarsWarBaseChance * personalityWeight * (troopWeight * valorModifier) * MathF.Pow(clanCountModifier, calculatingModifier);
                if (warChance > new Random().Next(0, 100))
                {
                    var settlementInfo = Managers.Settlement.GetInfo(plotLeadingClan.HomeSettlement);
                    var bannerInfo = Managers.Banner.GetRevolutionsBannerBySettlementInfo(settlementInfo);
                    if (bannerInfo != null)
                    {
                        bannerInfo.Used = true;
                    }

                    var plotKingdom = Managers.Kingdom.CreateKingdom(plotLeadingClan.Leader, new TextObject($"Kingdom of {plottingLeader.Clan.Name}"), new TextObject($"Kingdom of {plottingLeader.Clan.Name}"), bannerInfo != null ? new Banner(bannerInfo.BannerId) : null, false);

                    InformationManager.DisplayMessage(new InformationMessage($"A Civil War started in {kingdomInfo.Kingdom.Name}! It's lead by the mighty {plotLeadingClan.Leader.Name} of {plotLeadingClan.Name}.", ColorHelper.RoyalRed));

                    var otherPlottingClans = plottingClans.Where(go => go.StringId != plotLeadingClan.StringId);
                    if (otherPlottingClans.Count() == 0)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Seems like {plotLeadingClan.Leader.Name} of {plotLeadingClan.Name} will be on his own.", ColorHelper.Orange));
                    }

                    foreach (var plottingClan in otherPlottingClans)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"{plottingClan.Leader.Name} of {plottingClan.Name} will be with the plotting leader!", ColorHelper.Orange));
                        ChangeKingdomAction.ApplyByJoinToKingdom(plottingClan, plotKingdom, false);
                    }

                    if (RevolutionsSettings.Instance.CivilWarsKeepExistingWars)
                    {
                        var existingEnemyKingdoms = FactionManager.GetEnemyKingdoms(kingdomInfo.Kingdom);
                        foreach (var enemyKingdom in existingEnemyKingdoms)
                        {
                            DeclareWarAction.Apply(plotKingdom, enemyKingdom);
                        }
                    }

                    DeclareWarAction.Apply(kingdomInfo.Kingdom, plotKingdom);

                    Managers.CivilWar.CivilWars.Add(new CivilWar(kingdomInfo.Kingdom, kingdomWithClans.Clans));
                    kingdomInfo.HasCivilWar = true;
                }
            }

            #endregion
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
            var clanLeaderFriends = kingdomClanLeaders.Where(w => w.IsFriend(clanLeader) && Managers.Character.GetInfo(w.CharacterObject).PlotState == PlotState.IsPlotting).ToList();

            float personalityTraits = kingdomLeaderHonor + clanLeaderHonor + 0f;
            float personalityWeight = MathF.Pow(RevolutionsSettings.Instance.CivilWarsPlottingPersonalityMultiplier, -personalityTraits);
            float friendModifier = clanLeaderFriends.Count + 0f <= 0 ? 1f : clanLeaderFriends.Count + 0f;
            float friendWeight = MathF.Pow(RevolutionsSettings.Instance.CivilWarsPlottingBaseChance, friendModifier);

            var plotChance = RevolutionsSettings.Instance.CivilWarsPlottingBaseChance * personalityWeight * friendWeight;
            return plotChance > new Random().Next(0, 100);
        }
    }
}