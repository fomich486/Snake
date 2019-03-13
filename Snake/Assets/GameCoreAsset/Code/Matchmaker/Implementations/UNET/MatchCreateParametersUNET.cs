using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MatchMaker
{
    [Serializable]
    public class MatchCreateParametersUNET : MonoBehaviour, IMatchCreateParameters
    {
        public string MatchName;
        public int MatchSize;
        public int MinPlayers;
        public int MaxPlayers;
        public bool MatchAdvertize;
        public string MatchPassword;
        public string PulicClientAddress;
        public string PrivateClientAddress;
        public int EloScoreForMatch;
        public int RequestDomain;

        public MatchCreateParametersUNET(string _matchName, int _matchSize, bool _atchAdvertize, string _matchPassword, string _publicClientAddress, string _privateClientAddress, int _eloScoreForMatch, int _requestDomain)
        {
            MatchName = _matchName;
            MatchSize = _matchSize;
            MatchAdvertize = _atchAdvertize;
            MatchPassword = _matchPassword;
            PulicClientAddress = _publicClientAddress;
            PrivateClientAddress = _privateClientAddress;
            EloScoreForMatch = _eloScoreForMatch;
            RequestDomain = _requestDomain;
        }

        public void DrawInspector()
        {
#if UNITY_EDITOR
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Create match settings", EditorStyles.boldLabel);

            MatchName = EditorGUILayout.TextField("Match name", MatchName);
            PulicClientAddress = EditorGUILayout.TextField("Pulic client address", PulicClientAddress);
            MatchSize = EditorGUILayout.IntField("Match size", MatchSize);
            MinPlayers = EditorGUILayout.IntField("Min Players", MinPlayers);
            MaxPlayers = EditorGUILayout.IntField("Max Players", MaxPlayers);
            MatchAdvertize = EditorGUILayout.Toggle("Match advertize", MatchAdvertize);
            MatchPassword = EditorGUILayout.TextField("Match password", MatchPassword);
            PulicClientAddress = EditorGUILayout.TextField("Pulic Client Address", PulicClientAddress);
            PrivateClientAddress = EditorGUILayout.TextField("Private Client Address", PrivateClientAddress);
            EloScoreForMatch = EditorGUILayout.IntField("Elo Score For Match", EloScoreForMatch);
            RequestDomain = EditorGUILayout.IntField("Request Domain", RequestDomain);

            EditorGUILayout.EndVertical();
#endif
        }
    }
}