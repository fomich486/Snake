using System;
using UnityEngine.Networking.Types;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MatchMaker
{
    [Serializable]
    public class MatchLeaveParametersUNET: MonoBehaviour, IMatchLeaveParameters, IInspectorparametersDraw
    {
        public ulong NetId { get; set; }
        public NodeID NodeId { get; set; }
        public int RequestDomain;

        public MatchLeaveParametersUNET(NetworkID networkId, NodeID nodeId, int domain)
        {
            NetId = (ulong)networkId;
            NodeId = nodeId;
            RequestDomain = domain;
        }

        public void DrawInspector()
        {
#if UNITY_EDITOR
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Leave match settings", EditorStyles.boldLabel);       
            RequestDomain = EditorGUILayout.IntField("RequestDomain", RequestDomain);
            EditorGUILayout.EndVertical();
#endif
        }
    }
}