using Revolutions;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Revolts
{
    public static class Commands
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("start_Revolt", "Revolts")]
        public static string StartRevolt(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"Revolts.start_Revolt [Settlement Name]\"";
            }

            var settlementName = strings.Aggregate((i, j) => i + " " + j);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            if (settlement.IsUnderSiege)
            {
                return $"{settlement.Name} is under siege.";
            }

            var settlementInfo = RevoltsManagers.Settlement.GetInfo(settlement);
            settlementInfo.RevoltProgress = 100;

            RevoltsManagers.Revolt.StartRebellionEvent(settlement);

            return $"Started a Revolt in {settlement.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("end_Revolt", "Revolts")]
        public static string EndRevolt(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 2 || !strings.Contains("-s") || !strings.Contains("-w") || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"Revolts.end_Revolt -s [Settlement Name] -w [Win (true|false)]\"";
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

            var Revolt = RevoltsManagers.Revolt.GetRevoltBySettlementId(settlement.StringId);
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
                RevoltsManagers.Revolt.EndSucceededRevoluton(Revolt);
            }
            else
            {
                RevoltsManagers.Revolt.EndFailedRevolt(Revolt);
            }

            return $"Ended a {(isWin ? "successful" : "failed")} Revolt in {settlement.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal_to_player", "Revolts")]
        public static string SetLoyalToPlayer(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"Revolts.set_loyal_to_player [Settlement Name]\"";
            }

            var settlementName = strings.Aggregate((i, j) => i + " " + j);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var settlementInfo = RevoltsManagers.Settlement.GetInfo(settlement);
            settlementInfo.LoyalFactionId = Hero.MainHero.MapFaction.StringId;

            return $"{settlement.Name} is now loyal to {settlementInfo.LoyalFaction.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal_to_current_owner", "Revolts")]
        public static string SetLoyalToCurrentOwner(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"Revolts.set_loyal_to_current_owner [Settlement Name]\"";
            }

            var settlementName = strings.Aggregate((i, j) => i + " " + j);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var settlementInfo = RevoltsManagers.Settlement.GetInfo(settlement);
            settlementInfo.LoyalFactionId = settlementInfo.CurrentFactionId;

            return $"{settlement.Name} is now loyal to {settlementInfo.LoyalFaction.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal_to", "Revolts")]
        public static string SetLoyalTo(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 4 || !strings.Contains("-s") || !strings.Contains("-f") || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"Revolts.set_loyal_owner_to -s [Settlement Name] -f [Faction Name]\"";
            }

            var aggregatedString = strings.Aggregate((i, j) => i + " " + j);
            var settlementNameIndex = 3;
            var factionNameIndex = aggregatedString.IndexOf("-f ") + 3;

            var settlementName = aggregatedString.Substring(settlementNameIndex, factionNameIndex - 7);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var factionName = aggregatedString.Substring(factionNameIndex, aggregatedString.Length - factionNameIndex);

            var faction = Campaign.Current.Factions.FirstOrDefault(s => s.Name.ToString().ToLower() == factionName.ToLower());
            if (settlement == null)
            {
                return $"There is no Faction \"{factionName}\".";
            }

            var settlementInfo = RevoltsManagers.Settlement.GetInfo(settlement);
            settlementInfo.LoyalFactionId = faction.StringId;

            return $"{settlement.Name} is now loyal to {settlementInfo.LoyalFaction.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal_owner_to_player", "Revolts")]
        public static string SetLoyalOwnerToPlayer(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"Revolts.set_loyal_owner_to_player [Settlement Name]\"";
            }

            var settlementName = strings.Aggregate((i, j) => i + j);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var settlementInfo = RevoltsManagers.Settlement.GetInfo(settlement);
            settlementInfo.RevoltProgress = 0;
            settlementInfo.CurrentFactionId = Hero.MainHero.MapFaction.StringId;
            settlementInfo.LoyalFactionId = Hero.MainHero.MapFaction.StringId;
            settlement.OwnerClan = Hero.MainHero.Clan;

            return $"{settlement.Name} is now owned by and loyal to {Hero.MainHero.Clan.Name} ({settlementInfo.LoyalFaction.Name}).";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("set_loyal_owner_to", "Revolts")]
        public static string SetLoyalOwnerTo(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 4 || !strings.Contains("-s") || !strings.Contains("-c") || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"Revolts.set_loyal_owner_to -s [Settlement Name] -c [Clan Name]\"";
            }

            var aggregatedString = strings.Aggregate((i, j) => i + " " + j);
            var settlementNameIndex = 3;
            var clanNameIndex = aggregatedString.IndexOf("-c ") + 3;

            var settlementName = aggregatedString.Substring(settlementNameIndex, clanNameIndex - 7);

            var settlement = Campaign.Current.Settlements.FirstOrDefault(s => s.Name.ToString().ToLower() == settlementName.ToLower());
            if (settlement == null)
            {
                return $"There is no Settlement \"{settlementName}\".";
            }

            var clanName = aggregatedString.Substring(clanNameIndex, aggregatedString.Length - clanNameIndex);

            var clan = Campaign.Current.Clans.FirstOrDefault(s => s.Name.ToString().ToLower() == clanName.ToLower());
            if (clan == null)
            {
                return $"There is no Clan \"{clanName}\".";
            }

            var settlementInfo = RevoltsManagers.Settlement.GetInfo(settlement);
            settlementInfo.RevoltProgress = 0;
            settlementInfo.CurrentFactionId = clan.MapFaction.StringId;
            settlementInfo.LoyalFactionId = clan.MapFaction.StringId;
            settlement.OwnerClan = clan;

            return $"{settlement.Name} is now owned by and loyal to {clan.Name} ({settlementInfo.LoyalFaction.Name}).";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("show_loyalty_of", "Revolts")]
        public static string ShowLoyaltyOf(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() < 1 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"Revolts.show_loyalty_of [Settlement Name]\"";
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

            var settlementInfo = RevoltsManagers.Settlement.GetInfo(settlement);
            return $"{settlement.Name} is loyal to {settlementInfo.LoyalFaction.Name} with a score of {settlement.Town.Loyalty}.";
        }


        [CommandLineFunctionality.CommandLineArgumentFunction("show_lucky_nations", "Revolts")]
        public static string ShowLuckyNations(List<string> strings)
        {
            if (Campaign.Current == null)
            {
                return "Campaign was not started.";
            }

            if (strings.Count() > 0 || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"Revolts.show_lucky_nations\"";
            }

            var luckyNations = new List<string>();

            foreach (var info in RevoltsManagers.Kingdom.Infos.Where(i => i.LuckyNation))
            {
                luckyNations.Add(info.Kingdom.Name.ToString());
            }

            return $"Lucky Nations: {luckyNations.Aggregate((i, j) => i + ", " + j)}";
        }
    }
}