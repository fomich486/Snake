using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchMaker;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
using NetworkExtension;
using UnityEngine.UI;

public class MatchMakerExample : MonoBehaviour
{
    /// <summary>
    /// Rank of current player
    /// </summary>
    public float PlayerRank;
    public Text Status;
    public Text PlayerCountText;
    public InputField InputRank;
    public InputField InputPlayerCount;

    private bool isServer = false;
    // Use this for initialization
    void Start()
    {
        //DontDestroyOnLoad(gameObject);
        //MatchMakingManager.Instance.MatchMaker.Connected += OnConnected;
        //MatchMakingManager.Instance.MatchMaker.MatchJoined += OnJoin;
        //MatchMakingManager.Instance.MatchMaker.MatchCreated += OnCreate;
        //MatchMakingManager.Instance.MatchMaker.MatchLeft += OnLeft;
        //MatchMakingManager.Instance.MatchMaker.MatchListGot += OnListGot;

        //AsyncNetworkWorker.Instance.RegisterListener(ServerCommands.Autorize, OnAutorize);
        //AsyncNetworkWorker.Instance.Send(new Command(ServerCommands.Autorize));
        //FindMatch();
    }

    private void OnCreate(object sender, EventArgs e)
    {
        var _responce = e as MatchCreateResponceUNET;
        if (_responce != null)
        {
            if (_responce.Success)
            {
                Status.text = "Created!"; 
            }
            else
            {
                Status.text = "Fail to create the match";
            }
        }
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(5f);
        //if((MatchMakingManager.Instance.MatchMaker as LobbyManager)._playerNumber<2)
        //{
        //    MatchMakingManager.Instance.LeaveMatch();
        //}
    }

    private void OnListGot(object sender, EventArgs e)
    {
        var _responce = e as MatchListResponceUNET;
        if(_responce!=null)
        {
            if (_responce.Success)
            {
                if(_responce.ResponseData.Count>0) Status.text += "Listed";
            }
            else
            {
                Status.text = "Fail to list matches";
            }
        }
    }

    private void OnLeft(object sender, EventArgs e)
    {
        var _responce = e as MatchLeaveResponceUNET;
        if(_responce!=null)
        {
            if(_responce.Success)
            {
                Status.text = "You've left the match";
            }
            else
            {
                Status.text = "Fail to leave the match";
            }
            MatchMakingManager.Instance.ListMatches();
            Invoke("FindMatch", 2);
        }
    }

    private void OnJoin(object sender, EventArgs e)
    {
        var _responce = e as MatchJoinResponceUNET;
        if (_responce != null)
        {
            if (_responce.Success)
            {
                Status.text = "Joined!"; 
            }
            else
            {
                Status.text = "Fail to join the match";
            }
        }
        StartCoroutine(Timer());
    }

    private void OnAutorize(string _s)
    {
        Debug.Log(_s);
    }

    // Update is called once per frame
    void Update()
    {
        //if(MatchMakingManager.Instance!=null)
        //{
        //    PlayerCountText.text = (MatchMakingManager.Instance.MatchMaker as LobbyManager)._playerNumber + "/" + MatchMakingManager.Instance.PlayersToStart;
        //}
        if(Input.GetKeyDown(KeyCode.V))
        {
            
        }
    }

    public void FindMatch()
    {
        //if(InputPlayerCount.text!="")
        //{
        MatchMakingManager.Instance.PlayersToStart = int.Parse(InputPlayerCount.text);
        //    MatchMakingManager.Instance.Create();
        //}
        var _automatch = MatchMakingManager.Instance.Automatch;
        _automatch.Start();
        MatchMakingManager.Instance.MatchMaker.Connected += OnConnected;
    }

    private void OnConnected(object sender, EventArgs e)
    {
        var _responce = e as MatchConnectResponceUNET;
        MatchMakingManager.Instance.MatchMaker.Connected -= OnConnected;
        SceneManager.sceneLoaded += OnSceneLoaded;
        NetworkManager.singleton.ServerChangeScene("NetworkSpaceship");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        //NetworkServer.SpawnObjects();
        Debug.Log("OnSceneLoaded");
        if (MatchMakerUIControllerBase.Instance != null) MatchMakerUIControllerBase.Instance.gameObject.SetActive(false);
    }

    private int GetEqualRoomRank(float _playerRank)
    {
        var _closestRoomRank = 0;
        int _integer = (int)_playerRank;
        _closestRoomRank = Math.Abs(_integer - _playerRank) < 0.51f ? _integer : _integer + 1;
        return _closestRoomRank;
    }

    public void OnStartClick()
    {


        //if (!string.IsNullOrEmpty(InputRank.text)&&!string.IsNullOrEmpty(InputPlayerCount.text))
        //{
            //PlayerRank = int.Parse(InputRank.text);
            //MatchMakingManager.Instance.MatchMaker.SetPlayersCountToStartGame(int.Parse(InputPlayerCount.text));

            //(MatchMakingManager.Instance.MatchMaker.CreateParameters as MatchCreateParametersUNET).EloScoreForMatch = GetEqualRoomRank(PlayerRank);
            //(MatchMakingManager.Instance.MatchMaker.JoinParameters as MatchJoinParametersUNET).EloScoreForClient = GetEqualRoomRank(PlayerRank);

            //InputRank.transform.parent.gameObject.SetActive(false);
            //Status.transform.parent.gameObject.SetActive(true);

            FindMatch();
        //}
       
    }
}
