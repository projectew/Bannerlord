using System;
using System.Collections.Generic;
using System.Linq;
using Revolutions.Module.Components.Characters;
using Revolutions.Module.Components.Revolts.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Revolutions.Module
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

            if (!strings.Any() || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolutions.show_loyalty_of [Settlement Name]\"";
            }

            var settlementName = strings.Aggregate((i, j) => i + " " + j);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => string.Equals(s.Name.ToString(), settlementName, StringComparison.CurrentCultureIgnoreCase));
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

            if (!strings.Any() || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolts.start [Settlement Name]\"";
            }

            var settlementName = strings.Aggregate((i, j) => i + " " + j);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => string.Equals(s.Name.ToString(), settlementName, StringComparison.CurrentCultureIgnoreCase));
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

            if (strings.Count < 2 || !strings.Contains("-s") || !strings.Contains("-w") || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolts.end -s [Settlement Name] -w [Win (true|false)]\"";
            }

            var aggregatedString = strings.Aggregate((i, j) => i + " " + j);
            const int settlementNameIndex = 3;
            var winIndex = aggregatedString.IndexOf("-w ", StringComparison.Ordinal) + 3;

            var settlementName = aggregatedString.Substring(settlementNameIndex, winIndex - 7);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => string.Equals(s.Name.ToString(), settlementName, StringComparison.CurrentCultureIgnoreCase));
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var revolt = Managers.Revolt.GetRevoltBySettlement(settlement);
            if (revolt == null)
            {
                return $"{settlementName} is not conflicted in a revolt.";
            }

            if (!bool.TryParse(aggregatedString.Substring(winIndex, aggregatedString.Length - winIndex), out var isWin))
            {
                return "Format is \"Revolts.end_Revolt -s [Settlement Name] -w [Win (true|false)]\".";
            }

            if (isWin)
            {
                RevoltBehavior.EndSucceededRevolt(revolt);
            }
            else
            {
                RevoltBehavior.EndFailedRevolt(revolt);
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

            if (strings.Any() || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"revolts.show_lucky_nations\"";
            }

            var luckyNations = Managers.Kingdom.Infos.Where(i => i.LuckyNation).Select(info => info.Kingdom.Name.ToString()).ToList();
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

            if (!strings.Any() || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"civilwars.show_plotting_state_of [Kingdom Name]\"";
            }

            var kingdomName = strings.Aggregate((i, j) => i + " " + j);

            var kingdom = Campaign.Current.Kingdoms.FirstOrDefault(s => string.Equals(s.Name.ToString(), kingdomName, StringComparison.CurrentCultureIgnoreCase));
            if (kingdom == null)
            {
                return $"There is no Kingdom \"{kingdomName}\".";
            }

            var clanLeaders = (from clan in kingdom.Clans.Where(c => c.StringId != kingdom.Leader.StringId) select Managers.Character.Get(clan.Leader.CharacterObject) into clanLeader where clanLeader != null select $"{clanLeader.Character.Name} | {Enum.GetName(typeof(PlotState), clanLeader.PlotState)}").ToList();
            return $"{clanLeaders.Aggregate((i, j) => i + Environment.NewLine + j)}";
        }

        #endregion
    }
}