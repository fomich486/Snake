using DataSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSeterEditor : EditorWindow
{
    [MenuItem("Tools/Material seter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MaterialSeterEditor));
    }

    private string objectsName;
    private Material Mat;
    private Font someText;

    private string objectSelectName;


    void OnGUI()
    {
        EditorGUILayout.BeginVertical("Box");
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Name of object");
                objectsName = EditorGUILayout.TextField(objectsName);
                Mat = (Material)EditorGUILayout.ObjectField(Mat, typeof(Material),false);
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Replace"))
            {
                ReplaceObjects();
            }

            if (GUILayout.Button("Find"))
            {
                FindObjects();
            }
            EditorGUILayout.BeginHorizontal();
            {
                someText = (Font)EditorGUILayout.ObjectField(someText, typeof(Font), false);
                if (GUILayout.Button("OVERFLOW"))
                {
                    OverflowTexts();
                }
            }
            EditorGUILayout.EndHorizontal();
            if(GUILayout.Button("Fill renders"))
            {
                FillRenders();
            }
        }
        EditorGUILayout.EndVertical();
    } 

    void FillRenders()
    {
        if (Selection.activeGameObject != null)
        {
            ActiveObject _ao = Selection.activeGameObject.GetComponent<ActiveObject>();
            if (_ao != null)
            {
                Log.Write("Found some active object..");
                string _name = "vision_TESTER";
                Renderer[] _rends = _ao.gameObject.GetComponentsInChildren<Renderer>(true);
                Log.Write("Count found:" + _rends.Length);
                List<Renderer> _rendsList = new List<Renderer>();
                foreach(Renderer _r in _rends)
                {
                    Log.Write("_r:" + _r.gameObject.name);
                    if(_r.gameObject.activeSelf && !_r.gameObject.name.Contains(_name))
                    {
                        Log.Write("Adding object:" + _r.gameObject.name);
                        _rendsList.Add(_r);
                    }
                    else if(_r.gameObject.activeSelf && _r.gameObject.name.Contains(_name))
                    {
                        _ao.RendTemplate = _r;
                    }
                }
                _ao.RendsToHide = _rendsList.ToArray();
            }
            else
            {
                Log.Write("There is no selected active object!", LogColors.Red);
            }
        }
        else
        {
            Log.Write("Selection.activeGameObject null!", LogColors.Red);
        }
    }

    void OverflowTexts()
    {
        List<Text> _texts = new List<Text>();
        GameObject[] _gameObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach(GameObject _obj in _gameObjects)
        {
            if (_obj.transform.parent == null)
            {
                Text[] _texts2 = _obj.GetComponentsInChildren<Text>(true);
                foreach(Text _t in _texts2)
                {
                    _texts.Add(_t);
                } 
            }
        }

        foreach(Text _t in _texts)
        {
            if (_t.font == null)
            {
                _t.font = someText;
                Debug.Log(_t.name + " is missed font!");
            }
            if (!_t.resizeTextForBestFit)
            {
                _t.verticalOverflow = VerticalWrapMode.Overflow;
                _t.horizontalOverflow = HorizontalWrapMode.Overflow;
            }
        }
    }

    void FindObjects()
    {

        List<GameObject> _selected = new List<GameObject>();
        GameObject[] _allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject _obj in _allObjects)
        {
            if (_obj.name == objectsName)
            {
                if(!_selected.Contains(_obj))
                    _selected.Add(_obj);
            }
        }

        Selection.objects = _selected.ToArray();
    }

    void ReplaceObjects()
    {
        GameObject[] _allObjects = GameObject.FindObjectsOfType<GameObject>(); 
        foreach(GameObject _obj in _allObjects)
        {
            if(_obj.name == objectsName)
            {
                Renderer _rend = _obj.GetComponent<MeshRenderer>();
                if (_rend != null)
                {
                    _rend.material = Mat;
                }
            }
        }
    }
}