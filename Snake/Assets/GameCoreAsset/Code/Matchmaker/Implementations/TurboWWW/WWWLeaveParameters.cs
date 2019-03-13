using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace MatchMaker
{
    public class WWWLeaveParameters: MonoBehaviour, IMatchLeaveParameters
    {
        public string Url;
        public string Token;
        public string LeaveUrl { get { return Url + "?token="+Token; } }

        public void DrawInspector()
        {
#if UNITY_EDITOR
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("List match settings");           
            Url = EditorGUILayout.TextField("Create url", Url);
            EditorGUILayout.TextField("Result url", LeaveUrl);

#endif
        }
    }
}