using R1Engine;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class GameModeSelectionDropdown : AdvancedDropdown
{
    public GameModeSelectionDropdown(AdvancedDropdownState state) : base(state)
    {
        minimumSize = new Vector2(50, 700f);
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        var modes = EnumHelpers.GetValues<GameModeSelection>().Select(x => new
        {
            Mode = x,
            Attr = x.GetAttribute<GameModeAttribute>()
        }).GroupBy(x => x.Attr.MajorEngineVersion);

        var root = new AdvancedDropdownItem("Game");

        foreach (var mode in modes)
        {
            var group = new AdvancedDropdownItem(mode.Key.ToString())
            {
                id = -1
            };

            foreach (var selectionGroup in mode.GroupBy(x => x.Attr.EngineVersion))
            {
                foreach (var selection in selectionGroup)
                {
                    group.AddChild(new AdvancedDropdownItem(selection.Attr.DisplayName)
                    {
                        id = (int)selection.Mode
                    });
                }

                group.AddSeparator();
            }

            root.AddChild(group);
        }

        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        base.ItemSelected(item);

        if (item.id != -1)
        {
            Selection = (GameModeSelection)item.id;
            SelectionName = Selection.GetAttribute<GameModeAttribute>().DisplayName;
            HasChanged = true;
        }
    }

    public bool HasChanged { get; set; } = true;
    public GameModeSelection Selection { get; set; } = Settings.SelectedGameMode;
    public string SelectionName { get; set; } = Settings.SelectedGameMode.GetAttribute<GameModeAttribute>().DisplayName;
}