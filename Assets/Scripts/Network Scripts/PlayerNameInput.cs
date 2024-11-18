using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField NameInput;
    [SerializeField]
    private ButtonManager Confirm;

    public static string DisplayName {get; private set;}

    private const string PlayerPrefsNameKey = "PlayerName";

    private void Start() => SetUpInputField();

    private void SetUpInputField()
    {
        if(!PlayerPrefs.HasKey(PlayerPrefsNameKey))
        {
            return;
        }
        string defaultname = PlayerPrefs.GetString(PlayerPrefsNameKey);
        NameInput.text = defaultname;

        SetPlayerName();

    }

    public void SetPlayerName()
    {
        Confirm.Interactable(!string.IsNullOrEmpty(NameInput.text));  
    }

    public void SavePlayerName()
    {
        DisplayName = NameInput.text;

        PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
    }
}
