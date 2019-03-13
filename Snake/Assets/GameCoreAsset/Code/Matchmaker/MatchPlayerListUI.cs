using System;
using System.Collections;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace MatchMaker
{
    
    public class MatchPlayerListUI : Singleton<MatchPlayerListUI>
    {
        public RectTransform PlayerListContentTransform;
        public GameObject WarningDirectPlayServer;
        public Transform AddButtonRow;

        protected VerticalLayoutGroup layout;
        protected List<IMatchPlayer> players = new List<IMatchPlayer>();

        public void OnEnable()
        {
            layout = PlayerListContentTransform.GetComponent<VerticalLayoutGroup>();
        }


        // Use this for initialization
        void Start()
        {
           
        }

        // Update is called once per frame
        void Update()
        {
            //this dirty the layout to force it to recompute evryframe (a sync problem between client/server
            //sometime to child being assigned before layout was enabled/init, leading to broken layouting)
            if (layout)
                layout.childAlignment = Time.frameCount % 2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
        }

        public void LeaveMatch()
        {
            MatchMakingManager.Instance.LeaveMatch();
        }
      
        public void DisplayDirectServerWarning(bool _enabled)
        {
            if (WarningDirectPlayServer != null)
                WarningDirectPlayServer.SetActive(_enabled);
        }

        public void AddPlayer(IMatchPlayer _player)
        {
            if (players.Contains(_player))
                return;

            players.Add(_player);

            _player.transform.SetParent(PlayerListContentTransform, false);
            AddButtonRow.transform.SetAsLastSibling();

            PlayerListModified();
        }

        public void RemovePlayer(IMatchPlayer _player)
        {
            players.Remove(_player);
            PlayerListModified();
        }

        public void PlayerListModified()
        {
            int i = 0;
            foreach (IMatchPlayer p in players)
            {
                p.OnPlayerListChanged(i);
                ++i;
            }
        }
    }
}



