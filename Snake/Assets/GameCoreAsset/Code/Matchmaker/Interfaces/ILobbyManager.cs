namespace MatchMaker
{
    public interface ILobbyManager
    {


        ////
        //// Summary:
        ////     ///
        ////     This is the prefab of the player to be created in the PlayScene.
        ////     ///
        //GameObject gamePlayerPrefab { get; set; }
        ////
        //// Summary:
        ////     ///
        ////     This is the prefab of the player to be created in the LobbyScene.
        ////     ///
        //public NetworkLobbyPlayer lobbyPlayerPrefab { get; set; }
        ////
        //// Summary:
        ////     ///
        ////     The minimum number of players required to be ready for the game to start.
        ////     ///
        //public int minPlayers { get; set; }
        ////
        //// Summary:
        ////     ///
        ////     The maximum number of players per connection.
        ////     ///
        //public int maxPlayersPerConnection { get; set; }
        ////
        //// Summary:
        ////     ///
        ////     The maximum number of players allowed in the game.
        ////     ///
        //public int maxPlayers { get; set; }
        ////
        //// Summary:
        ////     ///
        ////     This flag enables display of the default lobby UI.
        ////     ///
        //public bool showLobbyGUI { get; set; }
        ////
        //// Summary:
        ////     ///
        ////     The scene to use for the lobby. This is similar to the offlineScene of the NetworkManager.
        ////     ///
        //public string lobbyScene { get; set; }
        ////
        //// Summary:
        ////     ///
        ////     The scene to use for the playing the game from the lobby. This is similar to
        ////     the onlineScene of the NetworkManager.
        ////     ///
        //public string playScene { get; set; }

        ////
        //// Summary:
        ////     ///
        ////     CheckReadyToBegin checks all of the players in the lobby to see if their readyToBegin
        ////     flag is set.
        ////     ///
        //void CheckReadyToBegin();
        //public override void OnClientConnect(NetworkConnection conn);
        //public override void OnClientDisconnect(NetworkConnection conn);
        //public override void OnClientSceneChanged(NetworkConnection conn);
        ////
        //// Summary:
        ////     ///
        ////     Called on the client when adding a player to the lobby fails.
        ////     ///
        //public virtual void OnLobbyClientAddPlayerFailed();
        ////
        //// Summary:
        ////     ///
        ////     This is called on the client when it connects to server.
        ////     ///
        ////
        //// parameters:
        ////   conn:
        ////     The connection that connected.
        //public virtual void OnLobbyClientConnect(NetworkConnection conn);
        ////
        //// Summary:
        ////     ///
        ////     This is called on the client when disconnected from a server.
        ////     ///
        ////
        //// parameters:
        ////   conn:
        ////     The connection that disconnected.
        //public virtual void OnLobbyClientDisconnect(NetworkConnection conn);
        ////
        //// Summary:
        ////     ///
        ////     This is a hook to allow custom behaviour when the game client enters the lobby.
        ////     ///
        //public virtual void OnLobbyClientEnter();
        ////
        //// Summary:
        ////     ///
        ////     This is a hook to allow custom behaviour when the game client exits the lobby.
        ////     ///
        //public virtual void OnLobbyClientExit();
        ////
        //// Summary:
        ////     ///
        ////     This is called on the client when the client is finished loading a new networked
        ////     scene.
        ////     ///
        ////
        //// parameters:
        ////   conn:
        //public virtual void OnLobbyClientSceneChanged(NetworkConnection conn);
        ////
        //// Summary:
        ////     ///
        ////     This is called on the server when a new client connects to the server.
        ////     ///
        ////
        //// parameters:
        ////   conn:
        ////     The new connection.
        //public virtual void OnLobbyServerConnect(NetworkConnection conn);
        ////
        //// Summary:
        ////     ///
        ////     This allows customization of the creation of the GamePlayer object on the server.
        ////     ///
        ////
        //// parameters:
        ////   conn:
        ////     The connection the player object is for.
        ////
        ////   playerControllerId:
        ////     The controllerId of the player on the connnection.
        ////
        //// Returns:
        ////     ///
        ////     A new GamePlayer object.
        ////     ///
        //public virtual GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId);
        ////
        //// Summary:
        ////     ///
        ////     This allows customization of the creation of the lobby-player object on the server.
        ////     ///
        ////
        //// parameters:
        ////   conn:
        ////     The connection the player object is for.
        ////
        ////   playerControllerId:
        ////     The controllerId of the player.
        ////
        //// Returns:
        ////     ///
        ////     The new lobby-player object.
        ////     ///
        //public virtual GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId);
        ////
        //// Summary:
        ////     ///
        ////     This is called on the server when a client disconnects.
        ////     ///
        ////
        //// parameters:
        ////   conn:
        ////     The connection that disconnected.
        //public virtual void OnLobbyServerDisconnect(NetworkConnection conn);
        ////
        //// Summary:
        ////     ///
        ////     This is called on the server when a player is removed.
        ////     ///
        ////
        //// parameters:
        ////   conn:
        ////
        ////   playerControllerId:
        //public virtual void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId);
        ////
        //// Summary:
        ////     ///
        ////     This is called on the server when all the players in the lobby are ready.
        ////     ///
        //public virtual void OnLobbyServerPlayersReady();
        ////
        //// Summary:
        ////     ///
        ////     This is called on the server when a networked scene finishes loading.
        ////     ///
        ////
        //// parameters:
        ////   sceneName:
        ////     Name of the new scene.
        //public virtual void OnLobbyServerSceneChanged(string sceneName);
        ////
        //// Summary:
        ////     ///
        ////     This is called on the server when it is told that a client has finished switching
        ////     from the lobby scene to a game player scene.
        ////     ///
        ////
        //// parameters:
        ////   lobbyPlayer:
        ////     The lobby player object.
        ////
        ////   gamePlayer:
        ////     The game player object.
        ////
        //// Returns:
        ////     ///
        ////     False to not allow this player to replace the lobby player.
        ////     ///
        //public virtual bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer);
        ////
        //// Summary:
        ////     ///
        ////     This is called on the client when a client is started.
        ////     ///
        ////
        //// parameters:
        ////   lobbyClient:
        //public virtual void OnLobbyStartClient(NetworkClient lobbyClient);
        ////
        //// Summary:
        ////     ///
        ////     This is called on the host when a host is started.
        ////     ///
        //public virtual void OnLobbyStartHost();
        ////
        //// Summary:
        ////     ///
        ////     This is called on the server when the server is started - including when a host
        ////     is started.
        ////     ///
        //public virtual void OnLobbyStartServer();
        ////
        //// Summary:
        ////     ///
        ////     This is called on the client when the client stops.
        ////     ///
        //public virtual void OnLobbyStopClient();
        ////
        //// Summary:
        ////     ///
        ////     This is called on the host when the host is stopped.
        ////     ///
        //public virtual void OnLobbyStopHost();
        //public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId);
        //public override void OnServerConnect(NetworkConnection conn);
        //public override void OnServerDisconnect(NetworkConnection conn);
        //public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player);
        //public override void OnServerSceneChanged(string sceneName);
        //public override void OnStartClient(NetworkClient lobbyClient);
        //public override void OnStartHost();
        //public override void OnStartServer();
        //public override void OnStopClient();
        //public override void OnStopHost();
        ////
        //// Summary:
        ////     ///
        ////     Sends a message to the server to make the game return to the lobby scene.
        ////     ///
        ////
        //// Returns:
        ////     ///
        ////     True if message was sent.
        ////     ///
        //public bool SendReturnToLobby();
        //public override void ServerChangeScene(string sceneName);
        ////
        //// Summary:
        ////     ///
        ////     Calling this causes the server to switch back to the lobby scene.
        ////     ///
        //public void ServerReturnToLobby();
        ////
        //// Summary:
        ////     ///
        ////     This is used on clients to attempt to add a player to the game.
        ////     ///
        //public void TryToAddPlayer();
    }
}