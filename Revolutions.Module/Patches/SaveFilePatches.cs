using System;
using HarmonyLib;
using Revolutions.Library.Helpers;
using Revolutions.Module.Settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.Module.Patches
{
    internal static class SaveFilePatches
    {
        [HarmonyPatch(typeof(MBSaveLoad), "OnNewGame")]
        internal static class OnNewGame
        {
            internal static void Postfix()
            {
                try
                {
                    DataStorage.ActiveSaveSlotName = string.Empty;
                }
                catch (Exception exception)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Revolutions: Failed at OnNewGame.", ColorHelper.Red));

                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Exception: {exception.Message}", ColorHelper.Red));
                        InformationManager.DisplayMessage(new InformationMessage($"StackTrace: {exception.StackTrace}", ColorHelper.Red));
                    }
                }
            }
        }

        [HarmonyPatch(typeof(MBSaveLoad), "LoadSaveGameData")]
        internal static class LoadSaveGameData
        {
            internal static void Postfix()
            {
                try
                {
                    DataStorage.ActiveSaveSlotName = AccessTools.Field(typeof(MBSaveLoad), "ActiveSaveSlotName").GetValue(null).ToString();
                }
                catch (Exception exception)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Revolutions: Failed at LoadSaveGameData for '{DataStorage.ActiveSaveSlotName ?? "Null"}'", ColorHelper.Red));

                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Exception: {exception.Message}", ColorHelper.Red));
                        InformationManager.DisplayMessage(new InformationMessage($"StackTrace: {exception.StackTrace}", ColorHelper.Red));
                    }
                }
            }
        }

        [HarmonyPatch(typeof(MBSaveLoad), "SaveAsCurrentGame")]
        internal static class SaveAsCurrentGame
        {
            internal static void Postfix()
            {
                try
                {
                    DataStorage.ActiveSaveSlotName = AccessTools.Field(typeof(MBSaveLoad), "ActiveSaveSlotName")?.GetValue(null)?.ToString();
                    DataStorage.SaveData();
                }
                catch (Exception exception)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Revolutions: Failed at SaveAsCurrentGame for '{DataStorage.ActiveSaveSlotName ?? "Null"}'", ColorHelper.Red));

                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Exception: {exception.Message}", ColorHelper.Red));
                        InformationManager.DisplayMessage(new InformationMessage($"StackTrace: {exception.StackTrace}", ColorHelper.Red));
                    }
                }
            }
        }

        [HarmonyPatch(typeof(MBSaveLoad), "QuickSaveCurrentGame")]
        internal static class QuickSaveCurrentGame
        {
            internal static void Postfix()
            {
                try
                {
                    DataStorage.ActiveSaveSlotName = AccessTools.Field(typeof(MBSaveLoad), "ActiveSaveSlotName").GetValue(null).ToString();
                    DataStorage.SaveData();
                }
                catch (Exception exception)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Revolutions: Failed at QuickSaveCurrentGame for '{DataStorage.ActiveSaveSlotName ?? "Null"}'", ColorHelper.Red));

                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Exception: {exception.Message}", ColorHelper.Red));
                        InformationManager.DisplayMessage(new InformationMessage($"StackTrace: {exception.StackTrace}", ColorHelper.Red));
                    }
                }
            }
        }

        [HarmonyPatch(typeof(MBSaveLoad), "AutoSaveCurrentGame")]
        internal static class AutoSaveCurrentGame
        {
            internal static void Postfix()
            {
                try
                {
                    DataStorage.ActiveSaveSlotName = $"{AccessTools.Field(typeof(MBSaveLoad), "AutoSaveNamePrefix").GetValue(null)}{AccessTools.Field(typeof(MBSaveLoad), "AutoSaveIndex").GetValue(null)}";
                    DataStorage.SaveData();
                }
                catch (Exception exception)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Revolutions: Failed at AutoSaveCurrentGame for '{DataStorage.ActiveSaveSlotName ?? "Null"}'", ColorHelper.Red));

                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Exception: {exception.Message}", ColorHelper.Red));
                        InformationManager.DisplayMessage(new InformationMessage($"StackTrace: {exception.StackTrace}", ColorHelper.Red));
                    }
                }
            }
        }
    }
}