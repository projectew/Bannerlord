using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Settings.Base.Global;
using Revolutions.Module.Settings.Localization;

namespace Revolutions.Module.Settings
{
    public class RevolutionsSettings : AttributeGlobalSettings<RevolutionsSettings>
    {
        public override string Id => $"Revolutions_v{typeof(RevolutionsSettings).Assembly.GetName().Version.ToString(3).Replace(".", "")}";

        public override string FolderName => "Revolutions";

        public override string DisplayName => $"Revolutions {typeof(RevolutionsSettings).Assembly.GetName().Version.ToString(3)}";

        public override string Format => "json";


        #region Revolts

        [SettingPropertyGroup(groupName: Categories.Revolts, GroupOrder = 1, IsMainToggle = true)]
        [SettingPropertyBool(displayName: Categories.Revolts, Order = 0, HintText = Descriptions.EnableRevolts, RequireRestart = true)]
        public bool EnableRevolts { get; set; } = true;

        #region General

        [SettingPropertyGroup(groupName: Groups.RevoltsGeneral, GroupOrder = 1, IsMainToggle = false)]
        [SettingPropertyFloatingInteger(displayName: Names.RevoltsGeneralPlayerBaseLoyalty, minValue: 0f, maxValue: 100f, Order = 1, HintText = Descriptions.RevoltsGeneralPlayerBaseLoyalty, RequireRestart = false)]
        public float RevoltsGeneralPlayerBaseLoyalty { get; set; } = 5.0f;

        [SettingPropertyGroup(groupName: Groups.RevoltsGeneral, GroupOrder = 1, IsMainToggle = false)]
        [SettingPropertyInteger(displayName: Names.RevoltsGeneralOwnerInTownLoyaltyIncrease, minValue: 0, maxValue: 100, Order = 2, HintText = Descriptions.RevoltsGeneralOwnerInTownLoyaltyIncrease, RequireRestart = false)]
        public int RevoltsGeneralOwnerInTownLoyaltyIncrease { get; set; } = 5;

        [SettingPropertyGroup(groupName: Groups.RevoltsGeneral, GroupOrder = 1, IsMainToggle = false)]
        [SettingPropertyInteger(displayName: Names.RevoltsGeneralCooldownTime, minValue: 0, maxValue: 365, Order = 3, HintText = Descriptions.RevoltsGeneralCooldownTime, RequireRestart = false)]
        public int RevoltsGeneralCooldownTime { get; set; } = 30;

        [SettingPropertyGroup(groupName: Groups.RevoltsGeneral, GroupOrder = 1, IsMainToggle = false)]
        [SettingPropertyInteger(displayName: Names.RevoltsGeneralBaseSize, minValue: 0, maxValue: 1000, Order = 4, HintText = Descriptions.RevoltsGeneralBaseSize, RequireRestart = false)]
        public int RevoltsGeneralBaseSize { get; set; } = 100;

        [SettingPropertyGroup(groupName: Groups.RevoltsGeneral, GroupOrder = 1, IsMainToggle = false)]
        [SettingPropertyFloatingInteger(displayName: Names.RevoltsGeneralProsperityMulitplier, minValue: 0f, maxValue: 100f, Order = 5, HintText = Descriptions.RevoltsGeneralProsperityMulitplier, RequireRestart = false)]
        public float RevoltsGeneralProsperityMulitplier { get; set; } = 0.1f;

        #endregion

        #region Imperial Loyalty

        [SettingPropertyGroup(groupName: Groups.RevoltsImperialLoyalty, GroupOrder = 2, IsMainToggle = true)]
        [SettingPropertyBool(displayName: Groups.RevoltsImperialLoyalty, Order = 0, HintText = Descriptions.RevoltsImperialLoyaltyMechanic, RequireRestart = false)]
        public bool RevoltsImperialLoyaltyMechanic { get; set; } = true;

        [SettingPropertyGroup(groupName: Groups.RevoltsImperialLoyalty, GroupOrder = 2, IsMainToggle = false)]
        [SettingPropertyInteger(displayName: Names.RevoltsImperialRenownLoss, minValue: 0, maxValue: 1000, Order = 1, HintText = Descriptions.RevoltsImperialRenownLoss, RequireRestart = false)]
        public int RevoltsImperialRenownLoss { get; set; } = 50;

        #endregion

        #region Overextension

        [SettingPropertyGroup(groupName: Groups.RevoltsOverextension, GroupOrder = 3, IsMainToggle = true)]
        [SettingPropertyBool(displayName: Groups.RevoltsOverextension, Order = 0, HintText = Descriptions.RevoltsOverextensionMechanics, RequireRestart = false)]
        public bool RevoltsOverextensionMechanics { get; set; } = true;

