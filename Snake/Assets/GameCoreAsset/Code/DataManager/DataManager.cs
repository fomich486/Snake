using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WeaponSystem;

namespace DataSystem
{
    /// <summary>
    /// Class for saving and loading data
    /// </summary>
    public class DataManager : Singleton<DataManager>
    {
        #region VARIABLES
        /// <summary>
        /// Balance data 
        /// </summary>
        public BalanceData balanceData;
        /// <summary>
        /// User save data
        /// </summary>
        public SaveData saveData;
        /// <summary>
        /// Flag if game will make autosave each SaveRepeat seconds
        /// </summary>
        public bool AutoSave = true;
        /// <summary>
        /// DetaTime of saves
        /// </summary>
        public float SaveRepeat = 10f;
        /// <summary>
        /// Variable of left time before auto saving
        /// </summary>
        private float curSaveLeast = 5f;
        /// <summary>
        /// Path to balance file
        /// </summary>
        public string balanceDataPath = "/DataStorage/BalanceData.json";
        /// <summary>
        /// Path to user save file
        /// </summary>
        public string saveDataPath = "/DataStorage/SaveData.json";
        /// <summary>
        /// Flag if we have to make saves by file
        /// </summary>
        public bool SaveByFile = false;
        #endregion

        #region METHODS_MANAGER
        /// <summary>
        /// Called once in the start
        /// </summary>
        private void Awake()
        { 
            base.Awake();

#if UNITY_EDITOR
            SaveByFile = true;
#endif  
            curSaveLeast = SaveRepeat;
            //SaveBalanceData();
            ReadBalanceData();
            ReadSaveData(); 
        }
            
        /// <summary>
        /// Called each frame
        /// </summary>
        private void Update()
        {
            //autosaving
            if (AutoSave)
            {
                if (curSaveLeast <= 0)
                {
                    curSaveLeast = SaveRepeat;
                    SaveUserData();
                }
                curSaveLeast -= Time.deltaTime;
            }

            //for balance data saving
            if (Input.GetKeyDown(KeyCode.O))
            {
                SaveBalanceData();
            }
        }

        /// <summary>
        /// Call it to get savedData instance
        /// </summary>
        /// <returns></returns>
        public SaveData GetSavedData()
        {
            return saveData;
        }

        /// <summary>
        /// Call it to get balanceData instance
        /// </summary>
        /// <returns></returns>
        public BalanceData GetBalaceData()
        {
            return balanceData;
        }

        /// <summary>
        /// Call it to save user's data
        /// </summary>
        public void SaveUserData()
        { 
            try
            {
                Log.Write("Saving user data..");
                //setting save data
                string _curData = DateTime.Now.ToString();
                saveData.DataSaveTime = _curData;
                string _dataAsJson = JsonUtility.ToJson(saveData);

                if (!SaveByFile)
                {
                    //Debug.Log("Saving in PlayerPrefs assets.");
                    PlayerPrefs.SetString(saveDataPath, _dataAsJson);
                    PlayerPrefs.Save();
                }
                else
                {
                    //Debug.Log("Saving in streaming assets.");
                    string _filePath = Path.Combine(Application.streamingAssetsPath, saveDataPath);
                    if (File.Exists(_filePath))
                    {
                        File.WriteAllText(_filePath, _dataAsJson);
                    }
                    else
                    {
                        Log.Write("There is no file to save in:" + _filePath, LogColors.Red);
                    }
                }

            }
            catch (UnityException ex)
            {
                Log.Write("Catched error: " + ex.Message, LogColors.Red);
            }
        }

        /// <summary>
        /// Call it to save balance data(EDITOR ONLY)
        /// </summary>
        public void SaveBalanceData()
        {
            string _dataAsJson = JsonUtility.ToJson(balanceData);
            string _filePath = Path.Combine(Application.streamingAssetsPath, balanceDataPath);
            File.WriteAllText(_filePath, _dataAsJson);
        }

        /// <summary>
        /// Call it to load saves on android
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadSaveAndroid()
        {
            SaveData _fileData = null;
            string _filePath = Path.Combine(Application.streamingAssetsPath, saveDataPath);
            WWW www = new WWW(_filePath);
            float _toWaitTime = 2f;
            while(_toWaitTime > 0 && !www.isDone)
            {
                _toWaitTime -= Time.deltaTime;
                yield return null;
            }

            if (www != null)
            { 
                string _dataAsJson = www.text;
                _fileData = JsonUtility.FromJson<SaveData>(_dataAsJson);
                if (saveData != null)
                {
                    saveData = _fileData;
                    Log.Write("loaded save data from file");
                }
            }

            string _savedAsJson = PlayerPrefs.GetString(saveDataPath, "");
            SaveData _dataSaved = null;
            if (_savedAsJson != "")
            {
                Log.Write("Readed save data from player prefs!");
                _dataSaved = JsonUtility.FromJson<SaveData>(_savedAsJson);
            }

            CompareTwoSaves(_fileData, _dataSaved); 
        }

