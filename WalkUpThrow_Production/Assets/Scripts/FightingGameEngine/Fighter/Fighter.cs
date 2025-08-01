using System.Collections.Generic;
using UnityEngine;

namespace FightingGameEngine 
{
    public class Fighter : MonoBehaviour
    {
        public Vector2 position;
        public float velocity_x;
        public bool isFaceRight;

        public List<Hitbox> hitboxes = new List<Hitbox>();
        public List<Hurtbox> hurtboxes = new List<Hurtbox>();
        public Pushbox pushbox;

        private FighterData fighterData;

        public bool isDead { get { return vitalHealth <= 0; } }
        public int vitalHealth { get; private set; }

        public ActionID currentActionID { get; private set; }
        public int currentActionFrame { get; private set; }
        public int currentActionFrameCount { get { return fighterData.actions[currentActionID].frameCount; } }
        private bool isActionEnd { get { return (currentActionFrame >= fighterData.actions[currentActionID].frameCount); } }
        public bool isAlwaysCancelable { get { return fighterData.actions[currentActionID].alwaysCancelable; } }

        public int currentActionHitCount { get; private set; }

        public int currentHitStunFrame { get; private set; }
        public bool isInHitStun { get { return currentHitStunFrame > 0; } }

        private static int inputRecordFrame = 180;
        private int[] input = new int[inputRecordFrame];
        private int[] inputDown = new int[inputRecordFrame];
        private int[] inputUp = new int[inputRecordFrame];

        private bool isInputBackward;
        private ActionID bufferActionID = ActionID.Nothing;
        private bool hasWon = false;

        /// <summary>
        /// Setup fighter at the start of battle
        /// </summary>
        /// <param name="fighterData"></param>
        /// <param name="startPosition"></param>
        /// <param name="isPlayerOne"></param>
        public void SetupBattleStart(FighterData fighterData, Vector2 startPosition, bool isPlayerOne)
        {
            this.fighterData = fighterData;
            //position = startPosition;
            isFaceRight = isPlayerOne;

            vitalHealth = 1;
            hasWon = false;

            velocity_x = 0;

            ClearInput();

            SetCurrentAction(ActionID.Stand);
        }

        /// <summary>
        /// Update action frame
        /// </summary>
        public void IncrementActionFrame()
        {
            // If fighter is in hit stun then the action frame stay the same
            if (currentHitStunFrame > 0)
            {
                currentHitStunFrame--;
                return;
            }

            currentActionFrame++;

            // For loop motion (winning pose etc.) set action frame back to loop start frame
            if (isActionEnd)
            {
                if (fighterData.actions[currentActionID].isLoop)
                {
                    currentActionFrame = fighterData.actions[currentActionID].loopFromFrame;
                }
            }
        }

        /// <summary>
        /// Action request for intro state ()
        /// </summary>
        public void UpdateIntroAction()
        {
            RequestAction(ActionID.Stand);
        }

        /// <summary>
        /// Update action request
        /// </summary>
        public void UpdateActionRequest()
        {
            // If won then just request win animation
            //if (hasWon)
            //{
            //    RequestAction((int)CommonActionID.WIN);
            //    return;
            //}

            // If there is any buffer action, set that to current action
            // Use for canceling normal to special attack
            //if (bufferActionID != -1
            //    && canCancelAttack()
            //    && currentHitStunFrame <= 0)
            //{
            //    SetCurrentAction(bufferActionID);
            //    bufferActionID = -1;
            //    return;
            //}

            //var isForward = IsForwardInput(input[0]);
            //var isBackward = IsBackwardInput(input[0]);
            //bool isAttack = IsAttackInput(inputDown[0]);
            //if (CheckSpecialAttackInput())
            //{
            //    if (isBackward || isForward)
            //        RequestAction((int)CommonActionID.B_SPECIAL);
            //    else
            //        RequestAction((int)CommonActionID.N_SPECIAL);
            //}
            //else if (isAttack)
            //{
            //    if ((currentActionID == (int)CommonActionID.N_ATTACK ||
            //        currentActionID == (int)CommonActionID.B_ATTACK) &&
            //        !isActionEnd)
            //        RequestAction((int)CommonActionID.N_SPECIAL);
            //    else
            //    {
            //        if (isBackward || isForward)
            //            RequestAction((int)CommonActionID.B_ATTACK);
            //        else
            //            RequestAction((int)CommonActionID.N_ATTACK);
            //    }
            //}

            // for proximity guard check
            //isInputBackward = isBackward;

            //if (isForward && isBackward)
            //{
            //    RequestAction((int)CommonActionID.STAND);
            //}
            //else if (isForward)
            //{
            //    RequestAction((int)CommonActionID.FORWARD);
            //}
            //else if (isBackward)
            //{
            //    if (isReserveProximityGuard)
            //        RequestAction((int)CommonActionID.GUARD_PROXIMITY);
            //    else
            //        RequestAction((int)CommonActionID.BACKWARD);
            //}
            //else
            //{
            //    RequestAction((int)CommonActionID.STAND);
            //}
        }

