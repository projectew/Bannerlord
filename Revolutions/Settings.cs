using MBOptionScreen.Attributes;
using MBOptionScreen.Attributes.v2;
using MBOptionScreen.Settings;

namespace Revolts
{
    public class Settings : AttributeSettings<Settings>
    {
        public override string Id { get; set; } = "RevoltsSettings_v050";

        public override string ModuleFolderName => "Revolts";

        public override string ModName => "Revolts";

        #region General

        [SettingPropertyGroup("General", order: 0)]
        [SettingPropertyFloatingInteger(displayName: "Base Player Loyalty", minValue: 0f, maxValue: 100f, Order = 0, HintText = "The base loyalty of cities to the player.", RequireRestart = false)]
        public float GeneralBasePlayerLoyalty { get; set; } = 5.0f;

        [SettingPropertyGroup("General", order: 0)]
        [SettingPropertyInteger(displayName: "Loyalty Change", minValue: 0, maxValue: 365, Order = 1, HintText = "Days until the city's loyalty to the new owner changes.", RequireRestart = false)]
        public int GeneralDaysUntilLoyaltyChange { get; set; } = 90;

        [SettingPropertyGroup("General", order: 0)]
        [SettingPropertyInteger(displayName: "Loyalty Increase", minValue: 0, maxValue: 100, Order = 2, HintText = "The amount by which loyalty increases when the owner is in town.", RequireRestart = false)]
        public int GeneralPlayerInTownLoyaltyIncrease { get; set; } = 5;

        [SettingPropertyGroup("General", order: 0)]
        [SettingPropertyInteger(displayName: "Minimum Obedience", minValue: 0, maxValue: 250, Order = 3, HintText = "Minimal loyalty is required for the city's obedience to the owner.", RequireRestart = false)]
        public int GeneralMinimumObedienceLoyalty { get; set; } = 25;

        #endregion

        #region Revolts

        [SettingPropertyGroup(groupName: "Revolts", order: 1, isMainToggle: true)]
        [SettingPropertyBool(displayName: "Revolts", Order = 0, HintText = "Enables/Disables Revolts.", RequireRestart = false)]
        public bool EnableRevolts { get; set; } = true;

        #region General

        [SettingPropertyGroup(groupName: "Revolts", order: 1)]
        [SettingPropertyFloatingInteger(displayName: "Revolt Cooldown", minValue: 0f, maxValue: 250f, Order = 1, HintText = "The time before another revolt can arise in the same faction.", RequireRestart = false)]
        public float RevoltsGeneralCooldownTime { get; set; } = 30.0f;

        [SettingPropertyGroup("Revolts", order: 1)]
        [SettingPropertyInteger(displayName: "Base Army", minValue: 0, maxValue: 1000, Order = 2, HintText = "The base size of the revolting army, which gets spawned.", RequireRestart = false)]
        public int RevoltsGeneralBaseArmy { get; set; } = 100;

        [SettingPropertyGroup("Revolts", order: 1)]
        [SettingPropertyFloatingInteger(displayName: "Army Prosperity Multiplier", minValue: 0f, maxValue: 100f, Order = 3, HintText = "This multiplier gets multiplied by the towns prosperity to add the amount to the base army size.", RequireRestart = false)]
        public float RevoltsGeneralArmyProsperityMulitplier { get; set; } = 0.1f;

        #endregion

        #region Imperial Loyalty

        [SettingPropertyGroup(groupName: "Revolts", order: 1)]
        [SettingPropertyBool(displayName: "::::: Imperial Loyalty :::::", Order = 4, HintText = "Enables/Disables the Revolts mechanic for Imperial Loyalty.", RequireRestart = false)]
        public bool RevoltsImperialLoyaltyMechanic { get; set; } = true;

        [SettingPropertyGroup(groupName: "Revolts", order: 1)]
        [SettingPropertyInteger(displayName: "Renown Loss", minValue: 0, maxValue: 1000, Order = 5, HintText = "The amount of renown a imperial clan will loose after a revolt against them had success.", RequireRestart = false)]
        public int RevoltsImperialRenownLoss { get; set; } = 50;

        #endregion

        #region Overextension

        [SettingPropertyGroup(groupName: "Revolts", order: 1)]
        [SettingPropertyBool(displayName: "::::: Overextension :::::", Order = 6, HintText = "Enables/Disables the Revolutions mechanics for Overextension.", RequireRestart = false)]
        public bool RevoltsOverextensionMechanics { get; set; } = true;

        [SettingPropertyGroup(groupName: "Revolts", order: 1)]
        [SettingPropertyBool(displayName: "Affects Player", Order = 7, HintText = "Does the mechanic affects the player as well.", RequireRestart = false)]
        public bool RevoltsOverextensionAffectsPlayer { get; set; } = true;

        [SettingPropertyGroup(groupName: "Revolts", order: 1)]
        [SettingPropertyFloatingInteger(displayName: "Multiplier", minValue: 0f, maxValue: 10f, Order = 8, HintText = "A multiplier to calculate with.", RequireRestart = false)]
        public float RevoltsOverextensionMultiplier { get; set; } = 2.0f;

        #endregion

        #region Minor Factions

        [SettingPropertyGroup(groupName: "Revolts", order: 1)]
        [SettingPropertyBool(displayName: "::::: Minor Factions :::::", Order = 9, HintText = "Enables/Disables the Revolutions mechanics Minor Factions.", RequireRestart = false)]
        public bool RevoltsMinorFactionsMechanic { get; set; } = true;

