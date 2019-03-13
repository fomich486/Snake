using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

namespace MatchMaker.Prototype.NetworkLobby
{
    public class MatchServerEntryUNET : MonoBehaviour, IServerEntry
    {
        public Text serverInfoText;
        public Text slotInfo;
        public Button joinButton;

        public void Populate(IMatchInfo _match, IMatchMaker _lobbyManager, Color c)
        {
            var _matchInfo = (_match as MatchInfoUNET).Info;
            serverInfoText.text = _matchInfo.name;
            slotInfo.text = _matchInfo.currentSize.ToString() + "/" + _matchInfo.maxSize.ToString(); ;
            joinButton.onClick.RemoveAllListeners();
            joinButton.onClick.AddListener(() => { JoinMatch(_match, _lobbyManager); });
            GetComponent<Image>().color = c;
        }

        public void JoinMatch(IMatchInfo _parameters, IMatchMaker _lobbyManager)
        {
            MatchMakingManager.Instance.MatchMaker.JoinMatch(_parameters);
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.DisplayIsConnecting();
            }
        }
    }
}