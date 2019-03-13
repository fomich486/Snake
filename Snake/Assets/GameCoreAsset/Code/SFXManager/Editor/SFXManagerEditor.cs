using UnityEditor;
using UnityEngine;

namespace SFXManagerModule
{
    /// <summary>
    /// Custom editor for SFXManager
    /// </summary>
    [CustomEditor(typeof(SFXManager))]
    [CanEditMultipleObjects]
    public class SFXManagerEditor : Editor
    {
        /// <summary>
        /// link to manager
        /// </summary>
        private SFXManager manager;

        /// <summary>
        /// called once when user select SFXManager
        /// </summary>
        void OnEnable()
        {
            manager = target as SFXManager;
        } 

        /// <summary>
        /// Drawing GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            //manager options, parameters and other settings
            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField("SFX MANAGER");

                DrawMusicGroup();

                DrawSoundGroup();

                Draw3DSoundGroup();
            }
            EditorGUILayout.EndVertical();

            //begin deep inspector
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Show deep:", GUILayout.Width(80));
                manager.ShowDeepInspector = EditorGUILayout.Toggle(manager.ShowDeepInspector);
            }
            EditorGUILayout.EndHorizontal();
            if (manager.ShowDeepInspector)
                DrawDefaultInspector();
            //end deep inspector
        } 

        /// <summary>
        /// Call it to draw all music group
        /// </summary>
        void DrawMusicGroup()
        {
            EditorGUILayout.BeginVertical("Box");
            { 
                manager.ShowMusicOptions = EditorGUILayout.Foldout(manager.ShowMusicOptions, "Music options");

                if (manager.ShowMusicOptions)
                {
                    //begin source
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Source:", GUILayout.Width(80));
                        manager.MusicSource = (AudioSource)EditorGUILayout.ObjectField(manager.MusicSource, typeof(AudioSource), true);
                    }
                    EditorGUILayout.EndHorizontal();
                    //end source

                    //begin volume group
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Volume:", GUILayout.Width(80));
                        manager.MusicVolume = EditorGUILayout.Slider(manager.MusicVolume, 0f, 1f);
                    }
                    EditorGUILayout.EndHorizontal();
                    //end volume group 

                    //begin pitch group
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Pitch:", GUILayout.Width(80));
                        manager.MusicPitch = EditorGUILayout.Slider(manager.MusicPitch, 0f, 3f);
                    }
                    EditorGUILayout.EndHorizontal();
                    //end pitch group

                    //begin music change time group
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Change time:", GUILayout.Width(80));
                        manager.MusicChangeTime = EditorGUILayout.Slider(manager.MusicChangeTime, 0f, 15f);
                    }
                    EditorGUILayout.EndHorizontal();
                    //end music change time group

                    //begin drawing all themes
                    for (int i = 0; i < manager.MusicThemes.Count; i++)
                    {
                        EditorGUILayout.BeginVertical("Box");
                        {
                            DrawTheme(i);
                        }
                        EditorGUILayout.EndVertical();
                    }
                    //end drawing all themes

                    if (GUILayout.Button("ADD THEME"))
                    {
                        AddNewMusicTheme();
                    }
                }
            }
            EditorGUILayout.EndVertical(); 
        }

        /// <summary>
        /// Call it to add new music theme
        /// </summary>
        void AddNewMusicTheme()
        {
            MusicDictionary _dict = new MusicDictionary();
            manager.MusicThemes.Add(_dict);
        }

        /// <summary>
        /// Call it to remove music theme
        /// </summary>
        /// <param name="_index"></param>
        void RemoveMusicTheme(int _index)
        {
            if(manager.MusicThemes.Count>_index)
                manager.MusicThemes.RemoveAt(_index);
            else
                Debug.LogWarning("There is no " + _index + " index in manager.MusicThemes.Count=" + manager.MusicThemes.Count);
        }

        void DrawTheme(int _index)
        {
            //begin show-hide option
            EditorGUILayout.BeginHorizontal();
            {
                manager.MusicThemes[_index].ShowDict = EditorGUILayout.Foldout(manager.MusicThemes[_index].ShowDict, manager.MusicThemes[_index].Key + " options");
                if(GUILayout.Button("X", GUILayout.Width(20)))
                {
                    RemoveMusicTheme(_index);
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();
            //end show-hide option

            //showing deep options
            if (manager.MusicThemes[_index].ShowDict)
            {
                //begin enum type choosing
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Type:", GUILayout.Width(80));
                    manager.MusicThemes[_index].Key = EditorGUILayout.TextField(manager.MusicThemes[_index].Key);
                }
                EditorGUILayout.EndHorizontal();
                //end enum type choosing
                 
                if (GUILayout.Button("ADD CLIP"))
                {
                    AddNewMusicToTheme(_index);
                }

                //begin list of fields
                for (int j = 0; j < manager.MusicThemes[_index].Value.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(" ", GUILayout.Width(80));

                        manager.MusicThemes[_index].Value[j] = (AudioClip) EditorGUILayout.ObjectField(manager.MusicThemes[_index].Value[j], typeof(AudioClip), false) as AudioClip;

                        if(GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            DeleteMusicFromTheme(_index, j);
                            return;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                //end list of fields
            }
        }

        /// <summary>
        /// Call it to add new music to some theme
        /// </summary>
        /// <param name="_index">Index of theme</param>
        void AddNewMusicToTheme(int _index)
        {
            if (manager.MusicThemes.Count > _index)
                manager.MusicThemes[_index].Value.Add(null);
            else
                Debug.LogWarning("There is no " + _index + " index in manager.MusicThemes.Count=" + manager.MusicThemes.Count);
        }

        /// <summary>
        /// Call it to delete some clip from theme
        /// </summary>
        /// <param name="_themeIndex">Theme index</param>
        /// <param name="_clipIndex">Clip index</param>
        void DeleteMusicFromTheme(int _themeIndex, int _clipIndex)
        {
            if (manager.MusicThemes.Count > _themeIndex)
                if (manager.MusicThemes[_themeIndex].Value.Count > _clipIndex)
                    manager.MusicThemes[_themeIndex].Value.RemoveAt(_clipIndex);
                else
                    Debug.Log("There is no clip with index :" + _clipIndex + " cause manager.MusicThemes[" + _themeIndex + "] count is " + manager.MusicThemes[_themeIndex].Value.Count);
            else
                Debug.LogWarning("There is no " + _themeIndex + " index in manager.MusicThemes.Count=" + manager.MusicThemes.Count);
        } 

        /// <summary>
        /// Call it to draw group of sound options
        /// </summary>
        void DrawSoundGroup()
        {
            EditorGUILayout.BeginVertical("Box");
            {
                manager.ShowSoundOptions = EditorGUILayout.Foldout(manager.ShowSoundOptions, "Sound options");
                if (manager.ShowSoundOptions)
                {
                    //begin source
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Source:", GUILayout.Width(80));
                        manager.SoundSource = (AudioSource)EditorGUILayout.ObjectField(manager.SoundSource, typeof(AudioSource), true);
                    }
                    EditorGUILayout.EndHorizontal();
                    //end source

                    //begin volume group
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Volume:", GUILayout.Width(80));
                        manager.SoundVolume = EditorGUILayout.Slider(manager.SoundVolume, 0f, 1f);
                    }
                    EditorGUILayout.EndHorizontal();
                    //end volume group 

                    //begin pitch group
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Pitch:", GUILayout.Width(80));
                        manager.SoundPitch = EditorGUILayout.Slider(manager.SoundPitch, 0f, 3f);
                    }
                    EditorGUILayout.EndHorizontal();
                    //end pitch group
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Call it to draw 3D sound group options
        /// </summary>
        void Draw3DSoundGroup()
        {
            EditorGUILayout.BeginVertical("Box");
            {
                manager.Show3DSoundOptions = EditorGUILayout.Foldout(manager.Show3DSoundOptions, "Show 3D sound options");
                if (manager.Show3DSoundOptions)
                {
                    if(GUILayout.Button("ADD SOURCE TYPE"))
                    {
                        AddSourceType();
                    }
                    if(GUILayout.Button("REINSTALL ALL SOURCES"))
                    {
                        ReinstallSources();
                    }
                    for(int i = 0; i < manager.AwailableSources.Count; i++)
                    {
                        DrawSourceparameters(i);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Call it to add source type(source group)
        /// </summary>
        void AddSourceType()
        {
            SoundDictionary _dict = new SoundDictionary();
            _dict.ID = GetFreeID();
            _dict.Name = GetFreeName();
            manager.AwailableSources.Add(_dict);
        }

        /// <summary>
        /// CAll it to get free name
        /// </summary>
        /// <returns></returns>
        string GetFreeName()
        {
            string _curName = "default";
            string _baseName = "default";
            int _curIndex = -1;
            bool _found = true;
            while (_found)
            {
                _found = false;
                if(GetIsNameBusy(_curName))
                {
                    _found = true;
                    _curIndex++;
                    _curName = _baseName + _curIndex; 
                }
            }
            return _curName;
        }

        /// <summary>
        /// Call it to get know is name busy
        /// </summary>
        /// <param name="_curName"></param>
        /// <returns></returns>
        bool GetIsNameBusy(string _curName)
        { 
            foreach (SoundDictionary _dict in manager.AwailableSources)
            {
                if (_dict.Name == _curName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Call it to get free ID
        /// </summary>
        /// <returns></returns>
        int GetFreeID()
        {
            int _id = 0;
            bool _found = true;
            while (_found)
            {
                _found = false;
                if (GetIsIDBusy(_id))
                {
                    _found = true;
                    _id++;
                }
            }
            return _id;
        }

        /// <summary>
        /// Call it to get know is ID busy
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        bool GetIsIDBusy(int _id)
        {
            foreach (SoundDictionary _dict in manager.AwailableSources)
            {
                if (_id == _dict.ID)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Call it to remove some source
        /// </summary>
        /// <param name="_index">Index of removing type</param>
        void RemoveSourceType(int _index)
        {
            if (manager.AwailableSources.Count > _index)
                manager.AwailableSources.RemoveAt(_index);
            else
                Debug.LogWarning("There is no index:" + _index + " in maanger.AwailableSources.count: " + manager.AwailableSources.Count);
        }

        /// <summary>
        /// Call it to reinstall all players
        /// </summary>
        void ReinstallSources()
        {
            return;
            /*
            for(int i = 0; i < manager.AwailableSources.Count; i++)
            {
                manager.DestroyAndClearPlayers(i);
                manager.CreatePlayers(i);
            }*/
        }

        /// <summary>
        /// Call it to draw type parameters
        /// </summary>
        /// <param name="_index">Index of drawing type</param>
        void DrawSourceparameters(int _index)
        {
            //begin source parameters group
            EditorGUILayout.BeginVertical("Box");
            {
                //begin foldout 
                EditorGUILayout.BeginHorizontal();
                {
                    manager.AwailableSources[_index].ShowDict = EditorGUILayout.Foldout(manager.AwailableSources[_index].ShowDict, "Show options");
                    if(GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        RemoveSourceType(_index);
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();
                //end foldout

                if (manager.AwailableSources[_index].ShowDict)
                {
                    //NAME
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Name:", GUILayout.Width(80));
                        string _newName = EditorGUILayout.TextField(manager.AwailableSources[_index].Name);
                        if(_newName!= manager.AwailableSources[_index].Name && !GetIsNameBusy(_newName))
                        {
                            manager.AwailableSources[_index].Name = _newName;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    //ID
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("ID:", GUILayout.Width(80));
                        int _newID = EditorGUILayout.IntField(manager.AwailableSources[_index].ID);
                        if(manager.AwailableSources[_index].ID != _newID && !GetIsIDBusy(_newID))
                        {
                            manager.AwailableSources[_index].ID = _newID;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    //TEMPLATE
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Template:", GUILayout.Width(80));
                        Transform _oldTemplate = manager.AwailableSources[_index].Template;
                        Transform _newTemplate = (Transform)EditorGUILayout.ObjectField(manager.AwailableSources[_index].Template, typeof(Transform), false);

                        if (_oldTemplate != _newTemplate)
                        {
                            manager.AwailableSources[_index].Template = _newTemplate;

                            ReinstallTemplates(_index, _newTemplate, manager.AwailableSources[_index].Value.Count);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    //COUNT 
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Count:", GUILayout.Width(80));
                        manager.AwailableSources[_index].Count = EditorGUILayout.IntField(manager.AwailableSources[_index].Count);
                    }
                    EditorGUILayout.EndHorizontal(); 
                }
            }
            EditorGUILayout.EndVertical();
            //end source parameters group
        }

        /// <summary>
        /// Call it to reinstall all 
        /// </summary>
        /// <param name="_index"></param>
        /// <param name="_newTemplate"></param>
        /// <param name="_count"></param>
        void ReinstallTemplates(int _index, Transform _newTemplate, int _count)
        {
            return;
            /*
            manager.AwailableSources[_index].Template = _newTemplate;
            manager.DestroyAndClearPlayers(_index);
            manager.CreatePlayers(_index);  */
        } 
    }
}