namespace Revolutions.Localization.Settings
{
    internal static class Descriptions
    {
        #region General

        internal const string GeneralBasePlayerLoyalty = "{=XLMyVNzj}The base loyalty of cities to the owner.";

        internal const string GeneralDaysUntilLoyaltyChange = "{=rD2UgIHi}Days until the city's loyalty to the new owner changes.";

        internal const string GeneralPlayerInTownLoyaltyIncrease = "{=97WepaH0}The amount by which loyalty increases when the owner is in town.";

        internal const string GeneralMinimumObedienceLoyalty = "{=xxvyiZ31}Needed loyalty which is required for the city's obedience to the owner.";

        #endregion

        #region Revolts

        internal const string EnableRevolts = "{=XgDjEUh5}Enables/Disables Revolts.";

        #region General

        internal const string RevoltsGeneralCooldownTime = "{=6YaMmHjg}The time before another revolt can arise in the same faction.";

        internal const string RevoltsGeneralBaseSize = "{=YPLgcVBr}The base size of the revolting army, which gets spawned.";

        internal const string RevoltsGeneralProsperityMulitplier = "{=ErxSyXbf}This multiplier gets multiplied by the towns prosperity to add the amount to the base army size.";

        #endregion

        #region Imperial Loyalty

        internal const string RevoltsImperialLoyaltyMechanic = "{=eTxuxdut}Enables/Disables the mechanics for Imperial Loyalty.";

        internal const string RevoltsImperialRenownLoss = "{=8Py6iVGm}The amount of renown a imperial clan will loose after a revolt against them had success.";

        #endregion

        #region Overextension

        internal const string RevoltsOverextensionMechanics = "{=aktFvWrc}Enables/Disables the mechanics for Overextension.";

        internal const string RevoltsOverextensionAffectsPlayer = "{=bzcvlOkc}Does the mechanic affects the player as well.";

        internal const string RevoltsOverextensionMultiplier = "{=0tFsxz7g}A multiplier to calculate with.";

        #endregion

        #region Minor Factions

        internal const string RevoltsMinorFactionsMechanic = "{=viKJpnX7}Enables/Disables the mechanics for Minor Factions.";

        internal const string RevoltsMinorFactionsRenownGainOnWin = "{=oimmeOva}The amount of renown a minor faction will get after they successful revolted.";

        #endregion

        #region Lucky Nation

        internal const string RevoltsLuckyNationMechanic = "{=xiDVtZlJ}Enables/Disables the Revolts mechanics for Lucky Nation.";

        internal const string RevoltsLuckyNationRandom = "{=mLaEWxas}A random lucky nation from all possible kingdoms.";

        internal const string RevoltsLuckyNationImperial = "{=V3bJdu4A}Guarantees an Imperial lucky nation.";

        internal const string RevoltsLuckyNationNonImperial = "{=3gOiyj6Z}Guarantees a  Non-Imperial lucky nation.";

        #endregion

        #endregion

        #region Civil Wars

        internal const string EnableCivilWars = "{=AWgtsKpI}Enables/Disables Civil Wars.";

        #region General

        internal const string CivilWarsKeepExistingWars = "{=h0qu9eTf}Civil War factions inherit the wars of their origin kingdom.";

        internal const string CivilWarsPositiveRelationshipTreshold = "{=Z5BHX5SQ}When relationship between the Lord and his King deteriorate above this threshold, he will stop willing to plot.";

        internal const string CivilWarsNegativeRelationshipTreshold = "{=xbvpfSoO}When relationship between the Lord and his King deteriorate below this threshold, there is a chance he will start plotting to overthrow him.";

        internal const string CivilWarsPlottingConsiderTime = "{=h7qICnX7}Time in days before player will be asked again to join the plot or not.";

        #endregion

        #region Plotting

        internal const string CivilWarsPlottingBaseChance = "{=WJnJ6OqO}Base chance to become a plotter before all other factors are taken into account.";

        internal const string CivilWarsPlottingFriendMultiplier = "{=2engz0Rc}Increases chance of becoming a Plotter based on whether a lord’s friends are also plotting.";

        internal const string CivilWarsPlottingPersonalityMultiplier = "{=B7f17Deb}Increases chance of becoming a Plotter when the Lord and Vassal are dishonorable.";

        #endregion

        #region War

        internal const string CivilWarsWarBaseChance = "{=2niRfWx2}Base chance for the Plot Leader to declare War on liege before all other factors are taken into account.";

        internal const string CivilWarsWarPersonalityMultiplier = "{=ZPSeEboV}Decreases chance of declaring a war when the Lord and Vassal are generosity and mercy.";

        internal const string CivilWarsRelationshipChange = "{=saF5Rr3J}The amount by which relations will change (positive and negative) between lords based upon the Civil War faction they choose to be a part of.";

        internal const string CivilWarsRelationshipChangeMultiplier = "{=1DRZeNBE}Multiplier for relations change between lords and faction leaders.";

        #endregion

        #endregion

        #region Miscellaneous

        internal const string DebugMode = "{=O0WtFLpL}Enables/Disables the output of detailed information.";

        #endregion
    }
}