        [SettingPropertyGroup(groupName: "Revolts", order: 1)]
        [SettingPropertyInteger(displayName: "Renown Gain", minValue: 0, maxValue: 1000, Order = 10, HintText = "The amount of renown a minor faction will get after they successful revolted.", RequireRestart = false)]
        public int RevoltsMinorFactionsRenownGainOnWin { get; set; } = 350;

        #endregion

        #region Lucky Nation

        [SettingPropertyGroup(groupName: "Revolts", order: 1)]
        [SettingPropertyBool(displayName: "::::: Lucky Nation :::::", Order = 11, HintText = "Enables/Disables the Revolts mechanics for lucky nation.", RequireRestart = false)]
        public bool RevoltsLuckyNationMechanic { get; set; } = true;

        [SettingPropertyGroup(groupName: "Revolts", order: 1)]
        [SettingPropertyBool(displayName: "Random", Order = 12, HintText = "A random lucky nation from all possible kingdoms.", RequireRestart = false)]
        public bool RevoltsLuckyNationRandom { get; set; } = true;

        [SettingPropertyGroup(groupName: "Revolts", order: 1)]
        [SettingPropertyBool(displayName: "Imperial", Order = 13, HintText = "Guarantees an Imperial lucky nation.", RequireRestart = false)]
        public bool RevoltsLuckyNationImperial { get; set; } = true;

        [SettingPropertyGroup(groupName: "Revolts", order: 1)]
        [SettingPropertyBool(displayName: "Non-Imperial", Order = 14, HintText = "Guarantees a  Non-Imperial lucky nation.", RequireRestart = false)]
        public bool RevoltsLuckyNationNonImperial { get; set; } = true;

        #endregion

        #endregion

        #region Civil Wars

        [SettingPropertyGroup(groupName: "Civil Wars", order: 2, isMainToggle: true)]
        [SettingPropertyBool(displayName: "Civil Wars", Order = 0, HintText = "Enables/Disables Civil Wars.", RequireRestart = false)]
        public bool EnableCivilWars { get; set; } = true;

        [SettingPropertyGroup("Civil Wars", order: 0)]
        [SettingPropertyInteger(displayName: "Loyal Relationship Treshold", minValue: -100, maxValue: 100, Order = 1, HintText = "When relationship between the Lord and his King deteriorate above this threshold, he will stop willing to plot.", RequireRestart = false)]
        public int CivilWarsLoyalRelationshipTreshold { get; set; } = 0;

        [SettingPropertyGroup("Civil Wars", order: 0)]
        [SettingPropertyInteger(displayName: "Plotting Relationship Treshold", minValue: -100, maxValue: 100, Order = 2, HintText = "When relationship between the Lord and his King deteriorate below this threshold, there is a chance he will start plotting to overthrow him.", RequireRestart = false)]
        public int CivilWarsPlottingRelationshipTreshold { get; set; } = -25;

        [SettingPropertyGroup(groupName: "Civil Wars", order: 1)]
        [SettingPropertyFloatingInteger(displayName: "Plotting Base Chance", minValue: 1f, maxValue: 50f, Order = 3, HintText = "Base chance to become a plotter before all other factors are taken into account.", RequireRestart = false)]
        public float CivilWarsPlottingBaseChance { get; set; } = 5f;

        [SettingPropertyGroup(groupName: "Civil Wars", order: 1)]
        [SettingPropertyFloatingInteger(displayName: "Plotting Friend Multiplier", minValue: 1f, maxValue: 25f, Order = 4, HintText = "Increases chance of becoming a Plotter based on whether a lord’s friends are also plotting.", RequireRestart = false)]
        public float CivilWarsPlottingFriendMultiplier { get; set; } = 1.05f;

        [SettingPropertyGroup(groupName: "Civil Wars", order: 1)]
        [SettingPropertyFloatingInteger(displayName: "Plotting Personality Multiplier", minValue: 1f, maxValue: 20f, Order = 5, HintText = "Increases chance of becoming a Plotter when the Lord and Vassal are dishonorable.", RequireRestart = false)]
        public float CivilWarsPlottingPersonalityMultiplier { get; set; } = 1.15f;

        [SettingPropertyGroup(groupName: "Civil Wars", order: 1)]
        [SettingPropertyFloatingInteger(displayName: "War Base Chance", minValue: 1f, maxValue: 50f, Order = 6, HintText = "Base chance for the Plot Leader to declare War on liege before all other factors are taken into account.", RequireRestart = false)]
        public float CivilWarsWarBaseChance { get; set; } = 25f;

        [SettingPropertyGroup(groupName: "Civil Wars", order: 1)]
        [SettingPropertyFloatingInteger(displayName: "Plotting Personality Multiplier", minValue: 1f, maxValue: 20f, Order = 7, HintText = "Decreases chance of declaring a war when the Lord and Vassal are generosity and mercy.", RequireRestart = false)]
        public float CivilWarsWarPersonalityMultiplier { get; set; } = 1.15f;

        #endregion

        #region Miscellaneous

        [SettingPropertyGroup(groupName: "Miscellaneous", order: 99)]
        [SettingPropertyBool(displayName: "Debug Mode", Order = 0, HintText = "Enables/Disables the output of detailed information.", RequireRestart = true)]
        public bool DebugMode { get; set; } = false;

        #endregion
    }
}