using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Sound
{
    [Serializable]
    public struct SongData
    {
        public AudioClip clip;
        public float bpm;
    }
}
