using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using IL.MoreSlugcats;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace Resize;

public class TailPartCustomization(float radiusRatio = 1f, float connectionRadiusRatio = 1f)
{
    public float radiusRatio = radiusRatio;
    public float? radius = null;
    public float connectionRadiusRatio = connectionRadiusRatio;
    public float? connectionRadius = null;
}
public class SlugcatCustomization(string slugcatClass)
{
    public static List<SlugcatCustomization> customizations = new();
    public static ConditionalWeakTable<Player, SlugcatCustomization> customizationsLink = new();
    private static string CommonNameToEnum(string NameEnum)
    {
        NameEnum = NameEnum.ToLowerInvariant();
        if (NameEnum == "Survivor".ToLowerInvariant())
        {
            return SlugcatStats.Name.White.value.ToLowerInvariant();
        }
        else if (NameEnum == "Monk".ToLowerInvariant())
        {
            return SlugcatStats.Name.Yellow.value.ToLowerInvariant();
        }
        else if (NameEnum == "Hunter".ToLowerInvariant())
        {
            return SlugcatStats.Name.Red.value.ToLowerInvariant();
        }
        else if (ModManager.MSC && NameEnum == "Spearmaster".ToLowerInvariant())
        {
            return MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear.value.ToLowerInvariant();
        }
        else if (ModManager.MSC && NameEnum == "Inv".ToLowerInvariant())
        {
            return MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel.value.ToLowerInvariant();
        }
        return NameEnum;
    }
    public bool Compatible(Player player)
    {
        bool isLocal = RSFunc.IsLocal(player);
        return (!onlyLocal || isLocal) && (!onlyNPC || player.isNPC) &&
            (
                slugcatClass == ALLSLUGCAT
                || player.SlugCatClass.value.ToLowerInvariant() == slugcatClass.ToLowerInvariant()
            );
    }
    public void LogSpecifics()
    {
        Plugin.Log("The specifics are :"
            + "\n" + $"Slugcat : {this.slugcatClass}"
            + "\n" + $"Height : {this.slugHeight}"
            + "\n" + $"Height (Pup) : {this.slugPupHeight}"
            + "\n" + $"Only Local : {this.onlyLocal}"
            + "\n" + $"Only NPC : {this.onlyNPC}"
            + "\n" + $"Pup Render : {this.renderAsPup}"
            + "\n" + $"Tail segment 0 :"
            + "\n" + $"    > Radius : {this.tailParts[0].radius?.ToString() ?? $"x{this.tailParts[0].radiusRatio}"}"
            + "\n" + $"    > Connection Radius : {this.tailParts[0].connectionRadius?.ToString() ?? $"x{this.tailParts[0].connectionRadiusRatio}"}"
            + "\n" + $"Tail segment 1 :"
            + "\n" + $"    > Radius : {this.tailParts[1].radius?.ToString() ?? $"x{this.tailParts[1].radiusRatio}"}"
            + "\n" + $"    > Connection Radius : {this.tailParts[1].connectionRadius?.ToString() ?? $"x{this.tailParts[1].connectionRadiusRatio}"}"
            + "\n" + $"Tail segment 2 :"
            + "\n" + $"    > Radius : {this.tailParts[2].radius?.ToString() ?? $"x{this.tailParts[2].radiusRatio}"}"
            + "\n" + $"    > Connection Radius : {this.tailParts[2].connectionRadius?.ToString() ?? $"x{this.tailParts[2].connectionRadiusRatio}"}"
            + "\n" + $"Tail segment 3 :"
            + "\n" + $"    > Radius : {this.tailParts[3].radius?.ToString() ?? $"x{this.tailParts[3].radiusRatio}"}"
            + "\n" + $"    > Connection Radius : {this.tailParts[3].connectionRadius?.ToString() ?? $"x{this.tailParts[3].connectionRadiusRatio}"}"
        );
    }
    public const string ALLSLUGCAT = "all";
    public string slugcatClass = CommonNameToEnum(slugcatClass);

