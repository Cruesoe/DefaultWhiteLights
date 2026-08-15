# Default White Lights

RimWorld 1.6. Vanilla standing lamps, wall lamps, and flood lights (and other colourable electric lights) default to white instead of orange.

## Settings

**Options → Mod options → Default White Lights**

- **All lights** — global default colour (white unless you change it).
- **Room colours** — optional. Tick a room type and click the colour box to give lights built in that room a different default.
- **Apply to all existing lights** — recolours colourable electric lights already on the map to match the current settings (by the room they are in now). Requires a loaded colony.

New lights pick up the colour when they finish building. Lights you have already recoloured in-game keep that colour until you hit Apply.

Sun lamps, torches, and other non-colourable glowers are unchanged. The in-game colour picker and Ideology darklight toggle still work.

The XML patch still sets the vanilla orange def colour to white, so uncustomised lights are white even before the C# default is applied.

## Install

Requires [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077). Copy this folder to `RimWorld\Mods\`, or add it as a local mod in RimSort.

## Build

```
dotnet build Source\DefaultWhiteLights.csproj -c Debug
```

The DLL is copied to `1.6\Assemblies\DefaultWhiteLights.dll` and to `RimWorld\Mods\Default White Lights\1.6\Assemblies\`.
