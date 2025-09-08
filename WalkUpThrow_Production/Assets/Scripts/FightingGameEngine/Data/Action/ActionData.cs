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
        Lose,
        Win,
        AttemptThrow,
        Throw,
        BeingGrabbed,
    }

    public enum ActionType
    {
        Movement,
        Attack,
        Damage,
        Guard,
        WinOrLose,
        AttemptThrow,
    }

    public enum AttackID
    {
        Nothing = -1,
        CrouchMediumKick,
        Hadouken,
        AttemptThrow
    }

    public abstract class FrameData
    {
        public Vector2Int startEndFrame;
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
    public class GrabboxData : CollisionBoxData
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
        public float velocity_y;
    }

    [System.Serializable]
    public class CancelData : FrameData
    {
        public bool buffer;
        public bool execute;
        public List<ActionID> actionID = new List<ActionID>();
    }

    [System.Serializable]
    public class ConsequenceData : FrameData
    {
        public ActionID actionID;
    }

    [CreateAssetMenu]
    public class ActionData : ScriptableObject
    {
        public ActionID actionID;
        public ActionType actionType;
        public int frameCount;
        public bool isLoop;
        public int loopFromFrame;
        public AnimationFrameDataSet animationFrameDataSet;
        public HitboxData[] hitboxes;
        public GrabboxData[] grabboxes;
        public HurtboxData[] hurtboxes;
        public PushboxData[] pushboxes;
        public MovementData[] movements;
        public CancelData[] cancels;
        public ConsequenceData[] opponentConsequences;
        public bool alwaysCancelable;

        public Sprite GetAnimationSprite(int frame)
        {
            foreach (var data in animationFrameDataSet.animationDataList)
            {
                if (frame >= data.startEndFrame.x && frame <= data.startEndFrame.y)
                {
                    return data.sprite;
                }
            }

            return null;
        }

        public List<HitboxData> GetHitboxData(int frame)
        {
            var hb = new List<HitboxData>();

            if(this.hitboxes == null) return hb;

            foreach (var data in this.hitboxes)
            {
                if (frame >= data.startEndFrame.x && frame <= data.startEndFrame.y) 
                {
                    hb.Add(data);
                }
            }

            return hb;
        }

        public List<GrabboxData> GetGrabboxData(int frame)
        {
            var gb = new List<GrabboxData>();

            if (this.grabboxes == null) return gb;

            foreach (var data in this.grabboxes)
            {
                if (frame >= data.startEndFrame.x && frame <= data.startEndFrame.y)
                {
                    gb.Add(data);
                }
            }

            return gb;
        }

        public List<HurtboxData> GetHurtboxData(int frame)
        {
            var gb = new List<HurtboxData>();

            foreach (var data in this.hurtboxes)
            {
                if (frame >= data.startEndFrame.x && frame <= data.startEndFrame.y) 
                {
                    gb.Add(data);
                }
            }

            return gb;
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

        public List<ConsequenceData> GetConsequenceDatas(int frame)
        {
            var cd = new List<ConsequenceData>();

            if (this.opponentConsequences == null) return cd;

            foreach (var data in this.opponentConsequences)
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

