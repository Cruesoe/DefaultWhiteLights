using HarmonyLib;
using Verse;

namespace DefaultWhiteLights;

[HarmonyPatch(typeof(CompGlower), nameof(CompGlower.PostSpawnSetup))]
public static class Patch_CompGlower_PostSpawnSetup
{
    public static void Postfix(CompGlower __instance, bool respawningAfterLoad)
    {
        // Vanilla skips glow setup while a gravship is being placed, so lights arriving on one are left alone
        if (respawningAfterLoad || __instance.HasGlowColorOverride || __instance.parent.BeingTransportedOnGravship)
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
