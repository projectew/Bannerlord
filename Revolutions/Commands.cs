using Revolutions.Components.Characters;
using Revolutions.Components.Revolts.CampaignBehaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Revolutions
{
    public static class Commands
    {
        #region Revolutions

        [CommandLineFunctionality.CommandLineArgumentFunction("show_loyalty_of", "revolutions")]
        public static string ShowLoyaltyOf(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.show_loyalty_of [Settlement Name]\"";
            }

            var settlementName = strings.Aggregate((i, j) => i + " " + j);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            if (!settlement.IsTown)
            {
                return $"Settlement \"{settlementName}\" is not a town.";
            }

            var settlementInfo = Managers.Settlement.Get(settlement);
            return $"{settlement.Name} is loyal to {settlementInfo.LoyalFaction.Name}.";
        }

        #endregion

        #region Revolts

        [CommandLineFunctionality.CommandLineArgumentFunction("start", "revolts")]
        public static string StartRevolt(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolts.start [Settlement Name]\"";
            }

            var settlementName = strings.Aggregate((i, j) => i + " " + j);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var settlementInfo = Managers.Settlement.Get(settlement);
            if (settlementInfo.HasRebellionEvent)
            {
                return $"{settlement.Name} has an running revolt.";
            }

            RevoltBehavior.StartRevolt(settlement);
            return $"Started a Revolt in {settlement.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("end", "revolts")]
        public static string EndRevolt(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 2 || !strings.Contains("-s") || !strings.Contains("-w") || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolts.end -s [Settlement Name] -w [Win (true|false)]\"";
            }

            var aggregatedString = strings.Aggregate((i, j) => i + " " + j);
            var settlementNameIndex = 3;
            var winIndex = aggregatedString.IndexOf("-w ") + 3;

            var settlementName = aggregatedString.Substring(settlementNameIndex, winIndex - 7);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var Revolt = Managers.Revolt.GetRevoltBySettlement(settlement);
            if (Revolt == null)
            {
                return $"{settlementName} is not conflicted in a revolt.";
            }

            if (!bool.TryParse(aggregatedString.Substring(winIndex, aggregatedString.Length - winIndex), out var isWin))
            {
                return "Format is \"Revolts.end_Revolt -s [Settlement Name] -w [Win (true|false)]\".";
            }

            if (isWin)
            {
                RevoltBehavior.EndSucceededRevolt(Revolt);
            }
            else
            {
                RevoltBehavior.EndFailedRevolt(Revolt);
            }

            return $"Ended a {(isWin ? "successful" : "failed")} Revolt in {settlement.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("show_lucky_nations", "revolts")]
        public static string ShowLuckyNations(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() > 0 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolts.show_lucky_nations\"";
            }

            var luckyNations = new List<string>();

            foreach (var info in Managers.Kingdom.Infos.Where(i => i.LuckyNation))
            {
                luckyNations.Add(info.Kingdom.Name.ToString());
            }

            return luckyNations.Count == 0 ? "There are no Lucky Nations!" : $"{luckyNations.Aggregate((i, j) => i + Environment.NewLine + j)}";
        }

        #endregion

        #region Civil Wars

        [CommandLineFunctionality.CommandLineArgumentFunction("show_plotting_state_of", "civilwars")]
        public static string ShowPlottingStatesOf(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"civilwars.show_plotting_state_of [Kingdom Name]\"";
            }

            var kingdomName = strings.Aggregate((i, j) => i + " " + j);

            var kingdom = Campaign.Current.Kingdoms.FirstOrDefault(s => s.Name.ToString().ToLower() == kingdomName.ToLower());
            if (kingdom == null)
            {
                return $"There is no Kingdom \"{kingdomName}\".";
            }

            var clanLeaders = new List<string>();

            foreach (var clan in kingdom.Clans.Where(c => c.StringId != kingdom.Leader.StringId))
            {
                var clanLeader = Managers.Character.Get(clan.Leader.CharacterObject);
                if (clanLeader == null)
                {
                    continue;
                }

                clanLeaders.Add($"{clanLeader.Character.Name} | {Enum.GetName(typeof(PlotState), clanLeader.PlotState)}");
            }

            return $"{clanLeaders.Aggregate((i, j) => i + Environment.NewLine + j)}";
        }

        #endregion
    }
}