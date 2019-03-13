using System;
using System.Collections.Generic;

namespace MatchMaker
{
    public class WWWMatchListResponce:EventArgs,IMatchListResponce
    {
        private string s;
        private List<WWWMatch> matches;
        public WWWMatchListResponce(string s)
        {
            this.s = s;
        }

        public IMatchInfo this[int i]
        {
            get
            {
                return matches[i];
            }
        }

        public int Count
        {
            get
            {
                return matches.Count;
            }
        }
    }
}