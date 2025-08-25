using System.Collections.Generic;
using UnityEngine;

namespace FightingGameEngine 
{
    public class Fighter : MonoBehaviour
    {
        [SerializeField] 
        private CharacterInputManager _inputManager;
        public CharacterInputManager InputManager => _inputManager;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

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
        private bool isHitThisFrame = false;

        private void Update()
        {
            // Replace your old input recording with this:
            RecordInputs();
        }


        private void FixedUpdate()
        {
            UpdateSprite();
        }

        void UpdateSprite()
        {
            if (_spriteRenderer != null)
            {
                var sprite = GetCurrentAnimationSprite();
                if (sprite != null) 
                {
                    _spriteRenderer.sprite = sprite;
                }
            }
        }

        private void RecordInputs()
        {
            if (_inputManager == null) 
            {
                return;
            }

            // Shift previous inputs
            for (int i = inputRecordFrame - 1; i > 0; i--)
            {
                input[i] = input[i - 1];
                inputDown[i] = inputDown[i - 1];
                inputUp[i] = inputUp[i - 1];
            }

            // Get new inputs from InputManager
            input[0] = 0;
            if (_inputManager.GetInput(InputDefine.Left)) input[0] |= (int)InputDefine.Left;
            if (_inputManager.GetInput(InputDefine.Right)) input[0] |= (int)InputDefine.Right;
            if (_inputManager.GetInput(InputDefine.Attack)) input[0] |= (int)InputDefine.Attack;

            inputDown[0] = 0;
            if (_inputManager.GetInputDown(InputDefine.Left)) inputDown[0] |= (int)InputDefine.Left;
            if (_inputManager.GetInputDown(InputDefine.Right)) inputDown[0] |= (int)InputDefine.Right;
            if (_inputManager.GetInputDown(InputDefine.Attack)) inputDown[0] |= (int)InputDefine.Attack;

            inputUp[0] = 0;
            // Note: You'll need to add GetInputUp to InputManager if needed
        }

        private bool IsForwardInput(int input)
        {
            return isFaceRight ?
                (input & (int)InputDefine.Right) != 0 :
                (input & (int)InputDefine.Left) != 0;
        }

        private bool IsBackwardInput(int input)
        {
            return isFaceRight ?
                (input & (int)InputDefine.Left) != 0 :
                (input & (int)InputDefine.Right) != 0;
        }

        public bool IsAttackInput(int input)
        {
            return (input & (int)InputDefine.Attack) != 0;
        }

