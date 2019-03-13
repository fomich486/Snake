using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MatchMaker;

namespace Prototype.NetworkLobby
{
    //Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
    public class LobbyMainMenu : MonoBehaviour 
    {
        //public LobbyManager lobbyManager;

        public RectTransform lobbyServerList;
        public RectTransform lobbyPanel;

        public InputField ipInput;
        public InputField matchNameInput;

        public void OnEnable()
        {
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.TopPanel.ToggleVisibility(true);
            }

            ipInput.onEndEdit.RemoveAllListeners();
            ipInput.onEndEdit.AddListener(onEndEditIP);

            matchNameInput.onEndEdit.RemoveAllListeners();
            matchNameInput.onEndEdit.AddListener(onEndEditGameName);
        }

        ////public void OnClickHost()
        ////{
        ////    lobbyManager.StartHost();
        ////}

        public void OnClickJoin()
        {
            //MatchMakerUIControllerBase.Instance.ChangeTo(lobbyPanel);

            //lobbyManager.networkAddress = ipInput.text;
            //MatchMakingManager.Instance.Join();

            //MatchMakerUIControllerBase.Instance.backDelegate = lobbyManager.StopClientClbk;
            //MatchMakerUIControllerBase.Instance.DisplayIsConnecting();

            //MatchMakerUIControllerBase.Instance.SetServerInfo("Connecting...", lobbyManager.networkAddress);
        }

        public void OnClickDedicated()
        {
            //MatchMakerUIControllerBase.Instance.ChangeTo(null);
            //MatchMakerUIControllerBase.Instance.StartServer();

            //MatchMakerUIControllerBase.Instance.backDelegate = lobbyManager.StopServerClbk;

            //MatchMakerUIControllerBase.Instance.SetServerInfo("Dedicated Server", lobbyManager.networkAddress);
        }

        public void OnClickAutomatch()
        {
            MatchMakingManager.Instance.Automatch.Start();
        }

        public void OnClickCreateMatchmakingGame()
        {
            
            MatchMakingManager.Instance.MatchMaker.StartMatchMaker();
            MatchMakingManager.Instance.MatchMaker.CreateMatch(                //matchNameInput.text,
                //(uint)lobbyManager.maxPlayers,
                //true,
                //"", "", "", 0, 0,
                //lobbyManager.OnMatchCreate
                );
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.DisplayIsConnecting();
            }
        }

        public void OnClickOpenServerList()
        {
            MatchMakingManager.Instance.MatchMaker.StartMatchMaker();
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.ChangeTo(lobbyServerList);
            }
        }

        void onEndEditIP(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickJoin();
            }
        }

        void onEndEditGameName(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickCreateMatchmakingGame();
            }
        }

    }
}
