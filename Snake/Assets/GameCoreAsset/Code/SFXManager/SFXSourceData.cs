using System; 
using System.Collections.Generic;
using UnityEngine;

namespace SFXManagerModule
{
    /// <summary>
    /// SFX source data with different parameters
    /// </summary>
    [Serializable]
    public class SFXSourceData
    {
        /// <summary>
        /// Range for SFXPlayer
        /// </summary>
        public float Range = -1f;
        /// <summary>
        /// Volume for SFXPlayer
        /// </summary>
        public float Volume = 1f;
        /// <summary>
        /// Pitch for SFXPlayer
        /// </summary>
        public float Pitch = 1f;
        /// <summary>
        /// Loopt mode for SFXPlayer
        /// </summary>
        public bool Loop = false;
        /// <summary>
        /// List of AudioClips fo SFXPlayer
        /// </summary>
        public List<AudioClip> Clips = new List<AudioClip>();
        /// <summary>
        /// Flag if we have to random volume pitch
        /// </summary>
        public bool RandomVolumeAndPitch = true;
        /// <summary>
        /// Max offset of volume after randoming
        /// </summary>
        public float RandomVolumeValue = 0.3f;
        /// <summary>
        /// Max offset of pitch after randoming
        /// </summary>
        public float RandomPitchValue = 0.3f;
    }
}