        /// <summary>
        /// Setup fighter at the start of battle
        /// </summary>
        /// <param name="fighterData"></param>
        /// <param name="startPosition"></param>
        /// <param name="isPlayerOne"></param>
        public void SetupBattleStart(FighterData fighterData, Vector2 startPosition, bool isPlayerOne)
        {
            this.fighterData = fighterData;
            isFaceRight = isPlayerOne;

            vitalHealth = 1;
            hasWon = false;

            ClearInput();

            SetCurrentAction(ActionID.Idle);
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
        /// UpdateInput
        /// </summary>
        /// <param name="inputData"></param>
        public void UpdateInput(InputData inputData)
        {
            // Shift input history by 1 frame
            for (int i = input.Length - 1; i >= 1; i--)
            {
                input[i] = input[i - 1];
                inputDown[i] = inputDown[i - 1];
                inputUp[i] = inputUp[i - 1];
            }

            // Insert new input data at the front of the buffer
            input[0] = inputData.input;
            inputDown[0] = (input[0] ^ input[1]) & input[0];
            inputUp[0] = (input[0] ^ input[1]) & ~input[0];
        }


        /// <summary>
        /// Action request for intro state ()
        /// </summary>
        public void UpdateIntroAction()
        {
            RequestAction(ActionID.Idle);
        }

        /// <summary>
        /// Update action request
        /// </summary>
        public void UpdateActionRequest()
        {
            if (_inputManager == null) 
            {
                //Hack
                if (currentActionID == ActionID.Hurt && isActionEnd) 
                {
                    RequestAction(ActionID.Idle);
                }

                return;
            }

            // If won then just request win animation
            //if (hasWon)
            //{
            //    RequestAction((int)CommonActionID.WIN);
            //    return;
            //}

            if (currentActionID != ActionID.Hadouken && _inputManager.CheckHadokenMotion())
            {
                RequestAction(ActionID.Hadouken);
                return;
            }


            var isForward = IsForwardInput(_inputManager.CurrentInput.input);
            var isBackward = IsBackwardInput(_inputManager.CurrentInput.input);
            bool isAttack = _inputManager.GetInputDown(InputDefine.Attack);
            bool isSpecial = _inputManager.GetInputDown(InputDefine.Special);

            if (isAttack)
            {
                RequestAction(ActionID.Cr_Mk);
            }
            else if (isSpecial) 
            {
                RequestAction(ActionID.Hadouken);
            }

                // for proximity guard check
            isInputBackward = isBackward;

            if (isForward && isBackward)
            {
                RequestAction(ActionID.Idle);
            }
            else if (isForward)
            {
                RequestAction(ActionID.Forward);
            }
            else if (isBackward)
            {
                RequestAction(ActionID.Backward);
            }
            else
            {
                RequestAction(ActionID.Idle);
            }
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
        /// Update character position
        /// </summary>
        public void UpdateMovement()
        {
            if (isInHitStun)
                return;

            // Position changes from walking forward and backward
            var sign = isFaceRight ? 1 : -1;
            float newX = transform.position.x;
            if (currentActionID == ActionID.Forward)
            {
                newX += fighterData.forwardMoveSpeed * sign * Time.deltaTime;
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                return;
            }
            else if (currentActionID == ActionID.Backward)
            {
                newX -= fighterData.backwardMoveSpeed * sign * Time.deltaTime;
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                return;
            }

            // Position changes from action data
            var movementData = fighterData.actions[currentActionID].GetMovementData(currentActionFrame);
            if (movementData != null)
            {
                if (movementData.velocity_x != 0)
                {
                    newX += movementData.velocity_x * sign * Time.deltaTime;
                    transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                }
            }
        }

        /// <summary>
        /// Apply position changed to all variable
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ApplyPositionChange(float x, float y)
        {
            transform.position += new Vector3(x, y, 0f);
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

            //Debug.Log($"=== APPLY CURRENT ACTION DATA ===");
            //Debug.Log($"Current Action: {currentActionID}, Frame: {currentActionFrame}, Facing Right: {isFaceRight}");
            //Debug.Log($"Fighter Position: {transform.position}");

            foreach (var hitbox in fighterData.actions[currentActionID].GetHitboxData(currentActionFrame))
            {
                var box = new Hitbox();
                box.rect = UpdateCollisionBox(hitbox.rect, transform.position, isFaceRight);
                box.attackID = hitbox.attackID;
                hitboxes.Add(box);

                //Debug.Log($"[Hitbox] Local: {hitbox.rect}, World: {box.rect}, AttackID: {box.attackID}");
                //Debug.Log($"          World xMin: {box.rect.xMin}, yMin: {box.rect.yMin}, xMax: {box.rect.xMax}, yMax: {box.rect.yMax}");
            }

            foreach (var hurtbox in fighterData.actions[currentActionID].GetHurtboxData(currentActionFrame))
            {
                var box = new Hurtbox();
                Rect rect = hurtbox.useBaseRect ? fighterData.baseHurtBoxRect : hurtbox.rect;
                box.rect = UpdateCollisionBox(rect, transform.position, isFaceRight);
                hurtboxes.Add(box);

                //Debug.Log($"[Hurtbox] Local: {rect}, World: {box.rect}, UseBase: {hurtbox.useBaseRect}");
                //Debug.Log($"           World xMin: {box.rect.xMin}, yMin: {box.rect.yMin}, xMax: {box.rect.xMax}, yMax: {box.rect.yMax}");
            }

            var pushBoxData = fighterData.actions[currentActionID].GetPushboxData(currentActionFrame);
            if (pushBoxData == null)
            {
                Debug.LogError("Pushbox data is null for currentActionFrame: " + currentActionFrame);
            }
            else
            {
                pushbox = new Pushbox();
                Rect pushRect = pushBoxData.useBaseRect ? fighterData.basePushBoxRect : pushBoxData.rect;
                pushbox.rect = UpdateCollisionBox(pushRect, transform.position, isFaceRight);

                //Debug.Log($"[Pushbox] Local: {pushRect}, World: {pushbox.rect}, UseBase: {pushBoxData.useBaseRect}");
                //Debug.Log($"           World xMin: {pushbox.rect.xMin}, yMin: {pushbox.rect.yMin}, xMax: {pushbox.rect.xMax}, yMax: {pushbox.rect.yMax}");
            }

           // Debug.Log("==================================");
        }

        private Rect UpdateCollisionBox(Rect dataRect, Vector2 basePosition, bool isFaceRight)
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

