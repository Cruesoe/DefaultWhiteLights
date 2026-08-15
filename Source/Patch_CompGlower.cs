using HarmonyLib;
using Verse;

namespace DefaultWhiteLights;

[HarmonyPatch(typeof(CompGlower), nameof(CompGlower.PostSpawnSetup))]
public static class Patch_CompGlower_PostSpawnSetup
{
    public static void Postfix(CompGlower __instance, bool respawningAfterLoad)
    {
        if (respawningAfterLoad || __instance.HasGlowColorOverride)
        {
            return;
        }

        if (!LightColorUtility.IsColorableLight(__instance))
        {
            return;
        }

        LightColorUtility.ApplyTo(__instance);
    }
}
