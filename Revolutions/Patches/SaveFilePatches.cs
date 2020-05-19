using HarmonyLib;
using KNTLibrary.Helpers;
using Revolutions.Settings;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Revolutions.Patches
{
    internal static class SaveFilePatches
    {
        [HarmonyPatch(typeof(MBSaveLoad), "LoadSaveGameData")]
        internal static class LoadSaveGameData
        {
            internal static void Postfix()
            {
                try
                {
                    DataStorage.ActiveSaveSlotName = AccessTools.Field(typeof(MBSaveLoad), "ActiveSaveSlotName").GetValue(null)?.ToString() ?? AccessTools.Field(typeof(MBSaveLoad), "AutoSaveName").GetValue(null).ToString();
                    DataStorage.LoadData();
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

        [HarmonyPatch(typeof(SaveHandler), "QuickSaveCurrentGame")]
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

        [HarmonyPatch(typeof(SaveHandler), "AutoSave")]
        internal static class SaveHandlerAutoSave
        {
            internal static void Postfix()
            {
                try
                {
                    DataStorage.ActiveSaveSlotName = AccessTools.Field(typeof(MBSaveLoad), "AutoSaveName").GetValue(null).ToString();
                    DataStorage.SaveData();
                }
                catch (Exception exception)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Revolutions: Failed at AutoSave for '{DataStorage.ActiveSaveSlotName ?? "Null"}'", ColorHelper.Red));

                    if (RevolutionsSettings.Instance.DebugMode)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"Exception: {exception.Message}", ColorHelper.Red));
                        InformationManager.DisplayMessage(new InformationMessage($"StackTrace: {exception.StackTrace}", ColorHelper.Red));
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SaveHandler), "SaveAs")]
        internal static class SaveAs
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
                    InformationManager.DisplayMessage(new InformationMessage($"Revolutions: Failed at SaveAs for '{DataStorage.ActiveSaveSlotName ?? "Null"}'", ColorHelper.Red));

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