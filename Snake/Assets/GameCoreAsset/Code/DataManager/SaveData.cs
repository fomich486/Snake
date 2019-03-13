using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    /// <summary>
    /// Class for storing save data
    /// </summary>
    [Serializable]
    public class SaveData
    {
        /// <summary>
        /// String that can be parsed for date of data saving
        /// </summary>
        public string DataSaveTime;
        /// <summary>
        /// Music volume value (0..1)
        /// </summary>
        public float MusicVolume;
        /// <summary>
        /// Sound volume value (0..1)
        /// </summary>
        public float SoundVolume;
        //put here any variable you need to store in save data
    }

    //actually, it's ok to store here Serializable classes or structures, used in save file
}