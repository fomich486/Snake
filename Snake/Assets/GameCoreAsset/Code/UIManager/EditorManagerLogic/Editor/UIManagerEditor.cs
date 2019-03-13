using SFXManagerModule; 
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UISystem
{
    /// <summary>
    /// Custom editor for UIManager
    /// </summary>
    [CustomEditor(typeof(UIManager))]
    public class UIManagerEditor : Editor
    {
        #region VARIABLES
        /// <summary>
        /// Cashed SFXManager instance
        /// </summary>
        private SFXManager sfxManager;
        /// <summary>
        /// Cashed UIManager
        /// </summary>
        private UIManager manager; 
        /// <summary>
        /// /
        /// </summary>
        private string[] musicThemeNames;
        #endregion

        #region METHODS
        /// <summary>
        /// Called when you click on UImanager
        /// </summary>
        void OnEnable()
        {
            manager = target as UIManager;
            InstallRelations();
            InstallVariables();
        }

        /// <summary>
        /// Call it to t install all variables
        /// </summary>
        void InstallVariables()
        {
            sfxManager = GameObject.FindObjectOfType<SFXManager>();
            if (sfxManager != null)
            {
                List<string> _names = new List<string>();
                foreach (MusicDictionary _theme in sfxManager.MusicThemes)
                {
                    if (_theme != null)
                    {
                        _names.Add(_theme.Key);
                    }
                }
                musicThemeNames = _names.ToArray();
            }
            else
            {
                Debug.Log("There is no SFXManager on scene. Fix it!");
            }
        }

        /// <summary>
        /// Call it to update all relations in pages
        /// </summary>
        void InstallRelations()
        {
            //fill available relations data
            List<RelationshipData> _relations = new List<RelationshipData>();

            foreach (UIPageData _data in manager.Pages)
            {
                if (_data == null)
                {
                    Debug.LogWarning("There is null UIPageData. Check it out!");
                }
                else
                { 
                    RelationshipData _rel = new RelationshipData();
                    _rel.PageName = _data.Name;
                    _rel.RelationType = PagesRelations.Ignore;
                    _relations.Add(_rel);
                }
            }

            //updating it in each page data
            foreach (UIPageData _data in manager.Pages)
            {
                if (_data != null)
                {
                    _data.UpdateRelations(_relations);
                }
                else
                {
                    Debug.LogWarning("There is null UIPageData. Check it out!");
                }
            }
        }

        /// <summary>
        /// Called each update of editor
        /// </summary>
        public override void OnInspectorGUI()
        {
            //begin total group
            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField("UI MANAGER");

                if (GUILayout.Button("add"))
                {
                    AddNewPage();
                }

                //BEGIN ALL PAGES
                EditorGUILayout.BeginVertical("Box");
                {
                    for (int i = 0; i < manager.Pages.Count; i++)
                    {
                        //BEGIN CURRENT PAGE 
                        EditorGUILayout.BeginHorizontal("Box");
                        {
                            //begin arrows
                            EditorGUILayout.BeginVertical();
                            {
                                if (GUILayout.Button("↑", GUILayout.Width(18)))
                                {
                                    PushPageUp(i);
                                }
                                if (GUILayout.Button("↓", GUILayout.Width(18)))
                                {
                                    PushPageDown(i);
                                }
                            }
                            EditorGUILayout.EndVertical();
                            //end arrows

                            EditorGUILayout.LabelField(" ", GUILayout.Width(7));

                            //BEGIN PAGE OPTIONS
                            EditorGUILayout.BeginVertical();
                            {
                                //BEGIN PREFAB GROUP
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("Prefab:", GUILayout.Width(100));
                                    UIPageData _oldData = manager.Pages[i];
                                    UIPageData _newData = EditorGUILayout.ObjectField(manager.Pages[i], typeof(UIPageData), true) as UIPageData;
                                    if (_oldData != _newData)
                                    {
                                        manager.Pages[i] = _newData;
                                        if (_newData != null && _newData.Name == "")
                                        {
                                            _newData.Name = _newData.gameObject.name;
                                            InstallRelations();
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                                //END PREFAB GROUP

                                //BEGIN PAGE NAME AND OPTIONS
                                if (manager.Pages[i] != null)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        manager.Pages[i].ShowInfo = EditorGUILayout.Foldout(manager.Pages[i].ShowInfo, manager.Pages[i].Name + " options");
                                        string _newName = EditorGUILayout.TextField(manager.Pages[i].Name, GUILayout.Width(Screen.width - 220));
                                        if (manager.Pages[i].Name != _newName)
                                        {
                                            manager.Pages[i].Name = _newName;
                                            InstallRelations();
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    if (manager.Pages[i].ShowInfo)
                                    {
                                        DrawPageOptions(i);
                                    }
                                }
                                else
                                {
                                    EditorGUILayout.LabelField("Empty");
                                }
                                //END PAGE NAME AND OPTIONS
                            }
                            EditorGUILayout.EndVertical();
                            //END PAGE OPTIONS

                            //BEGIN DELETE BUTTON
                            if (GUILayout.Button("X", GUILayout.Width(40), GUILayout.Height(40)))
                            {
                                DeletePage(i);
                                return;
                            }
                            //END DELETE BUTTON
                        }
                        EditorGUILayout.EndHorizontal();
                        //END CURRENT PAGE
                    }
                }
                EditorGUILayout.EndVertical();
                //END ALL PAGES
            }
            EditorGUILayout.EndVertical();
            //end total group 
        }

        /// <summary>
        /// Call it to move page up in queue
        /// </summary>
        /// <param name="_index"></param>
        void PushPageUp(int _index)
        {
            if (_index > 0 && manager.Pages.Count > 1)
            {
                UIPageData _data = manager.Pages[_index];
                manager.Pages[_index] = manager.Pages[_index - 1];
                manager.Pages[_index - 1] = _data;
            }
        }

        /// <summary>
        /// Call it to move page down in queue
        /// </summary>
        /// <param name="_index"></param>
        void PushPageDown(int _index)
        {
            if (_index < manager.Pages.Count - 1 && manager.Pages.Count > 1)
            {

                UIPageData _data = manager.Pages[_index];
                manager.Pages[_index] = manager.Pages[_index + 1];
                manager.Pages[_index + 1] = _data;
            }
        }

        /// <summary>
        /// Call it to draw page options
        /// </summary>
        /// <param name="_index"></param>
        void DrawPageOptions(int _index)
        {
            if (manager.Pages.Count > _index)
            {
                EditorGUILayout.BeginVertical("Box");
                {
                    for (int i = 0; i < manager.Pages[_index].Relations.Count; i++)
                    {
                        if (manager.Pages[_index].Name != manager.Pages[_index].Relations[i].PageName)
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(manager.Pages[_index].Relations[i].PageName, GUILayout.Width(100));
                                manager.Pages[_index].Relations[i].RelationType = (PagesRelations)EditorGUILayout.EnumPopup(manager.Pages[_index].Relations[i].RelationType);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
                EditorGUILayout.EndVertical();

                //Begin animation group
                EditorGUILayout.BeginVertical("Box");
                {
                    EditorGUILayout.LabelField("Animation group:");

                    //BEGIN DEPENDS REAL TIME
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Depends on real time:", GUILayout.Width(100));
                        manager.Pages[_index].DependsOnReal = EditorGUILayout.Toggle(manager.Pages[_index].DependsOnReal);
                    }
                    EditorGUILayout.EndHorizontal();
                    //END DEPENDS REAL TIME

                    //BEGIN FADE IN TIME
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Fade in time:", GUILayout.Width(100));
                        manager.Pages[_index].FadeInTime = EditorGUILayout.FloatField(manager.Pages[_index].FadeInTime);
                    }
                    EditorGUILayout.EndHorizontal();
                    //END FADE IN TIME

                    //BEGIN FADE OUT TIME
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Fade out time:", GUILayout.Width(100));
                        manager.Pages[_index].FadeOutTime = EditorGUILayout.FloatField(manager.Pages[_index].FadeOutTime);
                    }
                    EditorGUILayout.EndHorizontal();
                    //END FADE OUT TIME

                    //BEGIN FADE IN ANIMATION
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Fade in animation:", GUILayout.Width(100));
                        manager.Pages[_index].ShowAnimation = (AnimationClip)EditorGUILayout.ObjectField(manager.Pages[_index].ShowAnimation, typeof(AnimationClip), false);
                    }
                    EditorGUILayout.EndHorizontal();
                    //END FADE IN ANIMATIOn

                    //BEGIN FADE OUT ANIMATION
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Fade out animation:", GUILayout.Width(100));
                        manager.Pages[_index].HideAnimation = (AnimationClip)EditorGUILayout.ObjectField(manager.Pages[_index].HideAnimation, typeof(AnimationClip), false);
                    }
                    EditorGUILayout.EndHorizontal();
                    //END FADE OUT ANIMATIOn

                }
                EditorGUILayout.EndVertical();
                //end animation group 


                //begin callbacks group
                EditorGUILayout.BeginVertical();
                {
                    //begin call in start
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Call in the start:", GUILayout.Width(100));
                        manager.Pages[_index].CalledOnStart = EditorGUILayout.Toggle(manager.Pages[_index].CalledOnStart);
                    }
                    EditorGUILayout.EndHorizontal();
                    //end call in start

                    //begin call in start
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Separate:", GUILayout.Width(100));
                        manager.Pages[_index].SeparatePage = EditorGUILayout.Toggle(manager.Pages[_index].SeparatePage);
                    }
                    EditorGUILayout.EndHorizontal();
                    //end call in start

                    //GETTING SERIALIZED OBJECT
                    SerializedObject _obj = new SerializedObject(manager.Pages[_index]);

                    //on show property
                    SerializedProperty _prop = _obj.FindProperty("OnShowEvent");
                    if (_prop != null)
                        EditorGUILayout.PropertyField(_prop, true);

                    //on hide property
                    _prop = _obj.FindProperty("OnHideEvent");
                    if (_prop != null)
                        EditorGUILayout.PropertyField(_prop, true);
                }
                EditorGUILayout.EndVertical();
                //end callbacks group

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Music theme:", GUILayout.Width(100));
                    if (musicThemeNames == null || musicThemeNames.Length == 0)
                    {
                        EditorGUILayout.HelpBox("Add at least one music theme", MessageType.Warning);
                        //EditorGUILayout.LabelField("add at least one music theme");
                    }
                    else
                    {
                        int _oldIndex = GetIndexByName(manager.Pages[_index].MusicTheme);
                        if (_oldIndex <= -1 && musicThemeNames != null && musicThemeNames.Length > 0)
                        {
                            _oldIndex = 0;
                            manager.Pages[_index].MusicTheme = musicThemeNames[0];
                        }
                        int _newIndex = EditorGUILayout.Popup(_oldIndex, musicThemeNames);
                        if (_oldIndex != _newIndex)
                        {
                            manager.Pages[_index].MusicTheme = musicThemeNames[_newIndex];
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Call it to get of music dictionary by it's name
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        int GetIndexByName(string _name)
        {
            if (sfxManager == null)
                return -1;

            int i = 0;
            foreach (MusicDictionary _dict in sfxManager.MusicThemes)
            {
                if (_dict != null)
                {
                    if (_name == _dict.Key)
                    {
                        return i;
                    }
                }
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Call it to get name of musci dict by it's index 
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        string SetNameByIndex(int _index)
        {
            if (sfxManager == null)
                return "";

            if (sfxManager.MusicThemes.Count > _index)
            {
                return sfxManager.MusicThemes[_index].Key;
            }
            return "";
        }

        /// <summary>
        /// Call it to add new page
        /// </summary>
        void AddNewPage()
        {
            manager.Pages.Add(null);
        }

        /// <summary>
        /// Call it to delete page
        /// </summary>
        /// <param name="_index"></param>
        void DeletePage(int _index)
        {
            if (manager.Pages.Count > _index)
            {
                manager.Pages.RemoveAt(_index);
            }
        }
        #endregion
    }
}
