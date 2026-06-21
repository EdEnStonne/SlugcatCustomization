using BepInEx;
using BepInEx.Logging;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using UnityEngine;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace Resize;

[BepInPlugin(MOD_ID, "Resize", MOD_VERSION)]
sealed class Plugin : BaseUnityPlugin
{
    public const string MOD_ID = "qski.resize";
    public const string MOD_VERSION = "1.1.0";
    private new static ManualLogSource Logger;

    private static bool IsInit = false;
    internal static bool debug = true;

    public static bool meadowEnabled = false;
    
    public static string GetTimeString() => DateTime.Now.Hour.ToString() +":"+ DateTime.Now.Minute.ToString() +":"+ DateTime.Now.Second.ToString() +"."+ DateTime.Now.Millisecond.ToString();
    private static string TrimPath(string callerPath) { return (callerPath = callerPath.Substring(Mathf.Max(callerPath.LastIndexOf(Path.DirectorySeparatorChar), callerPath.LastIndexOf(Path.AltDirectorySeparatorChar)) + 1)).Substring(0, callerPath.LastIndexOf('.')); }

    public static void Log(object data, [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
    {
        if (Logger != null && debug)
        {
            Logger.LogDebug($"[{GetTimeString()}|{TrimPath(callerPath)}.{callerName}.{callerLine}] : "+ data);
        }
    }
    public static void LogError(object data, [CallerMemberName] string callerName = "", [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
    {
        if (Logger != null)
        {
            Logger.LogError($"[{GetTimeString()}|{TrimPath(callerPath)}.{callerName}.{callerLine}] : "+ data);;
        }
    }
    public void OnEnable()
    {
        Logger = base.Logger;
        On.RainWorld.OnModsInit += OnModsInit;
    }

    private void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        if (IsInit) return;
        IsInit = true;

        try
        {
            Log("Connecting the Remix menu !");

            MachineConnector.SetRegisteredOI(MOD_ID, RSRemix.instance);

            Log("Starting the hooks !");

            SlugcatCustomizationHooks.ApplyHooks();
            ResizeJSONReader.RefreshCustomizationInfos();

            Log("Hooks ended !");
        }
        catch (Exception ex)
        {
            LogError("Exception while hooking ! " + ex);
        }
        
    }
    public static void CheckMods()
    {
        Log("Checking Mods starts !");

        foreach (ModManager.Mod mod in ModManager.ActiveMods.FindAll(x => x.enabled))
        {
            if (mod.id == "henpemaz_rainmeadow")
            {
                Log("Found meadow !");
                meadowEnabled = true;
            }
        }

        Log("Checking Mods ends !");
    }
}
