using System;

namespace MatchMaker
{
    public class WWWMatchJoinResponce : EventArgs, IMatchJoinResponce
    {
        private string s;

        public WWWMatchJoinResponce(string s)
        {
            this.s = s;
        }
    }
}