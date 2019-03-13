using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace MatchMaker
{
    public class WWWCreateParameters : MonoBehaviour, IMatchCreateParameters
    {
        public string Url;

        public WWWBackEndMatchMaker.GameTypes GameType;

        /// <summary>
        /// Use it for match creation request
        /// </summary>
        public string CreationUrl { get { return Url + "?mode="+GetGameType(GameType); } }


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
            EditorGUILayout.LabelField("Create match settings");
            Url = EditorGUILayout.TextField("Create url", Url);
            GameType = (WWWBackEndMatchMaker.GameTypes)EditorGUILayout.EnumPopup("Mode:", GameType);
            EditorGUILayout.TextField("Result url", CreationUrl);

#endif
        }
    }
}