        public void RequestWinAction()
        {
            hasWon = true;
        }

        public void UpdateBoxes()
        {
            ApplyCurrentActionData();
        }

        /// <summary>
        /// Apply position changed to all variable
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ApplyPositionChange(float x, float y)
        {
            position.x += x;
            position.y += y;

            foreach (var hitbox in hitboxes)
            {
                hitbox.rect.x += x;
                hitbox.rect.y += y;
            }

            foreach (var hurtbox in hurtboxes)
            {
                hurtbox.rect.x += x;
                hurtbox.rect.y += y;
            }

            pushbox.rect.x += x;
            pushbox.rect.y += y;
        }

        /// <summary>
        /// Request action, if condition is met then set the requested action to current action
        /// </summary>
        /// <param name="actionID"></param>
        /// <param name="startFrame"></param>
        /// <returns></returns>
        public bool RequestAction(ActionID actionID, int startFrame = 0)
        {
            if (isActionEnd)
            {
                SetCurrentAction(actionID, startFrame);
                return true;
            }

            if (currentActionID == actionID)
            {
                return false;
            }

            if (fighterData.actions[currentActionID].alwaysCancelable)
            {
                SetCurrentAction(actionID, startFrame);
                return true;
            }
            else
            {
                foreach (var cancelData in fighterData.actions[currentActionID].GetCancelData(currentActionFrame))
                {
                    if (cancelData.actionID.Contains(actionID))
                    {
                        if (cancelData.execute)
                        {
                            bufferActionID = actionID;
                            return true;
                        }
                        else if (cancelData.buffer)
                        {
                            bufferActionID = actionID;
                        }
                    }
                }
            }

            return false;
        }

        public Sprite GetCurrentAnimationSprite()
        {
            var animationData = fighterData.actions[currentActionID].GetAnimationData(currentActionFrame);
            if (animationData == null) 
            {
                return null;
            }

            return fighterData.animationData[animationData.animationID].sprite;
        }

        public void ClearInput()
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = 0;
                inputDown[i] = 0;
                inputUp[i] = 0;
            }
        }

        /// <summary>
        /// Set current action
        /// </summary>
        /// <param name="actionID"></param>
        /// <param name="startFrame"></param>
        private void SetCurrentAction(ActionID actionID, int startFrame = 0)
        {
            currentActionID = actionID;
            currentActionFrame = startFrame;

            currentActionHitCount = 0;
            bufferActionID = ActionID.Nothing;
        }

        /// <summary>
        /// Copy data from current action and convert relative box position with fighter position
        /// </summary>
        private void ApplyCurrentActionData()
        {
            hitboxes.Clear();
            hurtboxes.Clear();

            foreach (var hitbox in fighterData.actions[currentActionID].GetHitboxData(currentActionFrame))
            {
                var box = new Hitbox();
                box.rect = TransformToFightRect(hitbox.rect, position, isFaceRight);
                box.attackID = hitbox.attackID;
                hitboxes.Add(box);
            }

            foreach (var hurtbox in fighterData.actions[currentActionID].GetHurtboxData(currentActionFrame))
            {
                var box = new Hurtbox();
                Rect rect = hurtbox.useBaseRect ? fighterData.baseHurtBoxRect : hurtbox.rect;
                box.rect = TransformToFightRect(rect, position, isFaceRight);
                hurtboxes.Add(box);
            }

            var pushBoxData = fighterData.actions[currentActionID].GetPushboxData(currentActionFrame);
            if (pushBoxData == null)
            {
                Debug.LogError("Pushbox data is null for currentActionFrame: " + currentActionFrame);
            }
            pushbox = new Pushbox();
            Rect pushRect = pushBoxData.useBaseRect ? fighterData.basePushBoxRect : pushBoxData.rect;

            pushbox.rect = TransformToFightRect(pushRect, position, isFaceRight);
        }

        /// <summary>
        /// Convert relative box position with current fighter position
        /// </summary>
        /// <param name="dataRect"></param>
        /// <param name="basePosition"></param>
        /// <param name="isFaceRight"></param>
        /// <returns></returns>
        private Rect TransformToFightRect(Rect dataRect, Vector2 basePosition, bool isFaceRight)
        {
            var sign = isFaceRight ? 1 : -1;

            var fightPosRect = new Rect();
            fightPosRect.x = basePosition.x + (dataRect.x * sign);
            fightPosRect.y = basePosition.y + dataRect.y;
            fightPosRect.width = dataRect.width;
            fightPosRect.height = dataRect.height;

            return fightPosRect;
        }
    }
}

