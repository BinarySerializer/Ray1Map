using R1Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandLine : MonoBehaviour
{
    public Common_EventCommand command;

    //UI elements
    public Dropdown uiDropdown;
    public InputField uiArg1;
    public InputField uiArg2;
    public InputField uiArg3;

    private void Start() {
        //Fill out the dropdown
        var all = Enum.GetValues(typeof(EventCommand));
        foreach (var e in all) {
            Dropdown.OptionData dat = new Dropdown.OptionData {
                text = e.ToString()
            };
            uiDropdown.options.Add(dat);
        }

        uiDropdown.value = (int)command.Command;

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
