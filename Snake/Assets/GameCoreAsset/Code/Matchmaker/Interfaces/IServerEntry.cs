using UnityEngine;

namespace MatchMaker
{
    public interface IServerEntry
    {
        void Populate(IMatchInfo _match, IMatchMaker _lobbyManager, Color _c);
        void JoinMatch(IMatchInfo _paramters, IMatchMaker _lobbyManager);
    }
}