    public const float defaultSize = 17f;
    public float slugHeight = defaultSize;
    public float SlugHeightRatio
    {
        get => slugHeight / defaultSize;
        set => slugHeight = defaultSize * value;
    }
    public const float defaultPupSize = 12f;
    public float slugPupHeight = defaultPupSize;
    public float SlugPupHeightRatio
    {
        get => slugPupHeight / defaultPupSize;
        set => slugPupHeight = defaultPupSize * value;
    }

    public bool onlyLocal = false;
    public bool onlyNPC = false;
    public bool renderAsPup = false;
    
    public TailPartCustomization[] tailParts = [new(), new(), new(), new()];
}

public static class SlugcatCustomizationHooks
{
    internal static void ApplyHooks()
    {
        On.Player.ctor += Player_ctor_LinkCustomization;
        IL.Player.MovementUpdate += Player_MovementUpdate_ModifyHeight;
        On.PlayerGraphics.ctor += PlayerGraphics_ctor_ChangeTailSize;
        new Hook(typeof(PlayerGraphics).GetProperty(nameof(PlayerGraphics.RenderAsPup)).GetGetMethod(), PlayerGraphics_RenderAsPup_ChangeRender);
        Plugin.Log("ApplyHooks of SlugcatCustomizationHooks ended !");
    }
    private static float ChangeHeight(float orig, Player player)
    {
        if (SlugcatCustomization.customizationsLink.TryGetValue(player, out var customization))
        {
            return orig * ((player.isSlugpup && player.playerState.isPup) 
                ? customization.SlugPupHeightRatio
                : customization.SlugHeightRatio);
        }
        return orig;
    }
    private static void Player_MovementUpdate_ModifyHeight(ILContext il)
    {
        Plugin.Log("SlugcatCustomization IL 1 starts");
        try
        {
            Plugin.Log("Trying to hook IL");
            ILCursor cursor = new(il);
            if (cursor.TryGotoNext(MoveType.After, 
                x => x.MatchLdloc(4),
                x => x.MatchConvR4()))
            {
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.EmitDelegate(ChangeHeight);
            }
            else
            {
                Plugin.LogError("Couldn't find IL hook :<");
            }
            Plugin.Log("IL hook ended");
        }
        catch (Exception ex)
        {
            Plugin.LogError(ex);
        }
        Plugin.Log("SlugcatCustomization IL 1 ends");
    }
    private delegate bool orig_RenderAsPup(PlayerGraphics self);
    private static bool PlayerGraphics_RenderAsPup_ChangeRender(orig_RenderAsPup orig, PlayerGraphics self)
    {
        if (SlugcatCustomization.customizationsLink.TryGetValue(self.player, out var customization) 
            && customization.renderAsPup)
        {
            return true;
        }
        return orig(self);
    }
    private static void PlayerGraphics_ctor_ChangeTailSize(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
    {
        orig(self, ow);
        for (int i = 0; i < SlugcatCustomization.customizations.Count; i++)
        {
            if (SlugcatCustomization.customizations[i].Compatible(self.player) 
                && !SlugcatCustomization.customizationsLink.TryGetValue(self.player, out _))
            {
                SlugcatCustomization.customizationsLink.Add(self.player, SlugcatCustomization.customizations[i]);
            }
        }
        
        if (SlugcatCustomization.customizationsLink.TryGetValue(self.player, out var customization))
        {
            for (int i = 0; i < self.tail.Length; i++)
            {
                self.tail[i].rad = customization.tailParts[i].radius is float radius 
                    ? radius 
                    : self.tail[i].rad * customization.tailParts[i].radiusRatio;
                self.tail[i].connectionRad = customization.tailParts[i].radius is float connectionRadius 
                    ? connectionRadius 
                    : self.tail[i].connectionRad * customization.tailParts[i].connectionRadiusRatio;
            }
            Plugin.Log($"Found customization for player {self.player} : {self.player.SlugCatClass} !");
            customization.LogSpecifics();
        }
    }
    private static void Player_ctor_LinkCustomization(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);
        for (int i = 0; i < SlugcatCustomization.customizations.Count; i++)
        {
            if (SlugcatCustomization.customizations[i].Compatible(self) 
                && !SlugcatCustomization.customizationsLink.TryGetValue(self, out _))
            {
                SlugcatCustomization.customizationsLink.Add(self, SlugcatCustomization.customizations[i]);
            }
        }
    }
}