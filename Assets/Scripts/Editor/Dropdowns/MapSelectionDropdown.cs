using System.Collections.Generic;
using System.Linq;
using R1Engine;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class MapSelectionDropdown : AdvancedDropdown
{
    public MapSelectionDropdown(AdvancedDropdownState state, GameInfo_Volume[] gameVolumes, Game game) : base(state)
    {
        GameVolumes = gameVolumes;

        LevelNames = MapNames.GetMapNames(game) ?? 
                     gameVolumes?.FirstOrDefault()?.Worlds.ToDictionary(x => x.Index, x => x.GetLevelNamesDictionary()) ?? 
                     new Dictionary<int, Dictionary<int, string>>();
        WorldNames = MapNames.GetWorldNames(game) ??
                     gameVolumes?.FirstOrDefault()?.Worlds.ToDictionary(x => x.Index, x => x.WorldName) ??
                     new Dictionary<int, string>();

        minimumSize = new Vector2(50, 400f);
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("Map");

        if (HasVolumes)
        {
            foreach (var vol in GameVolumes)
                root.AddChild(AddWorlds(new AdvancedDropdownItem(vol.Name), vol));
        }
        else
        {
            AddWorlds(root, GameVolumes.First());
        }

        return root;
    }

    protected string GetName(int index, string name) => name != null ? $"{index} - {name}" : $"{index}";
    public string GetWorldName(int worldIndex) => WorldNames?.TryGetItem(worldIndex) ?? $"{worldIndex:00}";
    public string GetLevelName(int worldIndex, int levelIndex) => GetName(levelIndex, LevelNames?.TryGetItem(worldIndex)?.TryGetItem(levelIndex));

    protected AdvancedDropdownItem AddWorlds(AdvancedDropdownItem parent, GameInfo_Volume vol)
    {
        int id = 0;
        foreach (var w in vol.Worlds.Where(x => x.Maps.Length > 0))
        {
            var worldItem = new AdvancedDropdownItem(GetName(w.Index, WorldNames?.TryGetItem(w.Index))) { id = -1 };

            foreach (var m in w.Maps.OrderBy(x => x))
                worldItem.AddChild(new MapSelectionDropdownItem(GetLevelName(w.Index, m), vol.Name, w.Index, m) { id = id++ });

            parent.AddChild(worldItem);
        }

        return parent;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        base.ItemSelected(item);

        if (item.id == -1 || !(item is MapSelectionDropdownItem mapItem)) 
            return;

        SelectedVolume = mapItem.Volume;
        SelectedWorld = mapItem.World;
        SelectedMap = mapItem.Map;
        HasChanged = true;
    }

    public bool HasVolumes => GameVolumes.Any() && (GameVolumes.Length > 1 || GameVolumes.First().Name != null);
    protected GameInfo_Volume[] GameVolumes { get; }
    public Dictionary<int, Dictionary<int, string>> LevelNames { get; }
    public Dictionary<int, string> WorldNames { get; }

    public bool HasChanged { get; set; }

    public string SelectedVolume { get; set; } = Settings.EduVolume;
    public int SelectedWorld { get; set; } = Settings.World;
    public int SelectedMap { get; set; } = Settings.Level;

    public class MapSelectionDropdownItem : AdvancedDropdownItem
    {
        public MapSelectionDropdownItem(string name, string volume, int world, int map) : base(name)
        {
            Volume = volume;
            World = world;
            Map = map;
        }

        public string Volume { get; }
        public int World { get; }
        public int Map { get; }
    }
}