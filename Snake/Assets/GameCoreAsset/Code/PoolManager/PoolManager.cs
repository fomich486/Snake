using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PoolManagerModule
{
    /// <summary>
    /// Class of PoolManager, need for optimized controlling of objects by Spawn-Despawn functions without expencive Instantiate/Destroy methods
    /// </summary>
    public class PoolManager : Singleton<PoolManager>
    {
        #region VARIABLES 
        /// <summary>
        /// List of templates(prefabs)
        /// </summary>
        public List<Transform> Templates = new List<Transform>();

        /// <summary>
        /// List of base count of templates. If seted 10 - there were be 10 objects 100%
        /// </summary>
        public List<int> TemplatesCount = new List<int>();

        /// <summary>
        /// Names of templates. Need to access to some template via some string
        /// </summary>
        public List<string> TemplateNames = new List<string>();

        /// <summary>
        /// IDs of templates. Need to access to some template via some int ID
        /// </summary>
        public List<int> TemplateIDs = new List<int>();

        /// <summary>
        /// Useful for EDIT_MODE. All objects will be sorted by parenting to their parents (SetParent) ->clear Hierarchy
        /// </summary>
        public List<bool> ProcessParenting = new List<bool>();

        /// <summary>
        /// Can PoolManager instantiate additional objects when called Spawn if there is no awailable object to spawn
        /// </summary>
        public List<bool> CanInstantiateNew = new List<bool>();

        /// <summary>
        /// Install all objects of template before play mode or not
        /// </summary>
        public List<bool> TemplatePreinstall = new List<bool>();

        /// <summary>
        /// private awailable objects
        /// </summary>
        [SerializeField]
        private LinkedList<SerializedDictionary> awailableObjects;

        /// <summary>
        /// Dictionaries that contained despawned objects(ready to spawn)
        /// </summary>
        public LinkedList<SerializedDictionary> AwailableObjects
        {
            get
            {
                if (awailableObjects == null)
                    awailableObjects = new LinkedList<SerializedDictionary>();

                return awailableObjects;
            }
            set
            {
                awailableObjects = value;
            }
        }

        /// <summary>
        /// private unawailable objects
        /// </summary>
        [SerializeField]
        private LinkedList<SerializedDictionary> unawailableObjects;

        /// <summary>
        /// Dictionaries, that contain spawned objects.
        /// </summary>
        public LinkedList<SerializedDictionary> UnawailableObjects
        {
            get
            {
                if (unawailableObjects == null)
                    unawailableObjects = new LinkedList<SerializedDictionary>();

                return unawailableObjects;
            }
            set
            {
                unawailableObjects = value;
            }
        }
        #endregion

        #region EDITOR_VARIABLES
        /// <summary>
        /// List of flags for editor to show/hide each template parameters
        /// </summary>
        public List<bool> ShowTemplate = new List<bool>();

        /// <summary>
        /// Flag of showing/hiding more options
        /// </summary>
        public bool MoreOptions = false;

        /// <summary>
        /// Flag if we have to show all templates
        /// </summary>
        public bool ShowTemplates = true;

        /// <summary>
        /// Flag of deep inspector(default class variables showin)
        /// </summary>
        public bool DeepInspector = false;
        #endregion

        #region INITIALIZATION_METHODS 

        /// <summary>
        /// You need to call this firstly, cause game should start with all objects
        /// </summary>
        private void Awake()
        {
            base.Awake();
            for (int i = 0; i < TemplatePreinstall.Count; i++)
            {
                if (!TemplatePreinstall[i])
                {
                    InstallObjects(i);
                }
            }
        }

        /// <summary>
        /// Call this function to generate objects in pool customly
        /// </summary>
        /// <param name="_template"></param>
        /// <param name="_count"></param>
        public void AddToPool(Transform _template, int _count)
        {
            if (_template == null)
            {
                Log.Write("Template is null!",LogColors.Yellow);
                return;
            }

            Templates.Add(_template);
            TemplatesCount.Add(_count);
            ShowTemplate.Add(true);
            TemplateNames.Add(GetFreeName());
            TemplateIDs.Add(GetFreeID());
            ProcessParenting.Add(false);
            CanInstantiateNew.Add(false);
            TemplatePreinstall.Add(false);
            InstallObjects(Templates.Count - 1);
        }

        /// <summary>
        /// Call it to genereate new free name
        /// </summary>
        /// <returns></returns>
        string GetFreeName()
        {
            string _startName = "Default";
            if (!IsNameFree(_startName))
            {
                int _startID = 0;
                while (!IsNameFree(_startName + _startID))
                {
                    _startID++;
                }
                return (_startName + _startID);
            }
            return _startName;
        }

        /// <summary>
        /// Call it to get know is name already busy
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        bool IsNameFree(string _name)
        {
            if (TemplateNames.Contains(_name))
                return false;

            return true;
        }

        /// <summary>
        /// Call it to generate free ID for pool
        /// </summary>
        /// <returns>Free ID</returns>
        int GetFreeID()
        {
            int _startId = 0;
            while (!IsKeyFree(_startId))
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
            if (TemplateIDs.Contains(_key))
                return false;

            return true;
        }

        /// <summary>
        /// DO NOT USE IT YOURSELF! ONLY EDITOR HAVE TO CALL IT!
        /// Destroys all object, clearing all lists 
        /// </summary>
        public void ClearAllPools()
        {
            ClearDictionaries(AwailableObjects);
            ClearDictionaries(UnawailableObjects);
        }

        /// <summary>
        /// Call it to destroy all instances and clear dictionaries
        /// </summary>
        /// <param name="_dictList"></param>
        public void ClearDictionaries(LinkedList<SerializedDictionary> _dictList)
        {
            foreach (SerializedDictionary _dict in _dictList)
            {
                ClearDictionary(_dict);
            }
            _dictList.Clear();
        }

        /// <summary>
        /// Call it to clear some dictionary
        /// </summary>
        /// <param name="_dict"></param>
        public void ClearDictionary(SerializedDictionary _dict)
        {
            foreach (Transform _obj in _dict.Value)
            {
                if (_obj)
                {
                    DestroyImmediate(_obj.gameObject);
                }
            }
            _dict.ClearAll();
        }

        /// <summary>
        /// DO NOT USE IT YOURSELF! ONLY EDITOR HAVE TO CALL IT
        /// Instantiates all objects and put them to their dictionaries
        /// </summary>
        public void CreateAllPools()
        {
            for (int i = 0; i < Templates.Count; i++)
            {
                InstallObjects(i);
            }
        }

        /// <summary>
        /// Call it to get index of some tempalate in list
        /// </summary>
        /// <param name="_list">Some list</param>
        /// <param name="_key">Some key</param>
        /// <returns>Index in list of template in list or -1, if didnt found</returns>
        public int GetIndex(List<SerializedDictionary> _list, Transform _key)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].Key == _key)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Call it to get parent for spawned objects in pool(preinstall place)
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        public Transform GetParentForTemplate(string _name)
        {
            foreach (Transform _child in transform)
            {
                if (_child && transform != _child)
                {
                    if (_child.gameObject.name == _name)
                    {
                        return _child;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Call it to install objects of some template
        /// </summary>
        /// <param name="_index">Index of template</param>
        public void InstallObjects(int _index)
        {
            //if template not null
            if (Templates.Count > _index && Templates[_index])
            {
                //if parenting turned on - it will clear scene by parenting objects to their parent and back. 
                //For release versions - TURN THIS OFF, cause SetParent is very expensive operation
                GameObject _parent = null;
                if (ProcessParenting[_index])
                {
                    Transform _foundChild = GetParentForTemplate(Templates[_index].name);
                    if (_foundChild)
                    {
                        _parent = _foundChild.gameObject;
                    }
                    else
                    {
                        GameObject _poolTemplateParent = new GameObject();
                        _poolTemplateParent.name = Templates[_index].name;
                        _poolTemplateParent.transform.parent = transform;
                        _parent = _poolTemplateParent;
                    }
                }

                //check if we already have unawailable dict of that template
                bool _haveToAddUnawailable = true;
                SerializedDictionary _selectedUn = FindDictionary(UnawailableObjects, Templates[_index]);
                //int _unwailableIndex = GetIndex(UnawailableObjects, Templates[_index]);
                if (_selectedUn != null)
                {
                    _haveToAddUnawailable = false;
                    if (_selectedUn.Value.Count > 0)
                    {
                        Log.Write("You have firstly remove dictionary with the same template before installing objects!" + TemplateNames[_index],LogColors.Yellow);
                    }
                }
                //if true - we have to add dictionary
                if (_haveToAddUnawailable)
                {
                    //creation empty dict
                    SerializedDictionary _dictUnawailable = new SerializedDictionary(Templates[_index]);
                    UnawailableObjects.AddLast(_dictUnawailable);
                }

                //check if we already have awailable dict of that template
                bool _haveToAddAwailable = true;
                bool _haveToAddAwailableObject = true;
                //int _awailableIndex = GetIndex(AwailableObjects, Templates[_index]);
                SerializedDictionary _selectedDict = FindDictionary(AwailableObjects, Templates[_index]);

                //if found index
                if (_selectedDict != null)
                {
                    _haveToAddAwailable = false;
                    if (_selectedDict.Value.Count > 0)
                    {
                        Log.Write("You have firstly remove ditionary with the same template before installing objets!" + TemplateNames[_index],LogColors.Yellow);
                    }
                } 

                if (_haveToAddAwailableObject)
                {
                    //if we have empty awailable dict without objects - delete it
                    if (!_haveToAddAwailable)
                    {
                        AwailableObjects.Remove(_selectedDict); 
                    }

                    //creation dict and instancing new objects here
                    SerializedDictionary _dict = new SerializedDictionary(Templates[_index]);
                    for (int j = 0; j < TemplatesCount[_index]; j++)
                    {
                        Transform _newObject = Instantiate(Templates[_index], transform.position, transform.rotation) as Transform;
                        ISpawnableObject _activeObject = _newObject.GetComponent<ISpawnableObject>();

                        if (ProcessParenting[_index])
                            _newObject.parent = _parent.transform;

                        if (_activeObject != null)
                        {
                            _activeObject.SetInstalled();
                            _activeObject.SetTemplate(Templates[_index]);
                        }
                        else
                        {
                            Log.Write("You have added object to pull without ActiveObject component. This may reduce your prefomance!",LogColors.Yellow);
                        }

                        _newObject.gameObject.SetActive(false);

                        _dict.AddObject(_newObject);
                    }
                    AwailableObjects.AddLast(_dict);
                }
            }
            else
            {
                Debug.LogError("You havent seted Template index:" + _index + " in pool manager. Check this out!");
            }
        }
        #endregion


        public SerializedDictionary FindDictionary(LinkedList<SerializedDictionary> _array, Transform _key)
        {
            foreach (SerializedDictionary _dict in _array)
            {
                if (_dict != null && _dict.Key == _key)
                {
                    return _dict;
                }
            }
            return null;
        }

        /// <summary>
        /// Call it to add object to unawailable list 
        /// </summary>
        /// <param name="_obj">Object to move</param>
        /// <param name="_template">Template(prefab) of object</param>
        void AddToUnawailable(Transform _obj, Transform _template)
        {
            foreach (SerializedDictionary _dict in UnawailableObjects)
            {
                if (_dict != null && _dict.Key == _template)
                {
                    _dict.AddObject(_obj);
                }
            }
        }

        #region SPAWN_TRANSFORM_TEMPLATE_METHODS
        /// <summary>
        /// Call it to spawn some object
        /// </summary>
        /// <param name="_template">Template(prefab) ob object</param>
        /// <param name="_position">Position, where object have to be spawned</param>
        /// <param name="_rotation">Rotation of object</param>
        /// <param name="_parent">Parent of object</param>
        /// <returns>Object, that were spawned for you</returns>
        public Transform Spawn(Transform _template, Vector3 _position, Quaternion _rotation, Transform _parent)
        {
            Transform _spawned = Spawn(_template, _position, _rotation) as Transform;
            if (_spawned)
                _spawned.parent = _parent;

            return _spawned;
        }

        /// <summary>
        /// Call it to spawn object
        /// </summary>
        /// <param name="_template">Template(prefab)</param>
        /// <param name="_position">Position of spawned object in the world</param>
        /// <param name="_rotation">Rotation of spawned object in the world</param>
        /// <returns></returns>
        public Transform Spawn(Transform _template, Vector3 _position, Quaternion _rotation)
        {
            //if our pool containts this template
            if (Templates.Contains(_template))
            {
                int _index = 0;

                //looking for needable dict
                foreach (SerializedDictionary _dict in AwailableObjects)
                {
                    //it's it. check for awailable object
                    if (_dict != null && _dict.Key == _template)
                    {
                        //if we have free elements
                        if (_dict.Value.Count > 0)
                        {
                            //returning selected, applying parameters
                            Transform _selectedObject = _dict.GetFirst();

                            //already in unawailable - this is necessary
                            AddToUnawailable(_selectedObject, _dict.Key);

                            _selectedObject.gameObject.SetActive(true);
                            _selectedObject.position = _position;
                            _selectedObject.rotation = _rotation;

                            if (ProcessParenting[_index])
                                _selectedObject.parent = null;


                            return _selectedObject;
                        }
                        else
                        {
                            //instantiate new and add to dict
                            if (CanInstantiateNew[_index])
                            {
                                Transform _newObject = Instantiate(_dict.Key, _position, _rotation) as Transform;
                                ISpawnableObject _activeObject = _newObject.GetComponent<ISpawnableObject>();

                                if (ProcessParenting[_index])
                                    _newObject.parent = null;

                                if (_activeObject != null)
                                {
                                    _activeObject.SetTemplate(_dict.Key);
                                }
                                else
                                {
                                    Log.Write("You have added object to pull without ActiveObject component. This may reduce your prefomance!",LogColors.Yellow);
                                }
                                AddToUnawailable(_newObject, _dict.Key);
                                return _newObject;
                            }
                        }
                    }
                    _index++;
                }
            }
            return null;
        }

        /// <summary>
        /// Call it to spawn object
        /// </summary>
        /// <param name="_template">template(prefab) of object</param>
        /// <param name="_position">Position of spawned object in the world</param>
        /// <returns></returns>
        public Transform Spawn(Transform _template, Vector3 _position)
        {
            return Spawn(_template, _position, Quaternion.identity, null);
        }

        /// <summary>
        /// Call it to spawn object
        /// </summary>
        /// <param name="_template">Object template(prefab)</param>
        /// <returns>Spawned object to perform</returns>
        public Transform Spawn(Transform _template)
        {
            return Spawn(_template, Vector3.zero, Quaternion.identity, null);
        }

        #endregion

        #region SPAWN_NAME_METHODS
        /// <summary>
        /// Call this to spawn object by it's template name
        /// </summary>
        /// <param name="_templateName">Name of template</param>
        /// <param name="_position">Position of spawned object</param>
        /// <param name="_rotation">Rotation of spawned object</param>
        /// <param name="_parent">Parent of spawned object</param>
        /// <returns>Instance of spawned object</returns>
        public Transform Spawn(string _templateName, Vector3 _position, Quaternion _rotation, Transform _parent)
        {
            Transform _spawned = Spawn(_templateName, _position, _rotation);
            if (_spawned)
            {
                _spawned.parent = _parent;
            }
            return _spawned;
        }

        /// <summary>
        /// Call this to spawn object by it's template name
        /// </summary>
        /// <param name="_templateName">Name of template</param>
        /// <param name="_position">Position of template</param>
        /// <param name="_rotaion">Rotation of template</param>
        /// <returns>Instance of spawned position</returns>
        public Transform Spawn(string _templateName, Vector3 _position, Quaternion _rotaion)
        {
            for (int i = 0; i < TemplateNames.Count; i++)
            {
                if (TemplateNames[i] == _templateName)
                {
                    return Spawn(Templates[i], _position, _rotaion);
                }
            }
            return null;
        }

        /// <summary>
        /// Call it ot spawn object by it's template name
        /// </summary>
        /// <param name="_templateName">Name of template</param>
        /// <param name="_position">Position of template</param>
        /// <returns>Instance of spawned object</returns>
        public Transform Spawn(string _templateName, Vector3 _position)
        {
            return Spawn(_templateName, _position, Quaternion.identity);
        }

        /// <summary>
        /// Call it to spawn object by it's template name
        /// </summary>
        /// <param name="_templateName">Name of template</param>
        /// <returns>Instance of spawned object</returns>
        public Transform Spawn(string _templateName)
        {
            return Spawn(_templateName, Vector3.zero, Quaternion.identity);
        }
        #endregion

        #region SPAWN_ID_METHODS
        /// <summary>
        /// Call it to spawn object by template ID
        /// </summary>
        /// <param name="_id">ID of object</param>
        /// <param name="_position">Position of object</param>
        /// <param name="_rotation">Rotation of object</param>
        /// <param name="_parent">Parent of object</param>
        /// <returns>Instance of spawned object</returns>
        public Transform Spawn(int _id, Vector3 _position, Quaternion _rotation, Transform _parent)
        {
            Transform _spawned = Spawn(_id, _position, _rotation);
            if (_spawned)
            {
                _spawned.parent = _parent;
            }
            return _spawned;
        }

        /// <summary>
        /// Call it to spawn object by template ID
        /// </summary>
        /// <param name="_id">ID of template</param>
        /// <param name="_position">Position of object</param>
        /// <param name="_rotation">Rotation of object</param>
        /// <returns>Instance of spawned object</returns>
        public Transform Spawn(int _id, Vector3 _position, Quaternion _rotation)
        {
            for (int i = 0; i < TemplateIDs.Count; i++)
            {
                if (TemplateIDs[i] == _id)
                {
                    return Spawn(Templates[i], _position, _rotation);
                }
            }
            return null;
        }

        /// <summary>
        /// Call it to spawn object by template's ID
        /// </summary>
        /// <param name="_id">ID of template</param>
        /// <param name="_position">Position of object</param>
        /// <returns>Spawned object's instance</returns>
        public Transform Spawn(int _id, Vector3 _position)
        {
            return Spawn(_id, _position, Quaternion.identity);
        }

        /// <summary>
        /// Call it to spawn object by it's ID
        /// </summary>
        /// <param name="_id">ID of object</param>
        /// <returns>Spawned instance of object</returns>
        public Transform Spawn(int _id)
        {
            return Spawn(_id, Vector3.zero, Quaternion.identity);
        }

        #endregion

        #region DESPAWN_METHODS
        /// <summary>
        /// Call it to despawn object after _despawnDelay seconds
        /// </summary>
        /// <param name="_object">Object to despawn</param>
        /// <param name="_despawnDelay">Despawn delay in seconds</param>
        public void Despawn(Transform _object, float _despawnDelay)
        {
            //if we got null object - just return out
            if (_object == null)
                return;

            //calling coroutne
            StartCoroutine(WaitForDespawn(_object, _despawnDelay));
        }

        /// <summary>
        /// Coroutine for despawn delaying
        /// </summary>
        /// <param name="_obj"></param>
        /// <param name="_delta"></param>
        /// <returns></returns>
        IEnumerator WaitForDespawn(Transform _obj, float _delta)
        {
            yield return new WaitForSeconds(_delta);
            Despawn(_obj);
        }

        /// <summary>
        /// Call it to despawn some object with choosed template and despawn delay
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="_template"></param>
        /// <param name="_despawnDelay"></param>
        public void Despawn(Transform _object, Transform _template, float _despawnDelay)
        {
            //if we got null object - just return out
            if (_object == null)
                return;

            //calling coroutne
            StartCoroutine(WaitForDespawn(_object, _template, _despawnDelay));
        }

        /// <summary>
        /// Coroutine for despawn delaying
        /// </summary>
        /// <param name="_obj">Object to despawn</param>
        /// <param name="_temp">Template of object(prefab)</param>
        /// <param name="_delta">Time in seco for pause</param>
        /// <returns></returns>
        IEnumerator WaitForDespawn(Transform _obj, Transform _temp, float _delta)
        {
            yield return new WaitForSeconds(_delta);
            Despawn(_obj, _temp);
        }

        /// <summary>
        /// Call it to despawn object
        /// </summary>
        /// <param name="_object">Object to despawn</param>
        public void Despawn(Transform _object, bool _dontRemove = false)
        {
            //if we got null object - just return out
            if (_object == null)
                return;

            //trying to get ISpawnableObject component
            ISpawnableObject _activeObject = _object.GetComponent<ISpawnableObject>();
            if (_activeObject == null)
            {
                DespawnHard(_object, _dontRemove);
            }
            else
            { 
                if (_activeObject.GetTemplate() != null)
                {
                    Despawn(_object, _activeObject.GetTemplate(), _dontRemove);
                }
                else
                {
                    DespawnHard(_object, _dontRemove);
                }
            }
        }

        /// <summary>
        /// Call it to despawn object
        /// </summary>
        /// <param name="_object">Object to despawn</param>
        /// <param name="_template">Object's template/prefab</param>
        public void Despawn(Transform _object, Transform _template, bool _dontRemove = false)
        {
            if (_template == null)
            {
                Despawn(_object);
                return;
            }

            if (Templates.Contains(_template))
            {
                foreach (SerializedDictionary _dict in UnawailableObjects)
                {
                    if(_dict.Key == _template)
                    {
                        if (_dict.Contains(_object))
                        {
                            //removing  from unawailable list
                            if (!_dontRemove)
                            {
                                _dict.RemoveObject(_object);
                            }
                            _object.gameObject.SetActive(false);

                            //adding to awailable list
                            foreach (SerializedDictionary _dictFuture in AwailableObjects)
                            {
                                if (_dictFuture.Key == _template)
                                {
                                    _dictFuture.AddObject(_object);
                                    return;
                                }
                            }
                        }
                        //and lets try - if object is still in Awailable list!
                        else
                        {
                            foreach (SerializedDictionary _dictA in AwailableObjects)
                            {
                                if (_dictA.Key == _template)
                                {
                                    if (_dictA.Contains(_object))
                                    {
                                        _object.gameObject.SetActive(false);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Log.Write("It doesnt contain needable template:" + _object.gameObject.name, LogColors.Yellow);
            }
            DespawnHard(_object);
        }

        /// <summary>
        /// Not recommended - it will start parce all dictionaries and lists in looking of instances
        /// </summary>
        /// <param name="_object">Object to despawn</param>
        public void DespawnHard(Transform _object, bool _dontRemove = false)
        {
            if (!_object)
                return;

            Log.Write("Despawning hard  " + _object.gameObject.name, LogColors.Yellow);
            foreach (SerializedDictionary _dict in UnawailableObjects)
            {
                if (_dict.Contains(_object))
                {
                    if (!_dontRemove)
                    {
                        _dict.RemoveObject(_object);
                    }
                    foreach (SerializedDictionary _dictFuture in AwailableObjects)
                    {
                        if (_dictFuture.Key == _dict.Key)
                        {
                            _dictFuture.AddObject(_object);
                            _object.gameObject.SetActive(false);
                            return;
                        }
                    }
                }
            }
            _object.gameObject.SetActive(false);
        }

        public void DespawnAll()
        {
            StopAllCoroutines();

            foreach (SerializedDictionary _dict in UnawailableObjects)
            {
                if (_dict != null)
                {
                    if (_dict.Value != null && _dict.Value.Count > 0)
                    {
                        foreach (Transform _objToDespawn in _dict.Value)
                        {
                            if (_objToDespawn != null)
                                Despawn(_objToDespawn, true);
                        }
                        _dict.Value.Clear();
                    }
                }
            }
        }
        #endregion
    }

    [Serializable]
    public class TemplateData
    {

    }


    [Serializable]
    public class SerializedDictionary
    {
        #region VARIABLES
        /// <summary>
        /// Key of dictionary - template of object(prefab)
        /// </summary>
        public Transform Key;

        /// <summary>
        /// private value - inaccesable from out
        /// </summary>
        [SerializeField]
        private LinkedList<Transform> value;

        /// <summary>
        /// value of dictionary - list of objects, that were Instantiated for that template
        /// </summary>
        public LinkedList<Transform> Value
        {
            get
            {
                if (value == null)
                { 
                    value = new LinkedList<Transform>();
                }

                return value;
            }

            set
            {
                this.value = value; 
            }
        } 
        #endregion

        #region METHODS
        /// <summary>
        /// Constructor for dictionary. Dictionary hasn't be without Key 
        /// </summary>
        /// <param name="_key"></param>
        public SerializedDictionary(Transform _key)
        {
            Key = _key;
        }

        /// <summary>
        /// Call it to get know does this object consist in that dict
        /// </summary>
        /// <param name="_obj">Objct template</param>
        /// <returns>TRUE if contains, FALSE if not</returns>
        public bool Contains(Transform _obj)
        { 
            return Value.Contains(_obj);
        } 

        /// <summary>
        /// Use it to get count of objects in current dictionary(Value.Count)
        /// </summary>
        /// <returns>Count of objects in list</returns>
        public int GetCount()
        {
            return Value.Count;
        }

        /// <summary>
        /// Call it to add object(Value.AddLast)
        /// </summary>
        /// <param name="_obj">Object to add</param>
        public void AddObject(Transform _obj)
        {
            Value.AddLast(_obj); 
        }

        /// <summary>
        /// Call it to remove object
        /// </summary>
        /// <param name="_obj">Object to remove</param>
        public void RemoveObject(Transform _obj)
        {
            Value.Remove(_obj);
        }

        /// <summary>
        /// Call it to get first object and remove it from list
        /// </summary>
        /// <returns></returns>
        public Transform GetFirst()
        {
            if (Value.Count > 0)
            {
                Transform _selected = Value.First.Value;
                RemoveFirst();
                return _selected;
            }

            return null;
        }

        /// <summary>
        /// Call it to remove first object
        /// </summary>
        public void RemoveFirst()
        {
            if(Value.Count > 0)
                Value.RemoveFirst();
        }

        /// <summary>
        /// Call it to clear list
        /// </summary>
        public void ClearAll()
        { 
            Value.Clear();
        }

        #endregion
    } 
}
