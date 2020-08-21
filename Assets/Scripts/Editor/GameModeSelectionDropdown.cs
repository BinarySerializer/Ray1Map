using System.Linq;
using R1Engine;
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
            Attr = EnumExtensions.GetAttribute<GameModeAttribute>(x)
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
            Selection = (GameModeSelection)item.id;
    }

    public GameModeSelection Selection { get; set; }
}