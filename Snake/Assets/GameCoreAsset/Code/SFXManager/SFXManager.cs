using DataSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFXManagerModule
{
    /// <summary>
    /// SFX manager for processing all music in game(including sounds, music themes, etc). Has functions like pool
    /// </summary>
    public class SFXManager : Singleton<SFXManager>
    {  
        #region VARIABLES
        /// <summary>
        /// Flag if we have to show music options (EDITOR ONLY)
        /// </summary>
        public bool ShowMusicOptions = false;
        /// <summary>
        /// Flag if we have to show sound options (EDITOR ONLY)
        /// </summary>
        public bool ShowSoundOptions = false;
        /// <summary>
        /// Flag if we have to show 3D sound options (EDITOR ONLY)
        /// </summary>
        public bool Show3DSoundOptions = false;
        /// <summary>
        /// Flag if we have to show deep inspector of options (EDITOR ONLY)
        /// </summary>
        public bool ShowDeepInspector = false;
        
        /// <summary>
        /// AudioSource to play music
        /// </summary>
        public AudioSource MusicSource;
        /// <summary>
        /// AudioSource to play sound
        /// </summary>
        public AudioSource SoundSource;

        /// <summary>
        /// Flag if we can play bacground music
        /// </summary>
        private bool canPlayBg = true;
        /// <summary>
        /// private inst
        /// </summary>
        private List<AudioClip> bgClips;
        /// <summary>
        /// Current AudioClips to play (music bg)
        /// </summary>
        public List<AudioClip> BgClips
        {
            get
            {
                if (bgClips == null)
                    bgClips = new List<AudioClip>();

                return bgClips;
            }
            set
            {
                bgClips = value;
            }
        }

        /// <summary>
        /// private inst
        /// </summary>
        [SerializeField]
        private List<MusicDictionary> musicThemes;
        /// <summary>
        /// Themes of music with lists of clips
        /// </summary>
        public List<MusicDictionary> MusicThemes
        {
            get
            {
                if (musicThemes == null)
                    musicThemes = new List<MusicDictionary>();

                return musicThemes;
            }
            set
            {
                musicThemes = value;
            }
        }

        /// <summary>
        /// private inst
        /// </summary>
        [SerializeField]
        private List<SoundDictionary> awailableSources;
        /// <summary>
        /// Awailable players detached by source type 
        /// </summary>
        public List<SoundDictionary> AwailableSources
        {
            get
            {
                if (awailableSources == null)
                    awailableSources = new List<SoundDictionary>();

                return awailableSources;
            } 
            set
            {
                awailableSources = value;
            }
        }

        /// <summary>
        /// Unawailable players detached by source type (currently in using)
        /// </summary>
        public List<SoundDictionary> unawailableSources = new List<SoundDictionary>(); 
         
        /// <summary>
        /// Current music listing coroutine
        /// </summary>
        private Coroutine musicListingCoroutine = null;

        /// <summary>
        /// Time of music fading
        /// </summary>
        public float MusicChangeTime = 1f;
         
        /// <summary>
        /// Music source volume
        /// </summary>
        public float MusicVolume = 1f; 
        /// <summary>
        /// Music source pitch
        /// </summary>
        public float MusicPitch = 1f;

        /// <summary>
        /// Sound source volume
        /// </summary>
        public float SoundVolume = 1f;
        /// <summary>
        /// Sound source pitch
        /// </summary>
        public float SoundPitch = 1f;
        /// <summary>
        /// Index of current music theme(that is currently playing)
        /// </summary>
        private int currentMusicTheme = -1;

        private WaitForSeconds bgChangeDelay = new WaitForSeconds(0.2f);
        #endregion

        #region METHODS_INSTALL
        private void Awake()
        {
            base.Awake();
            //install sources
            InstallSources();
        }

        /// <summary>
        /// Called in 1st frame
        /// </summary>
        private void Start()
        {
            //read music and sound values from save
            //MusicVolume = DataManager.Instance.saveData.MusicVolume;
            //SoundVolume = DataManager.instance.saveData.SoundVolume;
        } 

        /// <summary>
        /// Call it to install all sources
        /// </summary>
        void InstallSources()
        {
            foreach(SoundDictionary _dict in AwailableSources)
            {
                CreatePlayers(_dict);
            }
        }

        /// <summary>
        /// Called each frame
        /// </summary>
        private void Update()
        {
            //updating volume and pitch
            UpdateBaseOptions();
        }

        #endregion

        #region MUSIC_PLAYING_FUNCTIONS

        /// <summary>
        /// Call it to clear all players(disabling them)
        /// </summary>
        public void ClearAllPlayers()
        {
            //NOTE. We dont replace elements for first sight, but actually we do. After few functions you will see, that unawailable elements go to awailable
            //and in the end we clear list of unawailable. 

            //parse all unawailableSources (actually, sourses that are playing now) and disable them without removing from list
            if(unawailableSources != null && unawailableSources.Count > 0)
            {
                foreach (SoundDictionary _sound in unawailableSources.ToArray())
                {
                    if (_sound != null && _sound.Value != null)
                    { 
                        foreach (SFXPlayer _player in _sound.Value)
                        { 
                            _player.Disable(true); 
                        }
                    }
                    _sound.Value.Clear();
                }
            }
            //and parse awailable sources too (if, for some reason it's playing for now, or something like this)
            if (awailableSources != null && awailableSources.Count > 0)
            {
                foreach (SoundDictionary _sound in awailableSources.ToArray())
                {
                    if (_sound != null && _sound.Value != null)
                    {
                        foreach (SFXPlayer _player in _sound.Value)
                        {
                            _player.EndPlaying();
                            _player.Disable(true);
                        }
                    } 
                }
            }
        }

        /// <summary>
        /// Call it to play some track in background in loop
        /// </summary>
        /// <param name="_clip">AudioClip</param>
        public void PlayBGMusic(AudioClip _clip)
        {
            canPlayBg = true;
            //soft fade out of volume
            if (MusicSource.isPlaying)
            {
                MusicSource.loop = true;
                StartCoroutine(ProcessMusicChanging(_clip));
            }
            //just turn on
            else
            {
                MusicSource.clip = _clip;
                MusicSource.loop = true;
                MusicSource.Play(); 
            } 
        }
        
        /// <summary>
        /// Call it to switch music theme
        /// </summary>
        /// <param name="_type">Selected theme to switch on</param>
        public void SwitchMusicTheme(string _type)
        {
            //check if we found that theme or it's theme doesn't match current
            int _foundIndex = GetIndex(_type);
            if (_foundIndex > -1 && currentMusicTheme != _foundIndex)
            {
                currentMusicTheme = _foundIndex;    
                SetNewBGClips(MusicThemes[_foundIndex].Value);
            }
            else
            {
                //we havent found any saved MusicType group in our list
            }
        }

        /// <summary>
        /// Call it to get index of some theme
        /// </summary>
        /// <param name="_type">Theme type</param>
        /// <returns></returns>
        public int GetIndex(string _type)
        {
            for (int i = 0; i < MusicThemes.Count; i++)
            {
                if (MusicThemes[i].Key == _type)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Call it to make soft music changing
        /// </summary>
        /// <param name="_clip">AudioClip that have to be turned on after soft switching</param>
        /// <returns></returns>
        IEnumerator ProcessMusicChanging(AudioClip _clip)
        {
            //variables
            //current volume
            float _curVolume = MusicSource.volume;
            //volume that we have to reach
            float _targetVolume = 1f;
            //speed of lerp(V=S/T)
            float _lerpSpeed = 1f / MusicChangeTime;
            //lerp coeficient
            float _lerpCoef = 0f;
            //volume from max to 0
            while (_lerpCoef > 1)
            {
                MusicSource.volume = Mathf.Lerp(_curVolume, _targetVolume, _lerpCoef);
                _lerpCoef -= Time.deltaTime * _lerpSpeed;
                yield return new WaitForEndOfFrame();
            }
            _lerpCoef -= Time.deltaTime * _lerpSpeed;

            //turning on new clip
            MusicSource.Stop();
            MusicSource.clip = _clip;
            MusicSource.Play();

            //volume from 0 to max
            _curVolume = 0f;
            _targetVolume = MusicVolume;
            _lerpCoef = 0;
            while (_lerpCoef > 1)
            {
                MusicSource.volume = Mathf.Lerp(_curVolume, _targetVolume, _lerpCoef);
                _lerpCoef -= Time.deltaTime * _lerpSpeed;
                yield return new WaitForEndOfFrame();
            }
            _lerpCoef -= Time.deltaTime * _lerpSpeed;
        }

        /// <summary>
        /// Call it to start playing background music(from selected list)
        /// </summary>
        public void PlayBGMusic()
        {
            canPlayBg = true;
            //if one - loop
            if (BgClips.Count == 1)
            {
                MusicSource.clip = BgClips[0];
                MusicSource.loop = true;
                MusicSource.Play();
            }
            //if zero - its bad, have to do something in logic?
            else if (BgClips.Count == 0)
            {
                Log.Write("There is no BgClips to play! BgClips.Count == 0",LogColors.Yellow);
            }
            else
            {
                //randoming new index
                int _randomedIndex = (int)UnityEngine.Random.Range(0, BgClips.Count);

                //if clip null
                if (BgClips[_randomedIndex] == null)
                {
                    Log.Write("BgClips[" + _randomedIndex + "] is null! Fix it!",LogColors.Yellow);
                    return;
                }

                //if musicSource is currently playing - make soft switching
                if (MusicSource.isPlaying)
                {
                    MusicSource.loop = false;
                    StartCoroutine(ProcessMusicChanging(BgClips[_randomedIndex]));
                    musicListingCoroutine = StartCoroutine(ProcessBGMusicQueue(MusicChangeTime));
                }
                //else - hard
                else
                {
                    MusicSource.clip = BgClips[_randomedIndex];
                    MusicSource.loop = false;
                    MusicSource.Play();
                    musicListingCoroutine = StartCoroutine(ProcessBGMusicQueue(0f));
                }
            } 
        }

        /// <summary>
        /// Call it to set your own list of BG music
        /// </summary>
        /// <param name="_bgClips"></param>
        public void SetNewBGClips(List<AudioClip> _bgClips)
        {
            if (_bgClips == null)
            {
                Log.Write("You passed null list to SetNewBGClips! Check it!",LogColors.Yellow);
                return;
            }
            else if (_bgClips.Count == 0)
            {
                MusicSource.clip = null;
                MusicSource.Stop();
            }

            //stop previous music listing if need
            if (musicListingCoroutine != null)
                StopCoroutine(musicListingCoroutine);

            BgClips = _bgClips; 
             
            PlayBGMusic();
        }

        /// <summary>
        /// Call it to stop playing bg music
        /// </summary>
        public void StopMusic()
        {
            canPlayBg = false;
            MusicSource.Stop(); 
        }

        /// <summary>
        /// Call it to update volume and pitch of music and sound sources
        /// </summary>
        void UpdateBaseOptions()
        {
            //music source
            if (MusicSource != null)
            {
                if(MusicSource.volume != MusicVolume)
                    MusicSource.volume = MusicVolume;

                if(MusicSource.pitch != MusicPitch)
                    MusicSource.pitch = MusicPitch;
            }
            //sound source
            if (SoundSource != null)
            {
                SoundSource.volume = SoundVolume;
                SoundSource.pitch = SoundPitch;
            }
        }

        /// <summary>
        /// Call it to start procesing BG music switching(random next after previous)
        /// </summary>
        /// <param name="_startDelay"></param>
        /// <returns></returns>
        IEnumerator ProcessBGMusicQueue(float _startDelay)
        {
            yield return new WaitForSeconds(_startDelay);

            while (MusicSource.isPlaying)
            {
                yield return bgChangeDelay;
            }

            //just call this to random another clip
            if(canPlayBg)
                PlayBGMusic();
        }

        #endregion

        #region SOUND_PLAYING_FUNCTIONS
        /// <summary>
        /// Call it to play some sound(without 3D effect)
        /// </summary>
        /// <param name="_clip">Sound to play</param>
        public void PlaySound(AudioClip _clip, float _volume = 1f)
        {
            SoundSource.PlayOneShot(_clip, _volume);
        }

        /// <summary>
        /// Call it to play random sound sound(without 3D effect)
        /// </summary>
        /// <param name="_clip">AudioClip array to play</param>
        public void PlaySound(AudioClip[] _clips)
        {
            if (_clips != null)
            {
                int _randomedIndex = (int)UnityEngine.Random.Range(0, _clips.Length);
                if (_clips[_randomedIndex] != null)
                {
                    SoundSource.PlayOneShot(_clips[_randomedIndex]);
                }
            }
            else
            {
                Log.Write("You passed null AudioClip[] to play!",LogColors.Yellow);
            }
        }

        /// <summary>
        /// Call it to play random sound (without 3d effect)
        /// </summary>
        /// <param name="_clips">AudioClip list to play</param>
        public void AudioClip(List<AudioClip> _clips)
        {
            if (_clips != null)
            {
                int _randomedIndex = (int)UnityEngine.Random.Range(0, _clips.Count);
                if (_clips[_randomedIndex] != null)
                {
                    SoundSource.PlayOneShot(_clips[_randomedIndex]);
                }
            }
            else
            {
                Log.Write("You passed null AudioClip[] to play!",LogColors.Yellow);
            }
        }

        /// <summary>
        /// Call it to stop playing all sounds  
        /// </summary>
        /// <param name="_clip"></param>
        public void StopAllSounds()
        {
            SoundSource.Stop(); 
        }

        /// <summary>
        /// Call it to get free SFXPlayer by ID or Name or nothing!
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_name"></param>
        /// <returns></returns>
        public SFXPlayer GetPlayer(int _id = -1, string _name = "none")
        {
            SFXPlayer _selected = null;
            //audio source type by ID
            if (_id != -1)
            {
                int _index = GetSourceIndexByID(_id, AwailableSources);
                if (AwailableSources[_index].Value.Count > 0)
                    _selected = GetFirstPlayer(AwailableSources[_index]); 
            }
            //audio source type by name
            else if (_name != "none")
            {
                int _index = GeSourceIndexByName(_name);
                if (AwailableSources[_index].Value.Count > 0)
                    _selected = GetFirstPlayer(AwailableSources[_index]);

            }
            //audio source without ID and name - taking first
            else if (AwailableSources[0].Value.Count > 0)
                _selected = GetFirstPlayer(AwailableSources[0]);

            //try to get first
            if (_selected == null && AwailableSources[0].Value.Count > 0)
                _selected = GetFirstPlayer(AwailableSources[0]);

            if (_selected == null)
                Log.Write("There is no free AudioPlayers. Check it out!",LogColors.Yellow);
            else
                _selected.gameObject.SetActive(true);

            return _selected;
        }

        /// <summary>
        /// Call it to play sounds that will follow some object in 3d space
        /// </summary>
        /// <param name="_data">Source data</param>
        /// <param name="_parent">Sound will follow by this object in 3d space</param>
        /// <param name="_id">ID of source type</param>
        /// <param name="_name">Name of source type</param>
        /// <returns></returns>
        public SFXPlayer PlayFollowingSound(SFXSourceData _data, Transform _parent, int _id = -1, string _name = "none")
        {
            SFXPlayer _selected = GetPlayer(_id, _name);
            if (_selected != null && _parent != null)
            {  
                _selected.StartPlaying(_data); 
                _selected.transform.position = _parent.position;
                //add fake children component, that will move this object (perform just like child, actually) by parent
                FakeChildren _fc = _selected.gameObject.GetComponent<FakeChildren>();
                if(_fc == null)
                    _fc = _selected.gameObject.AddComponent<FakeChildren>();
                _fc.HaveToDespawnLikeParent = true;

                _fc.HaveToDespawnAfter = false;
                _fc.Pos = true;
                _fc.Rot = false;
                _fc.Lerp = false;
                _fc.Init(_parent, _selected.transform);
                _selected.gameObject.SetActive(true);
                return _selected;
            }
            if (_parent == null)
                Log.Write("Parent is null");

            Log.Write("Returning null! Free count: " + AwailableSources[0].Value.Count, LogColors.Red);
            return null;
        }

        /// <summary>
        /// Call it to play sounds that will follow some object in 3d space with some bias
        /// </summary>
        /// <param name="_data">Source data</param>
        /// <param name="_parent">Sound will follow by this object in 3d space</param>
        /// <param name="_toPos">Start position of source. Difference between this position and _parent.position will be total bias</param>
        /// <param name="_id">ID of source type</param>
        /// <param name="_name">Name of source type</param>
        /// <returns>Spawned SFXPlayer, ready to magane(if need)</returns>
        public SFXPlayer PlayFollowingSound(SFXSourceData _data, Transform _parent, Vector3 _toPos, int _id = -1, string _name = "none")
        {
            SFXPlayer _selected = GetPlayer(_id, _name);
            if (_selected != null)
            {
                _selected.StartPlaying(_data);
                _selected.transform.position = _toPos;
                FakeChildren _fc = _selected.gameObject.GetComponent<FakeChildren>(); // _selected.gameObject.AddComponent<FakeChildren>(); 
                if (_fc == null)
                    _fc = _selected.gameObject.AddComponent<FakeChildren>();

                _fc.HaveToDespawnLikeParent = true;
                _fc.Init(_parent, _selected.transform);
                return _selected;
            }
            return null;
        }

        /// <summary>
        /// Call it to play sound that will follow some object in 3d space 
        /// </summary>
        /// <param name="_clip">Audio clip to play</param>
        /// <param name="_parent">Virtual "Parent" of audio source</param>
        /// <param name="_volume">Volume multiplier</param>
        /// <param name="_pitch">Pitch multiplier</param>
        /// <param name="_id">ID of source type</param>
        /// <param name="_name">Name of source type</param>
        /// <returns>Spawned SFXPlayer, ready to manage(if need)</returns>
        public SFXPlayer PlayFollowingSound(AudioClip _clip, Transform _parent, float _volume = 1f, float _pitch = 1f, int _id = -1, string _name = "none")
        {
            SFXSourceData _data = new SFXSourceData();
            _data.Clips = new List<AudioClip>();
            _data.Clips.Add(_clip);
            _data.Volume = _volume;
            _data.Pitch = _pitch; 
            return PlayFollowingSound(_data, _parent, _id, _name);
        }

        /// <summary>
        /// Call it to play  sound that will follow some object in 3d space with some bias
        /// </summary>
        /// <param name="_clip">AudioClip to play</param>
        /// <param name="_parent">Virtual "Paren" of audio source</param>
        /// <param name="_toPos">Position to spawn source. Difference between position and parent position will be result bias</param>
        /// <param name="_volume">Volume multiplier</param>
        /// <param name="_pitch">Pitch multiplier</param>
        /// <param name="_id">ID of source type</param>
        /// <param name="_name">Name of source type</param>
        /// <returns>Spawned SFXPlayer, ready to manage(if need)</returns>
        public SFXPlayer PlayFollowingSound(AudioClip _clip, Transform _parent, Vector3 _toPos, float _volume = 1f, float _pitch = 1f, int _id = -1, string _name = "none")
        {
            SFXSourceData _data = new SFXSourceData();
            _data.Clips = new List<AudioClip>();
            _data.Clips.Add(_clip);
            _data.Volume = _volume;
            _data.Pitch = _pitch;
            return PlayFollowingSound(_data, _parent, _toPos, _id, _name); 
        }

        /// <summary>
        /// Call it to play  sound that will follow some object in 3d space with some bias
        /// </summary>
        /// <param name="_clip">LKist of audio clip to play</param>
        /// <param name="_parent">Virtual "Paren" of audio source</param> 
        /// <param name="_volume">Volume multiplier</param>
        /// <param name="_pitch">Pitch multiplier</param>
        /// <param name="_id">ID of source type</param>
        /// <param name="_name">Name of source type</param>
        /// <returns>Spawned SFXPlayer, ready to manage(if need)</returns>
        public SFXPlayer PlayFollowingSound(List<AudioClip> _clips, Transform _parent,float _volume = 1f, float _pitch = 1f, int _id = -1, string _name = "none")
        {
            SFXSourceData _data = new SFXSourceData();
            _data.Clips = _clips;
            _data.Volume = _volume;
            _data.Pitch = _pitch;
            PlayFollowingSound(_data, _parent);

            if (_id != -1)
                return PlayFollowingSound(_data, _parent, _id);
            else if (_name != "none")
                return PlayFollowingSound(_data, _parent, -1, _name);
            else
                return PlayFollowingSound(_data, _parent);

        }
         
        /// <summary>
        /// Call it to play  sound that will follow some object in 3d space with some bias
        /// </summary>
        /// <param name="_clip">LKist of audio clip to play</param>
        /// <param name="_parent">Virtual "Paren" of audio source</param>
        /// <param name="_toPos">Position to spawn source. Difference between position and parent position will be result bias</param>
        /// <param name="_volume">Volume multiplier</param>
        /// <param name="_pitch">Pitch multiplier</param>
        /// <param name="_id">ID of source type</param>
        /// <param name="_name">Name of source type</param>
        /// <returns>Spawned SFXPlayer, ready to manage(if need)</returns>
        public SFXPlayer PlayFollowingSound(List<AudioClip> _clips, Transform _parent, Vector3 _toPos, float _volume = 1f, float _pitch = 1f, int _id = -1, string _name = "none")
        {
            SFXSourceData _data = new SFXSourceData();
            _data.Clips = _clips;
            _data.Volume = _volume;
            _data.Pitch = _pitch;

            if(_id!=-1)
                return PlayFollowingSound(_data, _parent, _toPos, _id);
            else if(_name!="none")
                return PlayFollowingSound(_data, _parent, -1, _name);
            else
                return PlayFollowingSound(_data, _parent, _toPos);
        }

        /// <summary>
        /// Call it to get source index by it's ID
        /// </summary>
        /// <param name="_id">ID of source</param>
        /// <param name="_list">List of sources</param>
        /// <returns>Index of source in list</returns>
        public int GetSourceIndexByID(int _id, List<SoundDictionary> _list)
        {
            for(int i = 0; i < _list.Count; i++)
            {
                if (_list[i].ID == _id)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Call it to get source index by name
        /// </summary>
        /// <param name="_name">Name of source type</param>
        /// <returns>Index of source</returns>
        public int GeSourceIndexByName(string _name)
        {
            for (int i = 0; i < AwailableSources.Count; i++)
            {
                if (AwailableSources[i].Name == _name)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Call it to get first SFXPlayer from some list
        /// </summary>
        /// <param name="_dict"></param>
        /// <returns></returns>
        public SFXPlayer GetFirstPlayer(SoundDictionary _dict)
        {
            if (_dict.Value.Count > 0)
            {
                SFXPlayer _pl = _dict.Value.First.Value;
                _dict.Value.RemoveFirst();

                int _unawailableIndex = GetSourceIndexByID(_dict.ID, unawailableSources);
                if (_unawailableIndex > -1)
                {
                    unawailableSources[_unawailableIndex].Value.AddLast(_pl);
                }
                else
                {
                    SoundDictionary _newDict = new SoundDictionary();
                    _newDict.ID = _dict.ID;
                    _newDict.Name = _dict.Name;
                    _newDict.Value.AddLast(_pl);

                    unawailableSources.Add(_newDict);
                }

                return _pl;
            }
            return null;
        }

        /// <summary>
        /// Call it to return SFXPlayer to pool
        /// </summary>
        /// <param name="_player"></param>
        public void ReturnPlayer(SFXPlayer _player, bool _dontRemove = false)
        {
            _player.gameObject.SetActive(false);
            int _id = _player.ID;
            foreach(SoundDictionary _dict in unawailableSources)
            {
                if (_dict.ID == _id)
                {
                    if (_dict.Value.Contains(_player))
                    {
                        if (!_dontRemove)
                        {
                            //Log.Write("Disabling player "+ _dontRemove, LogColors.White);
                            _dict.Value.Remove(_player);
                        }

                        int _awailableIndex = GetSourceIndexByID(_id, AwailableSources);
                        if (_awailableIndex > -1 && !AwailableSources[_awailableIndex].Value.Contains(_player))
                        {
                            AwailableSources[_awailableIndex].Value.AddLast(_player);
                        }
                    }
                    return;
                }
            } 
        }

        /// <summary>
        /// Call it to destroy all dummies
        /// </summary>
        /// <param name="_index"></param>
        public void DestroyAndClearPlayers(int _index)
        {
            foreach(SFXPlayer _player in AwailableSources[_index].Value)
            {
                if(_player!=null)
                    DestroyImmediate(_player.gameObject);
            }
            AwailableSources[_index].Value.Clear();
        }

        /// <summary>
        /// Call it to create all dummies
        /// </summary>
        /// <param name="_index"></param>
        public void CreatePlayers(SoundDictionary _dict)
        {  
            for(int i = 0; i < _dict.Count; i++)
            {
                Transform _newDummie = Instantiate(_dict.Template, transform.position, transform.rotation) as Transform;
                _newDummie.parent = transform;
                _newDummie.name += UnityEngine.Random.Range(0, 999999);
                SFXPlayer _player = _newDummie.GetComponent<SFXPlayer>();
                if (_player != null)
                {
                    _player.Name = _dict.Name;
                    _player.ID = _dict.ID;
                    _dict.Value.AddLast(_player);
                    _newDummie.gameObject.SetActive(false);
                }
                else
                {
                    Log.Write("There is no SFXPlayer on template :" + _dict.Template.name, LogColors.Red);
                    DestroyImmediate(_newDummie.gameObject);
                    break;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Serializable class of music dictionary
    /// </summary>
    [Serializable]
    public class MusicDictionary
    { 
        public string Key;
        [SerializeField]
        private List<AudioClip> value;
        public List<AudioClip> Value
        {
            get
            {
                if (value == null)
                    value = new List<AudioClip>();

                return value;
            }
            set
            {
                this.value =  value;
            }
        }

        public bool ShowDict;
    }

    /// <summary>
    /// Serializable class of sound dictionary
    /// </summary>
    [Serializable]
    public class SoundDictionary
    {
        /// <summary>
        /// Name of dictionary
        /// </summary>
        public string Name;
        /// <summary>
        /// Id of dictionary
        /// </summary>
        public int ID;
        /// <summary>
        /// Template for dictionary
        /// </summary>
        public Transform Template;
        /// <summary>
        /// Count of template instantces
        /// </summary>
        public int Count;
        [SerializeField]
        private LinkedList<SFXPlayer> value;
        /// <summary>
        /// List of objects
        /// </summary>
        public LinkedList<SFXPlayer> Value
        {
            get
            {
                if (value == null)
                    value = new LinkedList<SFXPlayer>();

                return value;
            }
            set
            {
                this.value = value;
            }
        } 

        /// <summary>
        /// Edtio variable - show or not dict parameters
        /// </summary>
        public bool ShowDict;
    }
}
