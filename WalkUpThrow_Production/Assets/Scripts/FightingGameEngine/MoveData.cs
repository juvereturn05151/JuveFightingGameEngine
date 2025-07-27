/*
 Author: Ju-ve Chankasemporn
 E-Mail: juvereturn@gmail.com
 Date: 2023-07-27
 */

using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.PointerEventData;

namespace FightinGameEngine 
{
    [CreateAssetMenu(menuName = "FightingGame/Move")]
    public class MoveData : ScriptableObject
    {
        public string moveName;
        public AnimationClip animation;

        public int startup;
        public int active;
        public int recovery;

        public int damage;
        public int blockstun;
        public int hitstun;
        public bool causesKnockdown;
        public bool isThrow;
        public bool canCancel;
        public string nextMoveName;
        public Vector2 knockback;

        public DirectionalInput requiredDirection; 
        public InputButton button; 
        public List<HitboxFrame> hitboxFrames;
    }
}

