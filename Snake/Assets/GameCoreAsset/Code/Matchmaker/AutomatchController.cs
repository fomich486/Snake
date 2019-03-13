using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchMaker
{
    public class AutomatchController 
    {
        public float WaitingTime;
        private IMatchMaker matchMaker;
        
        public void Start()
        {
            MatchMakingManager.Instance.Mode = MatchMakingManager.MatchMakerModes.Automatch;
            FindMatch();
        }

        public void FindMatch()
        {
            matchMaker = MatchMakingManager.Instance.MatchMaker;
            matchMaker.StartMatchMaker();
            matchMaker.MatchListGot += OnList;
            matchMaker.ListMatches();
        }

        private void OnList(object sender, EventArgs e)
        {
            matchMaker.MatchListGot -= OnList;
            if (e != EventArgs.Empty)
            {
                var _matches = e as IMatchListResponce;
                for (var i = 0; i < _matches.Count; i++)
                {
                    if (matchMaker.IsOkToJoin(matchMaker.JoinParameters, _matches[i]))
                    {
                        matchMaker.MatchJoined += OnJoin;
                        matchMaker.JoinMatch(_matches[i]);
                        return;
                    }
                }
                matchMaker.MatchCreated += OnCreate;
                matchMaker.CreateMatch();
            }
            else
            {
                matchMaker.MatchCreated += OnCreate;
                matchMaker.CreateMatch();
            }
           
        }

        private void OnCreate(object sender, EventArgs e)
        {
            matchMaker.MatchCreated -= OnCreate;
            Debug.Log("Created");
        }

        private void OnJoin(object sender, EventArgs e)
        {
            matchMaker.MatchJoined -= OnJoin;
            //Do spawning
            Debug.Log("Joined");
        }

        public void SetMatchParameters(IMatchCreateParameters _params)
        {
            matchMaker.MatchJoined -= OnJoin;
            matchMaker.CreateParameters = _params;
        }
     
    }
}


