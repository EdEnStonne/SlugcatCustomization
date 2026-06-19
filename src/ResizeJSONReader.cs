using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Resize;
public static class ResizeJSONReader
{
    public const string fileName = "resize.json";

    public const string HEIGHT = "height";
    public const string HEIGHTR = "heightRatio";
    public const string PUPHEIGHT = "pupHeight";
    public const string PUPHEIGHTR = "pupheightRatio";

    public const string LOCALONLY = "onlyLocal";
    public const string NPCONLY = "onlyNPC";
    public const string RENDERPUP = "alwaysRenderAsPup";

    public const string TAIL = "tail";
    public const string TAILINDEX = "index";
    public const string TAILRAD = "radius";
    public const string TAILRADR = "radiusRatio";
    public const string TAILCRAD = "connectionRadius";
    public const string TAILCRADR = "connectionRadiusRatio";
    public const string GOLBALTAILRADR = "tailRadiusRatio";
    public const string GOLBALTAILCRADR = "tailConnectionRadiusRatio";
    
    public static string FindFilePathOrNull()
    {
        if (ModManager.ActiveMods.FirstOrDefault(x => x.id == Plugin.MOD_ID) is ModManager.Mod resizeMod)
        {
            string filePath = Path.Combine(resizeMod.path, fileName.ToLowerInvariant());
            if (File.Exists(filePath))
            {
                return filePath;
            }
        }
        return null;
    }
    
    internal static void RefreshCustomizationInfos()
    {
        string filePath = FindFilePathOrNull();
        if (filePath is not null)
        {
            SlugcatCustomization.customizations.Clear();
            string content = File.ReadAllText(filePath);
            JObject customizationPerSlug = JObject.Parse(content);
            Plugin.Log($"Found resize data !");

            foreach (var item in customizationPerSlug)
            {
                Plugin.Log($"Data for slugcat {item.Key} :");
                SlugcatCustomization customization = new(item.Key);
                JObject customizationContent = JObject.Parse(item.Value.ToString());
                
                customization.slugHeight = customizationContent.GetValue(HEIGHT)?.ToObject<float?>() ?? customization.slugHeight;
                customization.SlugHeightRatio = customizationContent.GetValue(HEIGHTR)?.ToObject<float?>() ?? customization.SlugHeightRatio;
                customization.slugPupHeight = customizationContent.GetValue(PUPHEIGHT)?.ToObject<float?>() ?? customization.slugPupHeight;
                customization.SlugPupHeightRatio = customizationContent.GetValue(PUPHEIGHTR)?.ToObject<float?>() ?? customization.SlugPupHeightRatio;
                
                customization.onlyLocal = customizationContent.GetValue(LOCALONLY)?.ToObject<bool?>() ?? customization.onlyLocal;
                customization.onlyNPC = customizationContent.GetValue(NPCONLY)?.ToObject<bool?>() ?? customization.onlyNPC;
                customization.renderAsPup = customizationContent.GetValue(RENDERPUP)?.ToObject<bool?>() ?? customization.renderAsPup;
                
                if (customizationContent.GetValue(GOLBALTAILRADR)?.ToObject<float?>() is float globalTailRad)
                {
                    for (int i = 0; i < customization.tailParts.Length; i++)
                    {
                        customization.tailParts[i].radiusRatio = globalTailRad;
                    }
                }
                if (customizationContent.GetValue(GOLBALTAILCRADR)?.ToObject<float?>() is float globalTailConRad)
                {
                    for (int i = 0; i < customization.tailParts.Length; i++)
                    {
                        customization.tailParts[i].connectionRadiusRatio = globalTailConRad;
                    }
                }
                foreach (JToken tailSegSpec in customizationContent.GetValue(TAIL)?.Children() ?? [])
                {
                    JObject tailSegSpecObj = JObject.Parse(tailSegSpec.ToString());
                    if (tailSegSpecObj.GetValue(TAILINDEX)?.ToObject<int?>() is int index
                        && index >= 0
                        && index < customization.tailParts.Length)
                    {
                        customization.tailParts[index].radius = tailSegSpecObj.GetValue(TAILRAD)?.ToObject<float?>() ?? customization.tailParts[index].radius;
                        customization.tailParts[index].radiusRatio = tailSegSpecObj.GetValue(TAILRADR)?.ToObject<float?>() ?? customization.tailParts[index].radiusRatio;
                        customization.tailParts[index].connectionRadius = tailSegSpecObj.GetValue(TAILCRAD)?.ToObject<float?>() ?? customization.tailParts[index].connectionRadius;
                        customization.tailParts[index].connectionRadiusRatio = tailSegSpecObj.GetValue(TAILCRADR)?.ToObject<float?>() ?? customization.tailParts[index].connectionRadiusRatio;
                    }
                }

                customization.LogSpecifics();
                SlugcatCustomization.customizations.Add(customization);
            }
        }
        else
        {
            Plugin.LogError("Could not find resize data :<");
        }
    }
}