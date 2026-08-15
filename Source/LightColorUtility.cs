using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace DefaultWhiteLights;

public static class LightColorUtility
{
    public static bool IsColorableLight(CompGlower glower)
    {
        return glower?.Props != null && glower.Props.colorPickerEnabled;
    }

    public static ColorInt ToGlowColor(Color color)
    {
        ColorInt glow = new ColorInt(color);
        glow.a = 0;
        return glow;
    }

    public static Color ColorFor(RoomRoleDef? role)
    {
        return DefaultWhiteLightsMod.Settings.ColorFor(role);
    }

    public static Color ColorFor(Thing thing)
    {
        Room? room = thing.GetRoom();
        return ColorFor(room?.Role);
    }

    public static void ApplyTo(CompGlower glower)
    {
        if (!IsColorableLight(glower) || glower.parent == null || !glower.parent.Spawned)
        {
            return;
        }

        glower.GlowColor = ToGlowColor(ColorFor(glower.parent));
    }

    public static int ApplyToAllSpawnedLights()
    {
        int count = 0;
        if (Current.ProgramState != ProgramState.Playing || Find.Maps == null)
        {
            return 0;
        }

        foreach (Map map in Find.Maps)
        {
            List<Thing> buildings = map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial);
            for (int i = 0; i < buildings.Count; i++)
            {
                CompGlower? glower = buildings[i].TryGetComp<CompGlower>();
                if (!IsColorableLight(glower))
                {
                    continue;
                }

                ApplyTo(glower!);
                count++;
            }
        }

        return count;
    }
}
