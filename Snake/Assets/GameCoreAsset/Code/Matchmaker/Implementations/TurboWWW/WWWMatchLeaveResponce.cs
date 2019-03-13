using System;

namespace MatchMaker
{
    public class WWWMatchLeaveResponce:EventArgs,IMatchLeaveResponce
    {
        private string s;

        public WWWMatchLeaveResponce(string s)
        {
            this.s = s;
        }
    }
}