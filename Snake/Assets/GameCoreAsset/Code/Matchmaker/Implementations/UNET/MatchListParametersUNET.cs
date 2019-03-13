using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MatchMaker
{
    public class MatchListParametersUNET :MonoBehaviour, IMatchListParameters
    {
        public int StartPageNumer;
        public int ResultPageSize;
        public string MatchNameFilter;
        public bool FilterOutPrivateMatchesFromResults;
        public int EloScoreTarget;
        public int RequestDomain;

        public MatchListParametersUNET(int _startPageNumer, int _resultPageSize, string _matchNameFilter, bool _filterOutPrivateMatchesFromResults, int _eloScoreTarget, int _requestDomain)
        {
            StartPageNumer = _startPageNumer;
            ResultPageSize = _resultPageSize;
            MatchNameFilter = _matchNameFilter;
            FilterOutPrivateMatchesFromResults = _filterOutPrivateMatchesFromResults;
            EloScoreTarget = _eloScoreTarget;
            RequestDomain = _requestDomain;
        }

        public void DrawInspector()
        {
#if UNITY_EDITOR
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("List match settings", EditorStyles.boldLabel);
            MatchNameFilter = EditorGUILayout.TextField("MatchNameFilter", MatchNameFilter);
            StartPageNumer = EditorGUILayout.IntField("StartPageNumer", StartPageNumer);
            ResultPageSize = EditorGUILayout.IntField("ResultPageSize", ResultPageSize);
            FilterOutPrivateMatchesFromResults = EditorGUILayout.Toggle("FilterOutPrivateMatchesFromResults", FilterOutPrivateMatchesFromResults);
            EloScoreTarget = EditorGUILayout.IntField("EloScoreTarget", EloScoreTarget);
            RequestDomain = EditorGUILayout.IntField("RequestDomain", RequestDomain);
            EditorGUILayout.EndVertical();
#endif
        }
    }


}