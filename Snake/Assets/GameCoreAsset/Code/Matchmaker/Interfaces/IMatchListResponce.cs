using System.Collections;

namespace MatchMaker
{
    public interface IMatchListResponce
    {
        IMatchInfo this[int i] { get;}
        int Count { get; }
    }
}