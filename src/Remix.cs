using UnityEngine;
using Menu.Remix.MixedUI;
using Menu;

namespace Resize;
public class RSRemix : OptionInterface
{
    private const float basePosY = 550f;
    private const float spacingY = 35f;
    private const float textUpY = 4f;
    private OpSimpleButton refreshJSON;
    private OpSimpleButton openJSON;
    private OpCheckBox toggleDebug;
    public override void Initialize()
    {
        base.Initialize();

        this.Tabs = [new(this, "Mod Config")];

        Tabs[0].AddItems([
            new OpLabel(20f, basePosY - spacingY * 0, "Mod Configuration", true) { description = "Trailseeker section" },

            toggleDebug = new OpCheckBox(EnableDebugLogging, new Vector2(20f, basePosY - spacingY * 1)) { description = EnableDebugLogging.info.description },
            new OpLabel(50f, basePosY - spacingY * 1 + textUpY, "Enable Debug Logging") { description = EnableDebugLogging.info.description },
            
            refreshJSON = new OpSimpleButton(new Vector2(20f, basePosY - spacingY * 2), new Vector2(150f, 30f), Translate("Reload JSON")) { description = "Reload the JSON values into the game without having to restart the game."},
            openJSON = new OpSimpleButton(new Vector2(20f, basePosY - spacingY * 3), new Vector2(150f, 30f), Translate("Open JSON")) { description = "Open the JSON."},
        ]);

        Plugin.debug = EnableDebugLogging.Value;
        toggleDebug.OnValueChanged += (UIconfig config, string value, string oldValue) =>
        {
            Plugin.debug = EnableDebugLogging.Value;
        };
        refreshJSON.OnClick += (UIfocusable trigger) => 
        { 
            trigger.Menu.PlaySound(SoundID.HUD_Game_Over_Prompt);
            ResizeJSONReader.RefreshCustomizationInfos(); 
        };
        openJSON.OnClick += _ =>
        {
            try
            {
                string filePath = ResizeJSONReader.FindFilePathOrNull();
                if (filePath is not null)
                {
                    System.Diagnostics.Process.Start(AssetManager.ResolveFilePath(filePath));
                }
            }
            catch (System.Exception e)
            {
                Plugin.LogError(e);
            }
        };
    }

    public static RSRemix instance = new();

    // Arena items
    public static Configurable<bool> EnableDebugLogging = instance.config.Bind("EnableDebugLogging", false, 
        new ConfigurableInfo("Enable extensive debug logging. Good to find crash and bugs, at the cost of light performance debuff. Default false.")
    );
}