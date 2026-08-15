using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace DefaultWhiteLights;

public class Dialog_LightColorPicker : Dialog_ColorPickerBase
{
    private static readonly List<Color> Palette = BuildPalette();

    private readonly Color defaultColor;
    private readonly Action<Color> onSave;

    protected override bool ShowDarklight => true;

    protected override Color DefaultColor => defaultColor;

    protected override List<Color> PickableColors => Palette;

    protected override float ForcedColorValue => 1f;

    protected override bool ShowColorTemperatureBar => true;

    public Dialog_LightColorPicker(Color current, Color defaultColor, Action<Color> onSave)
        : base(CompGlower.visibleColorTextfields, CompGlower.editableColorTextfields)
    {
        this.defaultColor = defaultColor;
        this.onSave = onSave;
        Color.RGBToHSV(current, out float h, out float s, out _);
        color = Color.HSVToRGB(h, s, 1f);
        oldColor = color;
    }

    protected override void SaveColor(Color chosen)
    {
        onSave(chosen);
    }

    private static List<Color> BuildPalette()
    {
        List<Color> colors = new List<Color>();
        for (int i = 0; i < 18; i++)
        {
            float hue = i / 18f;
            colors.Add(Color.HSVToRGB(hue, i == 0 ? 0f : 1f, 1f));
            colors.Add(Color.HSVToRGB(hue, 0.5f, 1f));
            colors.Add(Color.HSVToRGB(hue, 0.33f, 1f));
        }

        return colors;
    }
}
