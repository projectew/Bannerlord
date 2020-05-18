using HarmonyLib;
using Revolutions.Settings;
using SandBox.ViewModelCollection.SaveLoad;
using TaleWorlds.Core;

namespace Revolutions.Patches
{
    internal static class SavedGameVMPatches
    {
        //[HarmonyPatch(typeof(MBSaveLoad), "LoadSaveGameData")]
        //internal static class LoadSaveGameData
        //{
        //    internal static void Postfix(string saveName, string[] loadedModuleNames)
        //    {
        //        DataStorage.ActiveSaveSlotName = AccessTools.Field(typeof(MBSaveLoad), "ActiveSaveSlotName").GetValue(null).ToString();

        //        DataStorage.LoadBaseData();
        //        if (RevolutionsSettings.Instance.EnableRevolts)
        //        {
        //            DataStorage.LoadRevoltData();
        //        }

        //        if (RevolutionsSettings.Instance.EnableCivilWars)
        //        {
        //            DataStorage.LoadCivilWarData();
        //        }
        //    }
        //}
    }
}
