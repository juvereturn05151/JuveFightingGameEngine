/*
 Author: Ju-ve Chankasemporn
 E-mail: juvereturn@gmail.com
 */

using UnityEngine;

namespace FightingGameEngine
{
    public enum AnimationID 
    {
        Idle1,
        Idle2, 
        Idle3, 
        Idle4,
        Idle5,
        Idle6,
    }

    [System.Serializable]
    public class AnimationData
    {
        public AnimationID animationID;
        public Sprite sprite;
        public AudioClip audioClip;
    }
}
