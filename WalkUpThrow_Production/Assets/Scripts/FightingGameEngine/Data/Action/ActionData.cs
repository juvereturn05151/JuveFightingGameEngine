/*
 Author: Ju-ve Chankasemporn
 E-mail: juvereturn@gmail.com
 */

using System.Collections.Generic;
using UnityEngine;

namespace FightingGameEngine 
{
    public enum ActionID
    {
        Nothing = -1,
        Idle,
        Forward,
        Backward,
        Cr_Mk,
        Hurt,
        Hadouken,
        Block,
        Lose
    }

    public enum ActionType
    {
        Movement,
        Attack,
        Damage,
        Guard,
        WinOrLose
    }

    public enum AttackID
    {
        Nothing = -1,
        CrouchMediumKick,
        Hadouken
    }

    public abstract class FrameData
    {
        public Vector2Int startEndFrame;
    }

    [System.Serializable]
    public class AnimationFrameData : FrameData
    {
        public AnimationID animationID;
    }

    [System.Serializable]
    public class CollisionBoxData : FrameData
    {
        public Rect rect;
        public bool useBaseRect;
    }

    [System.Serializable]
    public class HitboxData : CollisionBoxData
    {
        public AttackID attackID;
    }

    [System.Serializable]
    public class HurtboxData : CollisionBoxData
    {

    }

    [System.Serializable]
    public class PushboxData : CollisionBoxData
    {

    }

    [System.Serializable]
    public class MovementData : FrameData
    {
        public float velocity_x;
    }

    [System.Serializable]
    public class CancelData : FrameData
    {
        public bool buffer;
        public bool execute;
        public List<ActionID> actionID = new List<ActionID>();
    }

    [CreateAssetMenu]
    public class ActionData : ScriptableObject
    {
        public ActionID actionID;
        public ActionType actionType;
        public int frameCount;
        public bool isLoop;
        public int loopFromFrame;
        public AnimationFrameData[] animations;
        public HitboxData[] hitboxes;
        public HurtboxData[] hurtboxes;
        public PushboxData[] pushboxes;
        public MovementData[] movements;
        public CancelData[] cancels;
        public bool alwaysCancelable;

        public AnimationFrameData GetAnimationData(int frame)
        {
            foreach (var data in animations)
            {
                if (frame >= data.startEndFrame.x && frame <= data.startEndFrame.y) 
                {
                    return data;
                }
            }

            return null;
        }

        public List<HitboxData> GetHitboxData(int frame)
        {
            var hb = new List<HitboxData>();

            foreach (var data in this.hitboxes)
            {
                if (frame >= data.startEndFrame.x && frame <= data.startEndFrame.y) 
                {
                    hb.Add(data);
                }
            }

            return hb;
        }

        public List<HurtboxData> GetHurtboxData(int frame)
        {
            var hb = new List<HurtboxData>();

            foreach (var data in this.hurtboxes)
            {
                if (frame >= data.startEndFrame.x && frame <= data.startEndFrame.y) 
                {
                    hb.Add(data);
                }
            }

            return hb;
        }

        public PushboxData GetPushboxData(int frame)
        {
            foreach (var data in this.pushboxes)
            {
                if (frame >= data.startEndFrame.x && frame <= data.startEndFrame.y) 
                {
                    return data;
                }
            }

            return null;
        }

        public MovementData GetMovementData(int frame)
        {
            foreach (var data in this.movements)
            {
                if (frame >= data.startEndFrame.x && frame <= data.startEndFrame.y) 
                {
                    return data;
                }
            }

            return null;
        }

        public List<CancelData> GetCancelData(int frame)
        {
            var cd = new List<CancelData>();

            foreach (var data in this.cancels)
            {
                if (frame >= data.startEndFrame.x && frame <= data.startEndFrame.y) 
                {
                    cd.Add(data);
                }
            }

            return cd;
        }
    }
}

