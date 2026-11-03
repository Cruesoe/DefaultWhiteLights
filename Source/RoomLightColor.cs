using UnityEngine;
using Verse;

namespace DefaultWhiteLights;

public class RoomLightColor : IExposable
{
    public bool enabled;
    public Color color = Color.white;

    public void ExposeData()
    {
        Scribe_Values.Look(ref enabled, "enabled", false);
        Scribe_Values.Look(ref color, "color", Color.white);
    }
}
