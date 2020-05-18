using HarmonyLib;
using Revolutions.Settings;
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
                DataStorage.ActiveSaveSlotName = AccessTools.Field(typeof(MBSaveLoad), "ActiveSaveSlotName").GetValue(null).ToString();

                DataStorage.LoadBaseData();

                if (RevolutionsSettings.Instance.EnableRevolts)
                {
                    DataStorage.LoadRevoltData();
                }

                if (RevolutionsSettings.Instance.EnableCivilWars)
                {
                    DataStorage.LoadCivilWarData();
                }
            }
        }

        [HarmonyPatch(typeof(SaveHandler), "QuickSaveCurrentGame")]
        internal static class QuickSaveCurrentGame
        {
            internal static void Postfix()
            {
                DataStorage.ActiveSaveSlotName = AccessTools.Field(typeof(MBSaveLoad), "ActiveSaveSlotName").GetValue(null).ToString();

                DataStorage.SaveBaseData();

                if (RevolutionsSettings.Instance.EnableRevolts)
                {
                    DataStorage.SaveRevoltData();
                }

                if (RevolutionsSettings.Instance.EnableCivilWars)
                {
                    DataStorage.SaveCivilWarData();
                }
            }
        }

        [HarmonyPatch(typeof(SaveHandler), "AutoSave")]
        internal static class SaveHandlerAutoSave
        {
            internal static void Postfix()
            {
                DataStorage.ActiveSaveSlotName = AccessTools.Field(typeof(MBSaveLoad), "ActiveSaveSlotName").GetValue(null).ToString();

                DataStorage.SaveBaseData();

                if (RevolutionsSettings.Instance.EnableRevolts)
                {
                    DataStorage.SaveRevoltData();
                }

                if (RevolutionsSettings.Instance.EnableCivilWars)
                {
                    DataStorage.SaveCivilWarData();
                }
            }
        }

        [HarmonyPatch(typeof(SaveHandler), "SaveAs")]
        internal static class SaveAs
        {
            internal static void Postfix()
            {
                DataStorage.ActiveSaveSlotName = AccessTools.Field(typeof(MBSaveLoad), "ActiveSaveSlotName").GetValue(null).ToString();

                DataStorage.SaveBaseData();

                if (RevolutionsSettings.Instance.EnableRevolts)
                {
                    DataStorage.SaveRevoltData();
                }

                if (RevolutionsSettings.Instance.EnableCivilWars)
                {
                    DataStorage.SaveCivilWarData();
                }
            }
        }
    }
}