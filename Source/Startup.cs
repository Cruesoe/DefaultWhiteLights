using HarmonyLib;
using Verse;

namespace DefaultWhiteLights;

[StaticConstructorOnStartup]
public static class Startup
{
    static Startup()
    {
        new Harmony("cruesoe.defaultwhitelights").PatchAll();
    }
}
