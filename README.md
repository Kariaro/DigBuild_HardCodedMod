# HardCodedMod

This mod introduces special features to the DigBuild game such as:

* Fluid Storage
* Wood doors

# Usage

DigBuild is currently under construction so this mod is not fully loaded by the game yet.
This means that if you install this mod there is a high probability that it won't work.

### Step 1

Inside DigBuildGame.cs you need to add the following to the ResourceManager Dictionary:
```cs
{ "hardcodedmod", "../../HardCodedMod/Resources" }
```

### Step 2
Inside ModLoader.cs you need to add the following mod:
```cs
{
    var content2 = Assembly.LoadFrom(Path.GetFullPath("HardCodedMod.Content.dll"));
    _mods.Add((IMod) content2.CreateInstance("HardCodedMod.Content.HardCodedMod")!);
}
```

This should be enough to make the mod load.

# DigBuild

DigBuild is a game created by [amadornes](https://github.com/amadornes)
His twitter can be found here [@amadornes](https://twitter.com/amadornes)

The game can be found here: [DigBuild](https://github.com/DigBuild)
