using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

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

public class DefaultWhiteLightsSettings : ModSettings
{
    public Color defaultColor = Color.white;
    public Dictionary<string, RoomLightColor> roomColors = new Dictionary<string, RoomLightColor>();

    private Vector2 scrollPosition;
    private const float ColorBoxSize = 22f;
    private const float MinRowHeight = 28f;
    private const float MinColumnWidth = 200f;

    public Color ColorFor(RoomRoleDef? role)
    {
        if (role != null
            && roomColors.TryGetValue(role.defName, out RoomLightColor setting)
            && setting.enabled)
        {
            return setting.color;
        }

        return defaultColor;
    }

    public void DoWindowContents(Rect inRect)
    {
        Text.Font = GameFont.Small;
        float y = inRect.y;

        string applyLabel = "DWL.ApplyAll".Translate();
        float applyWidth = Mathf.Max(200f, Text.CalcSize(applyLabel).x + 28f);
        Rect top = new Rect(inRect.x, y, inRect.width, 32f);
        Rect applyRect = new Rect(top.xMax - applyWidth, top.y, applyWidth, 30f);
        Rect defaultRect = new Rect(top.x, top.y, top.width - applyWidth - 10f, top.height);
        DrawColorRow(defaultRect, "DWL.AllLights".Translate(), defaultColor, false, null, color =>
        {
            defaultColor = color;
        });

        if (Widgets.ButtonText(applyRect, applyLabel))
        {
            if (Current.ProgramState == ProgramState.Playing)
            {
                int count = LightColorUtility.ApplyToAllSpawnedLights();
                Messages.Message("DWL.ApplyAllDone".Translate(count), MessageTypeDefOf.PositiveEvent, historical: false);
            }
            else
            {
                Messages.Message("DWL.ApplyAllNeedGame".Translate(), MessageTypeDefOf.RejectInput, historical: false);
            }
        }

        y = top.yMax + 4f;
        TaggedString hint = "DWL.RoomHint".Translate();
        float hintHeight = Text.CalcHeight(hint, inRect.width);
        Rect hintRect = new Rect(inRect.x, y, inRect.width, hintHeight);
        GUI.color = Color.gray;
        Widgets.Label(hintRect, hint);
        GUI.color = Color.white;
        y = hintRect.yMax + 6f;

        Widgets.DrawLineHorizontal(inRect.x, y, inRect.width);
        y += 8f;

        Rect headerRect = new Rect(inRect.x, y, inRect.width, 22f);
        Widgets.Label(headerRect, "DWL.RoomHeader".Translate());
        y = headerRect.yMax + 4f;

        Rect gridRect = new Rect(inRect.x, y, inRect.width, inRect.yMax - y);
        DrawRoomGrid(gridRect);
        Text.Font = GameFont.Small;
    }

    private RoomLightColor GetOrCreate(string defName)
    {
        if (!roomColors.TryGetValue(defName, out RoomLightColor setting))
        {
            setting = new RoomLightColor
            {
                enabled = false,
                color = defaultColor
            };
            roomColors[defName] = setting;
        }

        return setting;
    }

    private static List<RoomRoleDef> RoomRoles()
    {
        return DefDatabase<RoomRoleDef>.AllDefsListForReading
            .OrderBy(role => role.defName == "None" ? 0 : role.defName == "Room" ? 1 : 2)
            .ThenBy(role => role.LabelCap.ToString())
            .ToList();
    }

    private static string RoomLabel(RoomRoleDef role)
    {
        if (role.defName == "None")
        {
            return "DWL.Outdoors".Translate();
        }

        return role.LabelCap;
    }

    private void DrawRoomGrid(Rect gridRect)
    {
        List<RoomRoleDef> roles = RoomRoles();
        if (roles.Count == 0)
        {
            return;
        }

        int columns = FitColumns(gridRect.width, gridRect.height, roles.Count, out float rowHeight, out bool scroll);
        int rows = Mathf.CeilToInt(roles.Count / (float)columns);
        float viewWidth = scroll ? gridRect.width - 16f : gridRect.width;
        float columnWidth = viewWidth / columns;
        float viewHeight = rows * rowHeight;

        Rect viewRect = new Rect(0f, 0f, viewWidth, viewHeight);
        if (scroll)
        {
            Widgets.BeginScrollView(gridRect, ref scrollPosition, viewRect);
        }
        else
        {
            GUI.BeginGroup(gridRect);
        }

        for (int i = 0; i < roles.Count; i++)
        {
            int column = i % columns;
            int row = i / columns;
            Rect cell = new Rect(column * columnWidth, row * rowHeight, columnWidth - 8f, rowHeight - 2f);
            RoomRoleDef role = roles[i];
            RoomLightColor setting = GetOrCreate(role.defName);
            DrawColorRow(cell, RoomLabel(role), setting.color, true, setting.enabled, color =>
            {
                setting.color = color;
                setting.enabled = true;
            }, enabled => setting.enabled = enabled);
        }

        if (scroll)
        {
            Widgets.EndScrollView();
        }
        else
        {
            GUI.EndGroup();
        }
    }

    private static int FitColumns(float width, float height, int count, out float rowHeight, out bool scroll)
    {
        for (float minWidth = 240f; minWidth >= 168f; minWidth -= 8f)
        {
            int columns = Mathf.Max(1, Mathf.FloorToInt(width / minWidth));
            int rows = Mathf.CeilToInt(count / (float)columns);
            if (rows * MinRowHeight <= height)
            {
                scroll = false;
                rowHeight = Mathf.Max(MinRowHeight, height / rows);
                return columns;
            }
        }

        scroll = true;
        rowHeight = MinRowHeight;
        return Mathf.Max(1, Mathf.FloorToInt((width - 16f) / MinColumnWidth));
    }

    private static void DrawColorRow(
        Rect row,
        string label,
        Color color,
        bool showCheckbox,
        bool? enabled,
        System.Action<Color> onColor,
        System.Action<bool>? onEnabled = null)
    {
        Widgets.DrawHighlightIfMouseover(row);
        float x = row.x;

        if (showCheckbox)
        {
            bool value = enabled ?? false;
            float checkY = row.y + (row.height - 24f) / 2f;
            Widgets.Checkbox(x, checkY, ref value);
            if (onEnabled != null && value != enabled)
            {
                onEnabled(value);
            }

            x += 26f;
        }

        Rect box = new Rect(x, row.y + (row.height - ColorBoxSize) / 2f, ColorBoxSize, ColorBoxSize);
        Color draw = color;
        draw.a = 1f;
        Widgets.DrawBoxSolid(box, draw);
        Widgets.DrawBox(box);
        if (Mouse.IsOver(box))
        {
            Widgets.DrawHighlight(box);
            TooltipHandler.TipRegion(box, "DWL.ClickColor".Translate());
        }

        if (Widgets.ButtonInvisible(box))
        {
            SoundDefOf.Click.PlayOneShotOnCamera();
            Color fallback = DefaultWhiteLightsMod.Settings.defaultColor;
            Find.WindowStack.Add(new Dialog_LightColorPicker(color, fallback, onColor));
        }

        x += ColorBoxSize + 6f;
        Rect labelRect = new Rect(x, row.y, row.xMax - x, row.height);
        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(labelRect, label);
        Text.Anchor = TextAnchor.UpperLeft;
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref defaultColor, "defaultColor", Color.white);
        Scribe_Collections.Look(ref roomColors, "roomColors", LookMode.Value, LookMode.Deep);
        if (Scribe.mode == LoadSaveMode.PostLoadInit && roomColors == null)
        {
            roomColors = new Dictionary<string, RoomLightColor>();
        }
    }
}
