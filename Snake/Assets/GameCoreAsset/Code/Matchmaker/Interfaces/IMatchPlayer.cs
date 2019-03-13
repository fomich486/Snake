using UnityEngine;

namespace MatchMaker
{
    public interface IMatchPlayer
    {
        //
        // Summary:
        //     ///
        //     The slot within the lobby that this player inhabits.
        //     ///
        byte slot { get; set; }
        //
        // Summary:
        //     ///
        //     This is a flag that control whether this player is ready for the game to begin.
        //     ///
        bool readyToBegin { get; set; }
        Transform transform { get; }

        //
        // Summary:
        //     ///
        //     This is a hook that is invoked on all player objects when entering the lobby.
        //     ///
        void OnClientEnterLobby();
        //
        // Summary:
        //     ///
        //     This is a hook that is invoked on all player objects when exiting the lobby.
        //     ///
         void OnClientExitLobby();
        //
        // Summary:
        //     ///
        //     This is a hook that is invoked on clients when a LobbyPlayer switches between
        //     ready or not ready.
        //     ///
        //
        // Parameters:
        //   readyState:
        //     Whether the player is ready or not.
         void OnClientReady(bool readyState);

         void OnStartClient();
        //
        // Summary:
        //     ///
        //     This removes this player from the lobby.
        //     ///
        void RemovePlayer();
        //
        // Summary:
        //     ///
        //     This is used on clients to tell the server that this player is not ready for
        //     the game to begin.
        //     ///
        void SendNotReadyToBeginMessage();
        //
        // Summary:
        //     ///
        //     This is used on clients to tell the server that this player is ready for the
        //     game to begin.
        //     ///
        void SendReadyToBeginMessage();
        //
        // Summary:
        //     ///
        //     This is used on clients to tell the server that the client has switched from
        //     the lobby to the GameScene and is ready to play.
        //     ///
        void SendSceneLoadedMessage();
        void OnPlayerListChanged(int i);
    }
}