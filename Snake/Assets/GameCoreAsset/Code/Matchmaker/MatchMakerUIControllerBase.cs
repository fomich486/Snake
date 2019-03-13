using System;
using System.Collections;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace MatchMaker
{
    public class MatchMakerUIControllerBase : Singleton<MatchMakerUIControllerBase>
    {
        public LobbyInfoPanel InfoPanel;
        public LobbyTopPanel TopPanel;

        protected RectTransform currentPanel;

        public RectTransform mainMenuPanel;

        public RectTransform lobbyPanel;

        public Button backButton;

        public LobbyCountdownPanel countdownPanel;
        public GameObject addPlayerButton;

        public Text statusInfo;
        public Text hostInfo;


        private void Start()
        {
            currentPanel = mainMenuPanel;

            backButton.gameObject.SetActive(false);
            GetComponent<Canvas>().enabled = true;

            DontDestroyOnLoad(gameObject);

            SetServerInfo("Offline", "None");
        }


        public void ChangeTo(RectTransform newPanel)
        {
            if (currentPanel != null)
            {
                currentPanel.gameObject.SetActive(false);
            }

            if (newPanel != null)
            {
                newPanel.gameObject.SetActive(true);
            }

            currentPanel = newPanel;

            if (currentPanel != mainMenuPanel)
            {
                backButton.gameObject.SetActive(true);
            }
            else
            {
                backButton.gameObject.SetActive(false);
                SetServerInfo("Offline", "None");
            }
        }

        public void DisplayIsConnecting()
        {
            var _this = this;
            InfoPanel.Display("Connecting...", "Cancel", () => { _this.backDelegate(); });
        }

        public void SetServerInfo(string status, string host)
        {
            statusInfo.text = status;
            hostInfo.text = host;
        }

        public void GoBackButton()
        {
            backDelegate();
            TopPanel.isInGame = false;
        }

        public delegate void BackButtonDelegate();
        public BackButtonDelegate backDelegate;

    }
}



