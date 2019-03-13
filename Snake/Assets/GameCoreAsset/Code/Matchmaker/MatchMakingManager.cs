using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchMaker
{
    [Serializable]
    public class MatchMakingManager : Singleton<MatchMakingManager>
    {
        public IMatchMaker MatchMaker;

        public MatchMakerModes Mode;

        public bool ShowUI;

        private AutomatchController automatch;

        public AutomatchController Automatch {
            get
            {
                return automatch == null ? new AutomatchController() : automatch;
            }
            set
            {
                automatch = value;
            }
        }

        public GameObject MatchMakerGO;
        public GameObject MatchMakerUIPrefab;

        public int PlayersToStart;

        private void Awake()
        {
            base.Awake();
            if(MatchMakerGO!=null)
            {
                MatchMaker = MatchMakerGO.GetComponent<IMatchMaker>();
            }
        }


        // Use this for initialization
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            if (ShowUI)
            {
                if(MatchMakerUIControllerBase.Instance == null)
                {
                    InstantiateUI();
                }
            }
         }

        private void InstantiateUI()
        {
            var _ui = Instantiate<GameObject>(MatchMakerUIPrefab);
            _ui.transform.SetParent(transform);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ListMatches()
        {
            MatchMaker.MatchListGot += OnListGot;
            MatchMaker.ListMatches();
        }

        private void OnListGot(object sender, EventArgs e)
        {
            MatchMaker.MatchListGot -= OnListGot;
        }

        public void Create()
        {
            MatchMaker.CreateMatch();
        }

        public void Join()
        {
            MatchMaker.JoinMatch();
        }


        public void LeaveMatch()
        {
            MatchMaker.LeaveMatch();
        }

        public void SetMode(MatchMakerModes mode)
        {
            Mode = mode;
        }

        public enum MatchMakerModes
        {
            None,
            Automatch,
            Lobby,
            PlayAndHost,
            LAN,
            DedicatedServer
        }

        public enum MatchMakingStatus
        {
            Offline, 
            Connecting,
            Joined,
            Created
        }

    }
}

