using UnityEngine;
using Verse;

namespace DefaultWhiteLights;

public class DefaultWhiteLightsMod : Mod
{
    public static DefaultWhiteLightsSettings Settings = null!;

    public DefaultWhiteLightsMod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<DefaultWhiteLightsSettings>();
    }

    public override string SettingsCategory()
    {
        return "DWL.SettingsCategory".Translate();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        Settings.DoWindowContents(inRect);
    }
}
