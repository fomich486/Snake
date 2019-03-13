using System;

namespace MatchMaker
{
    public  class WWWMatchCreateResponce :EventArgs, IMatchCreateResponse
    {
        private string s;

        public WWWMatchCreateResponce(string s)
        {
            this.s = s;
        }
    }
}