        [SettingPropertyGroup(groupName: Groups.RevoltsOverextension, GroupOrder = 3, IsMainToggle = false)]
        [SettingPropertyBool(displayName: Names.RevoltsOverextensionAffectsPlayer, Order = 1, HintText = Descriptions.RevoltsOverextensionAffectsPlayer, RequireRestart = false)]
        public bool RevoltsOverextensionAffectsPlayer { get; set; } = true;

        [SettingPropertyGroup(groupName: Groups.RevoltsOverextension, GroupOrder = 3, IsMainToggle = false)]
        [SettingPropertyFloatingInteger(displayName: Names.RevoltsOverextensionMultiplier, minValue: 0f, maxValue: 10f, Order = 2, HintText = Descriptions.RevoltsOverextensionMultiplier, RequireRestart = false)]
        public float RevoltsOverextensionMultiplier { get; set; } = 2.0f;

        #endregion

        #region Minor Factions

        [SettingPropertyGroup(groupName: Groups.RevoltsMinorFactions, GroupOrder = 4, IsMainToggle = true)]
        [SettingPropertyBool(displayName: Groups.RevoltsMinorFactions, Order = 0, HintText = Descriptions.RevoltsMinorFactionsMechanic, RequireRestart = false)]
        public bool RevoltsMinorFactionsMechanic { get; set; } = true;

        [SettingPropertyGroup(groupName: Groups.RevoltsMinorFactions, GroupOrder = 4, IsMainToggle = false)]
        [SettingPropertyInteger(displayName: Names.RevoltsMinorFactionsRenownGainOnWin, minValue: 0, maxValue: 1000, Order = 1, HintText = Descriptions.RevoltsMinorFactionsRenownGainOnWin, RequireRestart = false)]
        public int RevoltsMinorFactionsRenownGainOnWin { get; set; } = 350;

        #endregion

        #region Lucky Nation

        [SettingPropertyGroup(groupName: Groups.RevoltsLuckyNation, GroupOrder = 5, IsMainToggle = true)]
        [SettingPropertyBool(displayName: Groups.RevoltsLuckyNation, Order = 0, HintText = Descriptions.RevoltsLuckyNationMechanic, RequireRestart = false)]
        public bool RevoltsLuckyNationMechanic { get; set; } = true;

        [SettingPropertyGroup(groupName: Groups.RevoltsLuckyNation, GroupOrder = 5)]
        [SettingPropertyBool(displayName: Names.RevoltsLuckyNationRandom, Order = 1, HintText = Descriptions.RevoltsLuckyNationRandom, RequireRestart = false)]
        public bool RevoltsLuckyNationRandom { get; set; } = true;

        [SettingPropertyGroup(groupName: Groups.RevoltsLuckyNation, GroupOrder = 5, IsMainToggle = false)]
        [SettingPropertyBool(displayName: Names.RevoltsLuckyNationImperial, Order = 2, HintText = Descriptions.RevoltsLuckyNationImperial, RequireRestart = false)]
        public bool RevoltsLuckyNationImperial { get; set; } = true;

        [SettingPropertyGroup(groupName: Groups.RevoltsLuckyNation, GroupOrder = 5, IsMainToggle = false)]
        [SettingPropertyBool(displayName: Names.RevoltsLuckyNationNonImperial, Order = 3, HintText = Descriptions.RevoltsLuckyNationNonImperial, RequireRestart = false)]
        public bool RevoltsLuckyNationNonImperial { get; set; } = true;

        #endregion

        #endregion

        #region Civil Wars

        [SettingPropertyGroup(groupName: Categories.CivilWars, GroupOrder = 6, IsMainToggle = true)]
        [SettingPropertyBool(displayName: Categories.CivilWars, Order = 0, HintText = Descriptions.EnableCivilWars, RequireRestart = true)]
        public bool EnableCivilWars { get; set; } = true;

        #region General

        [SettingPropertyGroup(groupName: Groups.CivilWarsGeneral, GroupOrder = 7, IsMainToggle = false)]
        [SettingPropertyBool(displayName: Names.CivilWarsKeepExistingWars, Order = 0, HintText = Descriptions.CivilWarsKeepExistingWars, RequireRestart = false)]
        public bool CivilWarsKeepExistingWars { get; set; } = false;

        [SettingPropertyGroup(groupName: Groups.CivilWarsGeneral, GroupOrder = 7, IsMainToggle = false)]
        [SettingPropertyInteger(displayName: Names.CivilWarsPositiveRelationshipTreshold, minValue: -100, maxValue: 100, Order = 1, HintText = Descriptions.CivilWarsPositiveRelationshipTreshold, RequireRestart = false)]
        public int CivilWarsPositiveRelationshipTreshold { get; set; } = 10;

