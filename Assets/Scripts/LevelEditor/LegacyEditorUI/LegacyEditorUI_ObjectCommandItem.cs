﻿using BinarySerializer.Ray1;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LegacyEditorUI_ObjectCommandItem : MonoBehaviour
{
    public Command command;

    //UI elements
    public Dropdown uiDropdown;
    public InputField uiArg1;
    public InputField uiArg2;
    public InputField uiArg3;

    private void Start() {
        //Fill out the dropdown
        var all = Enum.GetValues(typeof(CommandType));
        foreach (var e in all) {
            Dropdown.OptionData dat = new Dropdown.OptionData {
                text = e.ToString()
            };
            uiDropdown.options.Add(dat);
        }

        uiDropdown.value = (int)command.CommandType;

        //Apply the values from this command to all the uis
        if (command.Arguments != null) {
            if (command.Arguments.Length >= 1)
                uiArg1.text = command.Arguments[0].ToString();
            if (command.Arguments.Length >= 2)
                uiArg2.text = command.Arguments[1].ToString();
            if (command.Arguments.Length >= 3)
                uiArg3.text = command.Arguments[2].ToString();
        }
    }
}
