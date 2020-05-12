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
            var clansWithoutKing = Campaign.Current.Clans.Where(w => w.Leader.StringId != w.Kingdom?.Leader.StringId);

            //Check if clans will be plotting
            foreach (var clan in clansWithoutKing)
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
                var kingdomInfo = Managers.Kingdom.GetInfo(clan.Kingdom);
                var civilWar = Managers.CivilWar.GetCivilWarByClan(clan);
                if (kingdomInfo.HasCivilWar || civilWar != null || kingdomInfo.Kingdom.Clans.Count < 2)
                {
                    continue;
                }

                var clanLeaderInfo = Managers.Character.GetInfo(clanLeader.CharacterObject);
                var clanInfo = Managers.Clan.GetInfo(clanLeader.Clan);

                var clanLeadersOfKingdom = Campaign.Current.Clans.Where(w => w.Kingdom?.StringId == clanLeader.Clan.Kingdom?.StringId && kingdomLeader.StringId != clanLeader.StringId).Select(s => s.Leader).ToList();
                var clanLeaderFriendsInsideKingdom = clanLeadersOfKingdom.Where(w => w.IsFriend(clanLeader)).ToList();

                var relationBetweenClanLeaderAndKingdomLeader = clanLeader.GetRelation(kingdomLeader);

                if (clanLeaderInfo.PlotState == PlotState.IsPlotting && relationBetweenClanLeaderAndKingdomLeader > RevolutionsSettings.Instance.CivilWarsPositiveRelationshipTreshold)
                {
                    clanLeaderInfo.PlotState = PlotState.IsLoyal;

                    foreach (var clanLeaderFriendInfo in clanLeaderFriendsInsideKingdom.Select(s => Managers.Character.GetInfo(s.CharacterObject)).Where(w => w.PlotState == PlotState.IsPlotting))
                    {
                        var clanLeaderFriend = clanLeaderFriendInfo.Character.HeroObject;
                        var friendWillPlotting = this.WillPlotting(clanLeaderFriend);
                        if (!friendWillPlotting)
                        {
                            Managers.Character.GetInfo(clanLeaderFriend.CharacterObject).PlotState = PlotState.IsLoyal;
                        }
                    }

                    continue;
                }

                if (relationBetweenClanLeaderAndKingdomLeader > RevolutionsSettings.Instance.CivilWarsNegativeRelationshipTreshold)
                {
                    continue;
                }

                if (this.WillPlotting(clanLeader))
                {
                    Managers.Character.GetInfo(clanLeader.CharacterObject).PlotState = PlotState.WillPlotting;
                }
            }

            //Check clans who are willed to plot and set IsPlotting
            foreach (var clan in clansWithoutKing)
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

                var kingdomInfo = Managers.Kingdom.GetInfo(clan.Kingdom);
                var civilWar = Managers.CivilWar.GetCivilWarByClan(clan);
                if (kingdomInfo.HasCivilWar || civilWar != null || kingdomInfo.Kingdom.Clans.Count < 2)
                {
                    continue;
                }

                var clanLeaderInfo = Managers.Character.GetInfo(clan.Leader.CharacterObject);
                if (clanLeaderInfo.PlotState != PlotState.WillPlotting)
                {
                    continue;
                }

                clanLeaderInfo.PlotState = PlotState.IsPlotting;
            }

            //Check if there will be a war for each kingdom
            foreach (var kingdom in Campaign.Current.Kingdoms)
            {
                var kingdomInfo = Managers.Kingdom.GetInfo(kingdom);
                var civilWar = Managers.CivilWar.GetCivilWarByKingdom(kingdom);
                if (kingdomInfo.HasCivilWar || civilWar != null || kingdomInfo.Kingdom.Clans.Count < 2)
                {
                    continue;
                }

                var kingdomClans = Campaign.Current.Clans.Where(w => w.Kingdom?.StringId == kingdom.StringId).ToList();
                var kingdomLoyalClans = new List<ClanInfo>();
                var kingdomPlottingClans = new List<ClanInfo>();
                foreach (var clan in kingdomClans)
                {
                    var clanInfo = Managers.Clan.GetInfo(clan);
                    var characterInfo = Managers.Character.GetInfo(clan.Leader.CharacterObject);

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

                float personalityTraits = plottingLeaderGenerosity + kingdomLeaderGenerosity + plottingLeaderMercy + kingdomLeaderMercy + 0f;
                float personalityWeight = MathF.Pow(RevolutionsSettings.Instance.CivilWarsWarPersonalityMultiplier, -personalityTraits + 0f);
                float troopWeight = (plottersTroopWeight + 0f) / (loyalTroopWeight + 0f);
                float valorModifier = 1f + (plottingLeaderValor + 0f <= 0f ? 1f : plottingLeaderValor + 0f * 2f);
                float clanCountModifier = (kingdomPlottingClans.Count + 0f) / (kingdomLoyalClans.Count + 0f);
                float calculatingModifier = 1f + (plottingLeaderCalculating + 0f <= 0f ? 1f : plottingLeaderCalculating + 0f);

                var warChance = RevolutionsSettings.Instance.CivilWarsWarBaseChance * personalityWeight * (troopWeight * valorModifier) * MathF.Pow(clanCountModifier, calculatingModifier);
                if (warChance > new Random().Next(0, 100))
                {
                    var settlementInfo = Managers.Settlement.GetInfo(plotLeader.Clan.HomeSettlement);
                    Kingdom newKingdom;

                    var bannerInfo = Managers.Banner.GetRevolutionsBannerBySettlementInfo(settlementInfo);
                    if (bannerInfo != null)
                    {
                        bannerInfo.Used = true;

                        newKingdom = Managers.Kingdom.CreateKingdom(plotLeader.Clan.Leader, new TextObject($"Kingdom of {plottingLeader.Clan.Name}"), new TextObject($"Kingdom of {plottingLeader.Clan.Name}"), new Banner(bannerInfo.BannerId), false);
                    }
                    else
                    {
                        newKingdom = Managers.Kingdom.CreateKingdom(plotLeader.Clan.Leader, new TextObject($"Kingdom of {plottingLeader.Clan.Name}"), new TextObject($"Kingdom of {plottingLeader.Clan.Name}"), null, false);
                    }

                    var plottingClansWithoutLeader = kingdomPlottingClans.Where(w => w.ClanId != plotLeader.ClanId);

                    FactionManager.DeclareWar(newKingdom, kingdom);
                    Campaign.Current.FactionManager.RegisterCampaignWar(newKingdom, kingdom);

                    InformationManager.DisplayMessage(new InformationMessage($"A Civil War started in {kingdom.Name}! It's lead by the mighty {plotLeader.Clan.Leader.Name} of {plotLeader.Clan.Name}.", ColorHelper.RoyalRed));

                    if (plottingClansWithoutLeader.Count() == 0)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Seems like {plotLeader.Clan.Leader.Name} of {plotLeader.Clan.Name} will be on his own.", ColorHelper.Orange));

                    }
                    foreach (var plottingClan in plottingClansWithoutLeader)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"{plottingClan.Clan.Leader.Name} of {plottingClan.Clan.Name} will be with the plotting leader!", ColorHelper.Orange));
                        ChangeKingdomAction.ApplyByJoinToKingdom(plottingClan.Clan, newKingdom, false);
                    }

                    Managers.CivilWar.CivilWars.Add(new CivilWar(kingdom, kingdomClans));
                    kingdomInfo.HasCivilWar = true;
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