        /// <summary>
        /// Call it to read save date
        /// </summary>
        void ReadSaveData()
        {
#if UNITY_EDITOR
            string _filePath = Path.Combine(Application.streamingAssetsPath, saveDataPath);
            SaveData _fileData = null;

            if (File.Exists(_filePath))
            {
                Log.Write("save file is not null on " + _filePath);
                string _dataAsJson = File.ReadAllText(_filePath);
                _fileData = JsonUtility.FromJson<SaveData>(_dataAsJson);
                saveData = _fileData;
            }

            string _savedAsJson = PlayerPrefs.GetString(saveDataPath, "");
            SaveData _dataSaved = null;
            if (_savedAsJson != "")
            {
                Log.Write("save playerprefs is not null!");
                _dataSaved = JsonUtility.FromJson<SaveData>(_savedAsJson);
            }
            CompareTwoSaves(_fileData, _dataSaved);
#elif UNITY_ANDROID
            StartCoroutine(LoadSaveAndroid());
#else
            string _filePath = Path.Combine(Application.streamingAssetsPath, saveDataPath);
            SaveData _fileData = null;

            if (File.Exists(_filePath))
            {
                Log.Write("save file is not null");
                string _dataAsJson = File.ReadAllText(_filePath);
                _fileData = JsonUtility.FromJson<SaveData>(_dataAsJson);
                saveData = _fileData;
            }

            string _savedAsJson = PlayerPrefs.GetString(saveDataPath, "");
            SaveData _dataSaved = null;
            if (_savedAsJson != "")
            {
                Log.Write("save playerprefs is not null!");
                _dataSaved = JsonUtility.FromJson<SaveData>(_savedAsJson);
            }
            CompareTwoSaves(_fileData, _dataSaved);
#endif
        }

        /// <summary>
        /// Compares two saves, checks it's save data and selecting some
        /// </summary>
        /// <param name="_a"></param>
        /// <param name="_b"></param>
        public void CompareTwoSaves(SaveData _a, SaveData _b)
        {
            if (_a == null && _b == null)
            {
                Log.Write("Creation new save!");
                saveData = new SaveData();
                 

                return;
            }

            //pasing save data A
            bool _parsedA = false;
            DateTime _aDate = DateTime.Now.AddYears(-55);
            if(_a != null && DateTime.TryParse(_a.DataSaveTime, out _aDate))
            {
                _parsedA = true;
            }

            //parsing save data B
            bool _parsedB = false;
            DateTime _bDate = DateTime.Now.AddYears(-55);
            if(_b != null && DateTime.TryParse(_b.DataSaveTime, out _bDate))
            {
                _parsedB = true;
            }

            //A is installed B not
            if(_parsedA && !_parsedB)
            {
                saveData = _a;
                Log.Write("Accepting file save");
            }
            //B is installed A not
            else if(!_parsedA && _parsedB)
            {
                saveData = _b;
                Log.Write("Accepting PlayerPrefs save");
            }
            //both are installed
            else if(_parsedA && _parsedB)
            {
                if(_aDate > _bDate)
                {
                    saveData = _a;
                    Log.Write("Accepting file save");
                }
                else
                {
                    saveData = _b;
                    Log.Write("Accepting PlayerPrefs save");
                }
            }
            //there is no saves - creation new
            else
            {
                Log.Write("Creation new save!");
                saveData = new SaveData();

                SaveUserData();
            }
        }
         
        /// <summary>
        /// Call it to load balance on android
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadBalanceAndroid()
        {
            //FIRST VARIANT
            string _filePath = Path.Combine(Application.streamingAssetsPath, balanceDataPath);
            WWW www = new WWW(_filePath);
            float _toWaitTime = 2f;
            while (_toWaitTime > 0 && !www.isDone)
            {
                _toWaitTime -= Time.deltaTime;
                yield return null;
            }

            if (www != null)
            {
                string dataAsJson = www.text;
                BalanceData _data = JsonUtility.FromJson<BalanceData>(dataAsJson);
                if (_data != null)
                {
                    Log.Write("Loaded balance data");
                    balanceData = _data;

                }
            }
        }

        /// <summary>
        /// Call it ot read balance data
        /// </summary>
        void ReadBalanceData()
        {
#if UNITY_EDITOR
            string _filePath = Path.Combine(Application.streamingAssetsPath, balanceDataPath);

            if (File.Exists(_filePath))
            {
                Log.Write("loaded file of JSON " + _filePath);
                string dataAsJson = File.ReadAllText(_filePath);
                BalanceData _data = JsonUtility.FromJson<BalanceData>(dataAsJson);
                if (_data != null)
                    balanceData = _data;
                else
                    Log.Write("Cant convert balance data from file!", LogColors.Yellow);
            }
            else
            {
                Log.Write("There is no BalanceData.json in :" + _filePath + " , creation new file!", LogColors.Yellow);
                if (balanceData == null)
                    balanceData = new BalanceData();

                //string _dataAsJson = JsonUtility.ToJson(balanceData);
                //File.WriteAllText(_filePath, _dataAsJson);
            }
#elif UNITY_ANDROID
            StartCoroutine(LoadBalanceAndroid());
#else
            string _filePath = Path.Combine(Application.streamingAssetsPath, balanceDataPath);

            if (File.Exists(_filePath))
            {
                Debug.Log("loaded file of JSON");
                string dataAsJson = File.ReadAllText(_filePath);
                BalanceData _data = JsonUtility.FromJson<BalanceData>(dataAsJson);
                if (_data != null)
                    balanceData = _data;
                else
                    Debug.LogWarning("Cant convert balance data from file!");
            }
            else
            {
                Debug.LogWarning("There is no BalanceData.json in :" + _filePath + " , creation new file!");
                if (balanceData == null)
                    balanceData = new BalanceData();

                string _dataAsJson = JsonUtility.ToJson(balanceData);
                File.WriteAllText(_filePath, _dataAsJson);
            }
#endif
        }

        #endregion

        //put here read/save methods (like find some variable, index,etc)
        #region SAVE_METHODS    

        #endregion

        //put here read methods (like find some variable, index,etc)
        #region BALANCE_METHODS 

        #endregion 
    }
} 