using UnityEngine;
using UnityEngine.Networking.Types;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MatchMaker
{
    [Serializable]
    public class MatchJoinParametersUNET: MonoBehaviour, IMatchJoinParameters, IInspectorparametersDraw
    {
        public ulong NetId { get; set; }
        public string MatchPassword;
        public string PulicClientAddress;
        public string PrivateClientAddress;
        public int EloScoreForClient;
        public int RequestDomain;

        public MatchJoinParametersUNET(NetworkID _netId, string _matchPassword, string _pulicClientAddress, string _privateClientAddress, int _eloScoreForClient, int _requestDomain)
        {
            NetId = (ulong)_netId;
            MatchPassword = _matchPassword;
            PulicClientAddress = _pulicClientAddress;
            PrivateClientAddress = _privateClientAddress;
            EloScoreForClient = _eloScoreForClient;
            RequestDomain = _requestDomain;
        }

        public void DrawInspector()
        {
#if UNITY_EDITOR
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Join match settings", EditorStyles.boldLabel);
            EloScoreForClient = EditorGUILayout.IntField("EloScoreForClient", EloScoreForClient);
            RequestDomain = EditorGUILayout.IntField("RequestDomain", RequestDomain);
            EditorGUILayout.EndVertical();
#endif
        }
    }
}