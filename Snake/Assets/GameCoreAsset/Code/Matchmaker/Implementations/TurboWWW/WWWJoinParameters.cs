using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace MatchMaker
{
    public class WWWJoinParameters : MonoBehaviour, IMatchJoinParameters
    {
        public string Url;
        /// <summary>
        /// Use it for match join request
        /// </summary>
        public WWWBackEndMatchMaker.GameTypes GameType;

        public string JoinUrl { get { return Url  + "?mode=" + GetGameType(GameType) ; } }

        private string GetGameType(WWWBackEndMatchMaker.GameTypes _type)
        {
            var _resString = "Default";
            switch (_type)
            {
                case WWWBackEndMatchMaker.GameTypes.Normal:
                    _resString = "Normal";
                    break;
                case WWWBackEndMatchMaker.GameTypes.Hardkore:
                    _resString = "Hard";
                    break;
            }
            return _resString;
        }

        public void DrawInspector()
        {
#if UNITY_EDITOR
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Join match settings");
            Url = EditorGUILayout.TextField("Url", Url);
            GameType = (WWWBackEndMatchMaker.GameTypes)EditorGUILayout.EnumPopup("Mode:", GameType);
            EditorGUILayout.TextField("Result url", JoinUrl);

#endif
        }

    }
}