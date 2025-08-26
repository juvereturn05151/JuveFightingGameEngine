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
        Backward8,
        Cr_Mk1,
        Cr_Mk2,
        Cr_Mk3,
        Cr_MK4,
        Cr_Mk5,
        Cr_Mk6,
        Hurt1,
        Hurt2,
        Hurt3,
        Hadouken1,
        Hadouken2,
        Hadouken3,
        Hadouken4,
        Hadouken5,
        Hadouken6,
        Hadouken7,
        Hadouken8,
        Block1,
        Lose1,
        Lose2,
        Lose3,
        Lose4,
        Lose5,
        Lose6,
        Lose7,
        Lose8,
        Lose9,
        Lose10,
        Lose11,
        Lose12,
    }

    [System.Serializable]
    public class AnimationData
    {
        public AnimationID animationID;
        public Sprite sprite;
        public AudioClip audioClip;
    }
}
