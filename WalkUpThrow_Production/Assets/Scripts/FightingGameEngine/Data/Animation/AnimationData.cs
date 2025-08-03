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
        Idle7,
        Forward1,
        Forward2,
        Forward3,
        Forward4,
        Forward5,
        Forward6,
        Forward7,
        Forward8,
        Backward1,
        Backward2,
        Backward3,
        Backward4,
        Backward5,
        Backward6,
        Backward7,
        Backward8
    }

    [System.Serializable]
    public class AnimationData
    {
        public AnimationID animationID;
        public Sprite sprite;
        public AudioClip audioClip;
    }
}