        [SettingPropertyGroup(groupName: Groups.CivilWarsGeneral, GroupOrder = 7, IsMainToggle = false)]
        [SettingPropertyInteger(displayName: Names.CivilWarsNegativeRelationshipTreshold, minValue: -100, maxValue: 100, Order = 2, HintText = Descriptions.CivilWarsNegativeRelationshipTreshold, RequireRestart = false)]
        public int CivilWarsNegativeRelationshipTreshold { get; set; } = -10;

        [SettingPropertyGroup(groupName: Groups.CivilWarsGeneral, GroupOrder = 7, IsMainToggle = false)]
        [SettingPropertyInteger(displayName: Names.CivilWarsPlottingConsiderTime, minValue: 1, maxValue: 365, Order = 3, HintText = Descriptions.CivilWarsPlottingConsiderTime, RequireRestart = false)]
        public int CivilWarsPlottingConsiderTime { get; set; } = 7;

        #endregion

        #region Plotting

        [SettingPropertyGroup(groupName: Groups.CivilWarsPlotting, GroupOrder = 8, IsMainToggle = false)]
        [SettingPropertyFloatingInteger(displayName: Names.CivilWarsPlottingBaseChance, minValue: 1f, maxValue: 100f, Order = 0, HintText = Descriptions.CivilWarsPlottingBaseChance, RequireRestart = false)]
        public float CivilWarsPlottingBaseChance { get; set; } = 5f;

        [SettingPropertyGroup(groupName: Groups.CivilWarsPlotting, GroupOrder = 8, IsMainToggle = false)]
        [SettingPropertyFloatingInteger(displayName: Names.CivilWarsPlottingFriendMultiplier, minValue: 1f, maxValue: 3f, Order = 1, HintText = Descriptions.CivilWarsPlottingFriendMultiplier, RequireRestart = false)]
        public float CivilWarsPlottingFriendMultiplier { get; set; } = 2f;

        [SettingPropertyGroup(groupName: Groups.CivilWarsPlotting, GroupOrder = 8, IsMainToggle = false)]
        [SettingPropertyFloatingInteger(displayName: Names.CivilWarsPlottingPersonalityMultiplier, minValue: 1f, maxValue: 3f, Order = 2, HintText = Descriptions.CivilWarsPlottingPersonalityMultiplier, RequireRestart = false)]
        public float CivilWarsPlottingPersonalityMultiplier { get; set; } = 1.15f;

        #endregion

        #region War

        [SettingPropertyGroup(groupName: Groups.CivilWarsWar, GroupOrder = 9, IsMainToggle = false)]
        [SettingPropertyFloatingInteger(displayName: Names.CivilWarsWarBaseChance, minValue: 1f, maxValue: 100f, Order = 0, HintText = Descriptions.CivilWarsWarBaseChance, RequireRestart = false)]
        public float CivilWarsWarBaseChance { get; set; } = 10f;

        [SettingPropertyGroup(groupName: Groups.CivilWarsWar, GroupOrder = 9, IsMainToggle = false)]
        [SettingPropertyFloatingInteger(displayName: Names.CivilWarsWarPersonalityMultiplier, minValue: 1f, maxValue: 3f, Order = 1, HintText = Descriptions.CivilWarsWarPersonalityMultiplier, RequireRestart = false)]
        public float CivilWarsWarPersonalityMultiplier { get; set; } = 1.15f;

        [SettingPropertyGroup(groupName: Groups.CivilWarsGeneral, GroupOrder = 9, IsMainToggle = false)]
        [SettingPropertyInteger(displayName: Names.CivilWarsRelationshipChange, minValue: 0, maxValue: 25, Order = 2, HintText = Descriptions.CivilWarsRelationshipChange, RequireRestart = false)]
        public int CivilWarsRelationshipChange { get; set; } = 10;

        [SettingPropertyGroup(groupName: Groups.CivilWarsGeneral, GroupOrder = 9, IsMainToggle = false)]
        [SettingPropertyFloatingInteger(displayName: Names.CivilWarsRelationshipChangeMultiplier, minValue: 1f, maxValue: 3f, Order = 3, HintText = Descriptions.CivilWarsRelationshipChangeMultiplier, RequireRestart = false)]
        public float CivilWarsRelationshipChangeMultiplier { get; set; } = 1.5f;

        #endregion

        #endregion

        #region Miscellaneous

        [SettingPropertyGroup(groupName: Categories.Miscellaneous, GroupOrder = 99, IsMainToggle = false)]
        [SettingPropertyBool(displayName: Names.DebugMode, Order = 0, HintText = Descriptions.DebugMode, RequireRestart = true)]
        public bool DebugMode { get; set; } = false;

        #endregion
    }
}