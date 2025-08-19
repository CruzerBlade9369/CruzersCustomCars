using System;
using System.Reflection;
using HarmonyLib;
using UnityModManagerNet;

namespace BreakdownCrane
{
    public static class Main
    {
        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            Harmony harmony = new Harmony(modEntry.Info.Id);

            try
            {
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                modEntry.Logger.LogException($"Failed to load {modEntry.Info.DisplayName}:", ex);
                harmony.UnpatchAll(modEntry.Info.Id);
                return false;
            }

            return true;
        }
    }
}
