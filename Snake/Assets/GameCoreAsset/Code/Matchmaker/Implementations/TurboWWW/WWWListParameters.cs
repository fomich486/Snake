using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MatchMaker
{
    public class WWWListParameters: MonoBehaviour, IMatchListParameters
    {
        public string Url;
        public string FilterData { get { return "?mode="+GameType.ToString()+"&rank=" + Rank; } }
        public string ListUrl { get { return Url + FilterData; } }
        public float Rank;
        public WWWBackEndMatchMaker.GameTypes GameType;

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
            EditorGUILayout.LabelField("List match settings");
            Rank = EditorGUILayout.FloatField("Rank:", Rank);
            Url = EditorGUILayout.TextField("Create url", Url);
            GameType = (WWWBackEndMatchMaker.GameTypes)EditorGUILayout.EnumPopup("Mode:", GameType);          
            EditorGUILayout.TextField("Result url", ListUrl);

#endif
        }
    }
}