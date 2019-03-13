using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PoolManagerModule
{
    /// <summary>
    /// Custom editor of pool manager
    /// </summary>
    [CustomEditor(typeof(PoolManager))]
    [CanEditMultipleObjects]
    public class PoolManagerEditor : Editor
    {
        /// <summary>
        /// Pool manager instance
        /// </summary>
        PoolManager pool;

        /// <summary>
        /// Called once when user select PoolManager
        /// </summary>
        void OnEnable()
        {
            pool = target as PoolManager; 
        } 

        /// <summary>
        /// Called each frame
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("POOL MANAGER OPTIONS");
            //begin main body 
            EditorGUILayout.BeginVertical("Box");
            {
                pool.ShowTemplates = EditorGUILayout.Foldout(pool.ShowTemplates, "Show templates:");
                if (pool.ShowTemplates)
                {

                    if (GUILayout.Button("ADD NEW TEMPLATE"))
                    {
                        AddNewTemplate();
                    }
                    for (int i = 0; i < pool.Templates.Count; i++)
                    {
                        //begin template body
                        EditorGUILayout.BeginVertical("Box");
                        {
                            //begin foldout body
                            EditorGUILayout.BeginHorizontal();
                            {
                                bool _showValue = EditorGUILayout.Foldout(pool.ShowTemplate[i], pool.TemplateNames[i]);

                                pool.ShowTemplate[i] = _showValue;

                                if (GUILayout.Button("X", GUILayout.Width(20)))
                                {
                                    DeleteTemplate(i);
                                    return;
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            //end foldout body

                            if (pool.ShowTemplate[i])
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    ShowTemplateOptions(i);
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                        EditorGUILayout.EndVertical();
                        //end template body
                    }
                }
            }
            EditorGUILayout.EndVertical();
            //end main body 

            //begin MoreOptions inspector
            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Show more options");
                    pool.MoreOptions = EditorGUILayout.Toggle(pool.MoreOptions);
                }
                EditorGUILayout.EndHorizontal();
                if (pool.MoreOptions)
                    ShowAdditionalControls();
            }
            EditorGUILayout.EndVertical();
            //end deep inspector

            //begin deep options
            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Show deep options");
                    pool.DeepInspector = EditorGUILayout.Toggle(pool.DeepInspector);
                }
                EditorGUILayout.EndHorizontal();
                if (pool.DeepInspector) 
                    DrawDefaultInspector();
            }
            EditorGUILayout.EndVertical();
            //end deep options
        }

        #region CRUD_METHODS

        /// <summary>
        /// Call it to add new template
        /// </summary>
        void AddNewTemplate()
        { 
            pool.ShowTemplate.Add(true);
            pool.Templates.Add(null);
            pool.TemplateNames.Add(GetFreeName());
            pool.TemplateIDs.Add(GetFreeID());
            pool.TemplatesCount.Add(50);
            pool.ProcessParenting.Add(true);
            pool.CanInstantiateNew.Add(true);
            pool.TemplatePreinstall.Add(false);
            
        }

        /// <summary>
        /// Call it to delete template
        /// </summary>
        /// <param name="_index">Index of template</param>
        void DeleteTemplate(int _index)
        {
            if (pool.Templates.Count > _index )
            { 
                if(pool.Templates[_index])
                {
                    Transform _foundParent = pool.GetParentForTemplate(pool.Templates[_index].gameObject.name);
                    if (_foundParent)
                        DestroyImmediate(_foundParent.gameObject);
                }

                pool.ShowTemplate.RemoveAt(_index);
                pool.Templates.RemoveAt(_index);
                pool.TemplateNames.RemoveAt(_index);
                pool.TemplateIDs.RemoveAt(_index);
                pool.TemplatesCount.RemoveAt(_index);
                pool.ProcessParenting.RemoveAt(_index);
                pool.CanInstantiateNew.RemoveAt(_index);
                pool.TemplatePreinstall.RemoveAt(_index); 
            }
        }
        #endregion

        #region POOL_VARIABLES_PROCESSING

        /// <summary>
        /// Call it to generate free ID for pool
        /// </summary>
        /// <returns>Free ID</returns>
        int GetFreeID()
        {
            int _startId = 0;
            while(!IsKeyFree(_startId))
            {
                _startId++;
            }
            return _startId;
        }

        /// <summary>
        /// Call it to chek if ID free
        /// </summary>
        /// <param name="_key">ID to check</param>
        /// <returns>TRUE, if free. FALSE, if not.</returns>
        bool IsKeyFree(int _key)
        {
            if (pool.TemplateIDs.Contains(_key))
                return false;

            return true;
        } 

        /// <summary>
        /// Call it to generate free name
        /// </summary>
        /// <returns>Free name</returns>
        string GetFreeName()
        {
            string _startName = "Default";
            if (!IsNameFree(_startName))
            {
                int _startID = 0;
                while(!IsNameFree(_startName+_startID))
                {
                    _startID++;
                }
                return (_startName + _startID);
            }
            return _startName;
        }

        /// <summary>
        /// Call it to check if name isn't busy
        /// </summary>
        /// <param name="_name">Name to check</param>
        /// <returns>TRUE if free, FALSE if not</returns>
        bool IsNameFree(string _name)
        {
            if (pool.TemplateNames.Contains(_name))
                return false;

            return true;
        }

        #endregion

        /// <summary>
        /// Showing additional controls and default inspector
        /// </summary>
        void ShowAdditionalControls()
        {
            EditorGUILayout.LabelField("Additional controls");

            if(GUILayout.Button("CLEAR ALL PRENSTALLED"))
            {
                ClearAllPreinstalled();
            }

            if(GUILayout.Button("PREINSTALL ALL!"))
            {
                PreinstallAll();
            }

            if(GUILayout.Button("CHECK FOR ISpawnable component"))
            {
                CheckForISpawnableComponent();
            }

            if(GUILayout.Button("UNPARENT ALL"))
            {
                UnparentAll();
            }

            if(GUILayout.Button("PARENT ALL"))
            {
                ParentAll();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        void UnparentAll()
        {
            for(int i = 0; i < pool.ProcessParenting.Count; i++)
            {
                pool.ProcessParenting[i] = false;
            }
        }

        void ParentAll()
        { 
            for (int i = 0; i < pool.ProcessParenting.Count; i++)
            {
                pool.ProcessParenting[i] = true;
            }
        }

        /// <summary>
        /// Call it to clear all objects
        /// </summary>
        void ClearAllPreinstalled()
        {
            for(int i = 0; i < pool.TemplatePreinstall.Count; i++)
            {
                pool.TemplatePreinstall[i] = false;
                ProcessPreinstallChange(i, false);
            }
        }

        /// <summary>
        /// Call it to install all objects
        /// </summary>
        void InstallAll()
        {
            for (int i = 0; i < pool.TemplatePreinstall.Count; i++)
            {
                pool.TemplatePreinstall[i] = true;
                ProcessPreinstallChange(i, true);
            }
        }

        /// <summary>
        /// Call it to chek all objects for ISpawnableObject component
        /// </summary>
        void CheckForISpawnableComponent()
        {
            for(int i = 0; i < pool.Templates.Count; i++)
            {
                if (pool.Templates[i].GetComponent<ISpawnableObject>() == null)
                    Debug.LogError("There is no ISpawnableObject on " + pool.Templates[i].gameObject.name);
            }
        }

        /// <summary>
        /// Call it to delete all objects and install them 
        /// </summary>
        void PreinstallAll()
        {
            ClearAllPreinstalled();
            InstallAll();
        }

        /// <summary>
        /// Call it to show template options
        /// </summary>
        /// <param name="_index"></param>
        void ShowTemplateOptions(int _index)
        {
            if (pool.Templates.Count <= _index)
                return;

            EditorGUILayout.Space();

            //PREFAB SECTION
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Prefab:", GUILayout.Width(80));
                pool.Templates[_index] = EditorGUILayout.ObjectField(pool.Templates[_index], typeof(Transform), false) as Transform;
            }
            EditorGUILayout.EndHorizontal(); 

            //NAME SECTION
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Name:", GUILayout.Width(80));
                string _newName = EditorGUILayout.TextField(pool.TemplateNames[_index]);
                if (_newName != pool.TemplateNames[_index])
                {
                    if (IsNameFree(_newName))
                    {
                        pool.TemplateNames[_index] = _newName;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            //ID SECTION
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("ID:", GUILayout.Width(80));
                int _newId = EditorGUILayout.IntField(pool.TemplateIDs[_index]);
                if (_newId != pool.TemplateIDs[_index])
                {
                    if (IsKeyFree(_newId))
                    {
                        pool.TemplateIDs[_index] = _newId;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            //BASE COUNT
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Count:", GUILayout.Width(80));
                pool.TemplatesCount[_index] = EditorGUILayout.IntField(pool.TemplatesCount[_index]); 
            }
            EditorGUILayout.EndHorizontal();

            //PREINSTALL
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Preinstall:", GUILayout.Width(80));
                bool _newValue = EditorGUILayout.Toggle(pool.TemplatePreinstall[_index]);
                if(_newValue != pool.TemplatePreinstall[_index])
                {
                    pool.TemplatePreinstall[_index] = _newValue;
                    ProcessPreinstallChange(_index, _newValue);
                }
            }
            EditorGUILayout.EndHorizontal();

            //PARENTING
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Parenting:", GUILayout.Width(80));
                pool.ProcessParenting[_index] = EditorGUILayout.Toggle(pool.ProcessParenting[_index]);
            }
            EditorGUILayout.EndHorizontal();

            //INSTANTIATION
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Allow instantiate:", GUILayout.Width(80));
                pool.CanInstantiateNew[_index] = EditorGUILayout.Toggle(pool.CanInstantiateNew[_index]);
            }
            EditorGUILayout.EndHorizontal(); 
        } 

        /// <summary>
        /// Call is to process changing preinstall mode (INSTANTIATES/DESTROYS)
        /// </summary>
        /// <param name="_index"></param>
        /// <param name="_change"></param>
        void ProcessPreinstallChange(int _index, bool _preinstall)
        { 
            if (_preinstall)
            { 
                pool.InstallObjects(_index);
            }
            else
            { 
                SerializedDictionary _dict = pool.FindDictionary(pool.AwailableObjects, pool.Templates[_index]);

                //int _dictIndex = pool.GetIndex(pool.AwailableObjects, pool.Templates[_index]);

                if(_dict != null)
                    pool.ClearDictionary(_dict);
            }
        } 
    }
}