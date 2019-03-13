namespace MatchMaker
{
    public class HostAndPlayController
    {
        public void Host()
        {
            MatchMakingManager.Instance.MatchMaker.CreateMatch();
        }

        public void Join()
        {
            MatchMakingManager.Instance.MatchMaker.JoinMatch();
        }

    }
}