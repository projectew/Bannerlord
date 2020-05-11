using Revolutions.Components.Base.Characters;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.Components.CivilWars.CampaignBehaviors
{
    public class CivilWarsDailyBehavior : CampaignBehaviorBase
    {
        public CivilWarsDailyBehavior()
        {

        }

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
                    return;
                }

                var kingdomLeader = clan.Kingdom?.Leader;
                if (kingdomLeader == null)
                {
                    return;
                }

                var clanLeaderInfo = RevolutionsManagers.Character.GetInfo(clanLeader.CharacterObject);
                var clanInfo = RevolutionsManagers.Clan.GetInfo(clanLeader.Clan);

                var clanLeadersOfKingdom = Campaign.Current.Clans.Where(w => w.Kingdom.StringId == clanLeader.Clan.Kingdom?.StringId).Select(s => s.Leader).ToList();
                var clanLeaderFriendsInsideKingdom = clanLeadersOfKingdom.Where(w => w.IsFriend(clanLeader)).ToList();

                var relationBetweenClanLeaderAndKingdomLeader = clanLeader.GetRelation(kingdomLeader);

                var relationshipStopPlottingTreshhold = 0; //Config
                if (clanLeaderInfo.PlotState == PlotState.IsPlotting && relationBetweenClanLeaderAndKingdomLeader > relationshipStopPlottingTreshhold)
                {
                    clanLeaderInfo.PlotState = PlotState.Loyal;

                    foreach (var clanLeaderFriendInfo in clanLeaderFriendsInsideKingdom.Select(s => RevolutionsManagers.Character.GetInfo(s.CharacterObject)).Where(w => w.PlotState == PlotState.IsPlotting))
                    {
                        var clanLeaderFriend = clanLeaderFriendInfo.Character.HeroObject;
                        var friendWillPlotting = this.WillPlotting(clanLeaderFriend);
                        if (!friendWillPlotting)
                        {
                            RevolutionsManagers.Character.GetInfo(clanLeaderFriend.CharacterObject).PlotState = PlotState.Loyal;
                        }
                    }

                    continue;
                }

                var relationshipStartPlottingTreshhold = -25; //Config
                if (relationBetweenClanLeaderAndKingdomLeader > relationshipStartPlottingTreshhold)
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
                if(clanLeaderInfo.PlotState != PlotState.WillPlotting)
                {
                    continue;
                }

                clanLeaderInfo.PlotState = PlotState.IsPlotting;
            }

            foreach (var kingdom in Campaign.Current.Kingdoms)
            {
                var clanLeadersOfKingdom = Campaign.Current.Clans.Where(w => w.Kingdom?.StringId == kingdom.StringId).Select(s => s.Leader).ToList();
                //PlotLeader = Highest Clan Tier

                //Check if plotting clan will declare war
                foreach (var clanLeader in clanLeadersOfKingdom)
                {
                    var clanLeaderInfo = RevolutionsManagers.Character.GetInfo(clanLeader.CharacterObject);
                    if (clanLeaderInfo.PlotState != PlotState.IsPlotting)
                    {
                        continue;
                    }

                    var kingdomLeader = clanLeader.Clan?.Kingdom?.Leader;
                    if (kingdomLeader == null)
                    {
                        continue;
                    }

                    var kingdomLeaderGenerosity = kingdomLeader.GetHeroTraits().Generosity;
                    var kingdomLeaderMercy = kingdomLeader.GetHeroTraits().Mercy;
                    var kingdomLeaderValor = kingdomLeader.GetHeroTraits().Valor;
                    var kingdomLeaderCalculating = kingdomLeader.GetHeroTraits().Calculating;

                    var clanLeaderGenerosity = clanLeader.GetHeroTraits().Generosity;
                    var clanLeaderMercy = clanLeader.GetHeroTraits().Mercy;
                    var clanLeaderValor = clanLeader.GetHeroTraits().Valor;
                    var clanLeaderCalculating = clanLeader.GetHeroTraits().Calculating;
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

            var kingdomClanLeaders = Campaign.Current.Clans.Where(w => w.Kingdom.StringId == clanLeader.Clan.Kingdom?.StringId).Select(s => s.Leader).ToList();
            var clanLeaderFriends = kingdomClanLeaders.Where(w => w.IsFriend(clanLeader)).ToList();

            var basePlottingChance = 2; //Config
            var baseFriendWeight = 1.04; //Config

            return new Random().Next(0, 100) > Math.Pow(basePlottingChance, -(kingdomLeaderHonor + clanLeaderHonor)) * Math.Pow(baseFriendWeight, clanLeaderFriends.Count);
        }
    }
}