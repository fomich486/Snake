using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MatchMaker
{
    [CustomEditor(typeof(MatchMakingManager))]
    [CanEditMultipleObjects]
    public class MatchMakerEditor : Editor
    {
        private MatchMakingManager controller;
        private void Awake()
        {
            controller = target as MatchMakingManager;
        }
        public override void OnInspectorGUI()
        {
            controller.MatchMakerGO = EditorGUILayout.ObjectField("MatchMaker", controller.MatchMakerGO, typeof(GameObject), true) as GameObject;
            controller.MatchMakerUIPrefab = EditorGUILayout.ObjectField("MatchMakerUI", controller.MatchMakerUIPrefab, typeof(GameObject), true) as GameObject;
            if(controller.MatchMakerUIPrefab!=null)
            {
                controller.ShowUI = EditorGUILayout.Toggle("Show UI", controller.ShowUI);
            }
            if (controller.MatchMakerGO != null)
            {
                controller.MatchMaker = controller.MatchMakerGO.GetComponent<IMatchMaker>();
                controller.Mode  = (MatchMakingManager.MatchMakerModes)EditorGUILayout.EnumPopup("Mode:", controller.Mode);
                controller.PlayersToStart = EditorGUILayout.IntField("Players count:", controller.PlayersToStart);
            }

            if (controller.MatchMaker!=null)
            {
                if (controller.MatchMaker.CreateParameters == null)
                {
                    controller.MatchMaker.Init();
                }
                var settings = new List<IInspectorparametersDraw>() { controller.MatchMaker.CreateParameters, controller.MatchMaker.JoinParameters, controller.MatchMaker.ListParameters, controller.MatchMaker.LeaveParameters, controller.MatchMaker.StartParameters};
                foreach (var setting in settings)
                {
                    if (setting != null)
                    {
                        setting.DrawInspector();
                    }
                }
            }
        }      
    }
}

