using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFXManagerModule
{
    /// <summary>
    /// This is class that perform SFX player. 
    /// </summary>
    public class SFXPlayer : MonoBehaviour
    {
        #region VARIABLES
        /// <summary>
        /// Link to AudioSource for playing sound
        /// </summary>
        [SerializeField]
        private AudioSource source;
        /// <summary>
        /// Name of source type
        /// </summary>
        public string Name;
        /// <summary>
        /// ID of source type
        /// </summary>
        public int ID;
        /// <summary>
        /// Flag is player active
        /// </summary>
        private bool active = false;
        /// <summary>
        /// private inst
        /// </summary>
        private SFXSourceData data;
        /// <summary>
        /// SFX source data - contains custom range, volume, pitch, etc. 
        /// </summary>
        public SFXSourceData Data
        {
            get
            {
                if (data == null)
                    data = new SFXSourceData();

                return data;
            }
            set
            {
                data = value;
            }
        }
        /// <summary>
        /// Coroutine of sound chaning(looping?)
        /// </summary>
        Coroutine coroutine;
        /// <summary>
        /// Flag it we have to random pitch and volume
        /// </summary>
        public bool RandomVolumePitch = false;
        /// <summary>
        /// 0.4 =  random between 60%-140% of base volume
        /// </summary>
        public float RandomVolumeValue = 0.4f;
        /// <summary>
        /// 0.4 = random between 60%-140% of base pitch
        /// </summary>
        public float RandomPitchValue = 0.1f;
        #endregion

        #region METHODS
        /// <summary>
        /// Call it to set idintifierd(ID and NAME)
        /// </summary>
        /// <param name="_id">ID of source</param>
        /// <param name="_name">Name of source</param>
        public void SetInditifiers(int _id, string _name)
        {
            Name = _name;
            ID = _id; 
        }

        /// <summary>
        /// Call it to start playing some of selected clips
        /// </summary>
        /// <param name="_data"></param>
        public void StartPlaying(SFXSourceData _data)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            //cashing data
            Data = _data;
            //randoming sound
            int _randomedIndex = (int)Random.Range(0, Data.Clips.Count);
            //of course it becomes active
            active = true;
            //Log.Write("Generation index:" + _randomedIndex + " from count :" + Data.Clips.Count); 
            if (Data.Clips == null || Data.Clips.Count == 0 || Data.Clips[_randomedIndex] == null)
            {
                //Log.Write("There is null AudioClip at this position!",LogColors.Yellow);
                return;
            } 
            source.clip = Data.Clips[_randomedIndex];
              
            source.loop = Data.Loop;

            source.Play();
            //if range not basic - set it from data
            if(Data.Range > 0)
                source.maxDistance = Data.Range;
            //if not loop - start waiting for the end
            if(!source.loop)
            {
                coroutine = StartCoroutine(ProcessSoundChanging());
            } 
        } 

        /// <summary>
        /// Call it to stop playing some sound
        /// </summary>
        public void StopPlaying()
        {
            source.Stop();
            active = false; 
        }

        /// <summary>
        /// Call it to end sound playing
        /// </summary>
        public void EndPlaying()
        {
            active = false;
            source.Stop();
            FakeChildren _fk = gameObject.GetComponent<FakeChildren>();
            if (_fk != null)
            {
                Destroy(_fk);
            }
            if (coroutine != null)
                StopCoroutine(coroutine);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Call it to play some sound(without 3D effect)
        /// </summary>
        /// <param name="_clip">Sound to play</param>
        public void PlaySound(AudioClip _clip, float _volume = 1f)
        {
            if (!source.gameObject.activeSelf)
                source.gameObject.SetActive(true);

            if (RandomVolumePitch)
            {
                Data.Pitch= 1f + Random.Range(-RandomPitchValue, RandomPitchValue);
                Data.Volume = 1f + Random.Range(-RandomVolumeValue, RandomVolumeValue);
                Updateparameters();
            }
            source.PlayOneShot(_clip, _volume);
        }

        /// <summary>
        /// Call it to play some sound long
        /// </summary>
        /// <param name="_clip"></param>
        public void PlaySoundLong(AudioClip _clip)
        {
            if (RandomVolumePitch)
            {
                Data.Pitch = 1f + Random.Range(-RandomPitchValue, RandomPitchValue);
                Data.Volume = 1f + Random.Range(-RandomVolumeValue, RandomVolumeValue);
                Updateparameters();
            }
            Updateparameters();
            source.clip = _clip;
            source.Play();
            Log.Write("Playing sound!"+ _clip.name  + " player:"+gameObject.name + " with loop mode:"+source.loop);
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
                    PlaySound(_clips[_randomedIndex]);
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
                    PlaySound(_clips[_randomedIndex]);
                }
            }
            else
            {
                Log.Write("You passed null AudioClip[] to play!",LogColors.Yellow);
            }
        }

        /// <summary>
        /// Called when we disable this GameObject
        /// </summary>
        void OnDisable()
        { 
            //Disable(); 
        }

        /// <summary>
        /// Call it to disable player and return it back to manager
        /// </summary>
        public void Disable(bool _dontRemove = false)
        {
            active = false;
            source.Stop();
            Data = null;
            RandomVolumePitch = false;

            if (SFXManager.Instance != null)
                SFXManager.Instance.ReturnPlayer(this, _dontRemove);

            FakeChildren _fk = transform.GetComponent<FakeChildren>();
            if (_fk != null)
                Destroy(_fk);
        }

        /// <summary>
        /// Called each frame
        /// </summary>
        private void Update()
        {
            Updateparameters();
        } 

        /// <summary>
        /// Updating parameters, that depends on local multipliers and global options
        /// </summary>
        void Updateparameters()
        {
            if(source.volume!=Data.Volume * SFXManager.Instance.SoundVolume)
                source.volume = Data.Volume * SFXManager.Instance.SoundVolume;
            if (source.pitch != Data.Pitch * SFXManager.Instance.SoundPitch)
                source.pitch = Data.Pitch * SFXManager.Instance.SoundPitch;
            if (source.loop != Data.Loop)
                source.loop = Data.Loop;    
        }

        /// <summary>
        /// Call it to process sound changing (soft)
        /// </summary> 
        IEnumerator ProcessSoundChanging()
        { 
            while (source.isPlaying)
                yield return new WaitForEndOfFrame();
             
            if (!active)
                yield break;
             
            int _randomedIndex = (int)Random.Range(0, Data.Clips.Count);
            if (Data.Clips == null || Data.Clips[_randomedIndex] == null)
            {
                Log.Write("There is null AudioClip at this position!",LogColors.Yellow);
            } 
            source.clip = Data.Clips[_randomedIndex];
            source.Play();
             
            if (!source.loop)
            { 
                yield return new WaitForEndOfFrame();
                StartCoroutine(ProcessSoundChanging());
            }
        } 
        #endregion
    }
}
