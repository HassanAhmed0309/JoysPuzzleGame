using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private ButtonManager joinButton;

    private void OnEnable()
    {
        NetworkManagerLobby.OnClientConnected += HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
    }

    private void HandleClientDisconnected()
    {
        joinButton.Interactable(true);
    }

    private void HandleClientConnected()
    {
        joinButton.Interactable(true);

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    public void JoinLobby()
    {
        string ipAddress;
        if (ipAddressInputField.text == "")
        {
            ipAddress = "localhost";
        }
        else
            ipAddress = ipAddressInputField.text;
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.Interactable(false);
    }    

    
}
