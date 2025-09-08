using FightingGameEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace WalkUpThrow
{
    public class BattleCore : MonoBehaviour
    {
        public enum RoundStateType
        {
            Stop,
            Intro,
            Fight,
            KO,
            End,
        }

        [SerializeField]
        private float _battleAreaWidth = 10f;
        public float battleAreaWidth { get { return _battleAreaWidth; } }

        [SerializeField]
        private float _battleAreaMaxHeight = 2f;
        public float battleAreaMaxHeight { get { return _battleAreaMaxHeight; } }

        [SerializeField]
        private List<FighterData> fighterDataList = new List<FighterData>();

        private float timer = 0;
        private uint maxRoundWon = 3;

        [SerializeField]
        public Fighter fighter1;
        [SerializeField]
        public Fighter fighter2;

        [SerializeField]
        GameplaySceneUIManager gameplaySceneUIManager;

        public uint fighter1RoundWon { get; private set; }
        public uint fighter2RoundWon { get; private set; }

        public List<Fighter> fighters { get { return _fighters; } }
        private List<Fighter> _fighters = new List<Fighter>();

        private float roundStartTime;
        private int frameCount;

        public RoundStateType roundState { get { return _roundState; } }
        private RoundStateType _roundState = RoundStateType.Stop;

        private static uint maxRecordingInputFrame = 60 * 60 * 5;
        private InputData[] recordingP1Input = new InputData[maxRecordingInputFrame];
        private InputData[] recordingP2Input = new InputData[maxRecordingInputFrame];
        private uint currentRecordingInputIndex = 0;

        private InputData[] lastRoundP1Input = new InputData[maxRecordingInputFrame];
        private InputData[] lastRoundP2Input = new InputData[maxRecordingInputFrame];
        private uint currentReplayingInputIndex = 0;
        private uint lastRoundMaxRecordingInput = 0;
        private bool isReplayingLastRoundInput = false;

        private int roundNumber = 1;
        private float introStateTime = 3f;
        private float koStateTime = 2f;
        private float endStateTime = 3f;
        private float endStateSkippableTime = 1.5f;

        void Awake()
        {
            // Setup dictionary from ScriptableObject data
            fighterDataList.ForEach((data) => data.SetupDictionary());

            _fighters.Add(fighter1);
            _fighters.Add(fighter2);
        }

        void Start()
        {
            if (GameInputManager.Instance.player1Input)
            {
                fighter1.GetComponentInChildren<CharacterInputManager>().AssignInput(GameInputManager.Instance.player1Input);
            }


            if (GameInputManager.Instance.player2Input)
            {
                fighter2.GetComponentInChildren<CharacterInputManager>().AssignInput(GameInputManager.Instance.player2Input);
            }

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0; // Turn off vSync to control FPS manually
        }

        void FixedUpdate()
        {
            switch (_roundState)
            {
                case RoundStateType.Stop:

                    ChangeRoundState(RoundStateType.Intro);

                    break;
                case RoundStateType.Intro:

                    UpdateIntroState();

                    timer -= Time.deltaTime;
                    if (timer <= 0f)
                    {
                        ChangeRoundState(RoundStateType.Fight);
                    }

                    break;
                case RoundStateType.Fight:

                    frameCount++;

                    UpdateFightState();

                    var deadFighter = _fighters.Find((f) => f.isDead);
                    if (deadFighter != null)
                    {
                        ChangeRoundState(RoundStateType.KO);
                    }

                    break;
                case RoundStateType.KO:

                    UpdateKOState();
                    timer -= Time.deltaTime;
                    if (timer <= 0f)
                    {
                        ChangeRoundState(RoundStateType.End);
                    }

                    break;
                case RoundStateType.End:

                    UpdateEndState();
                    timer -= Time.deltaTime;
                    if (timer <= 0f
                        /*|| (timer <= endStateSkippableTime && IsKOSkipButtonPressed())*/)
                    {
                        ChangeRoundState(RoundStateType.Stop);
                    }

                    break;
            }
        }

        void ChangeRoundState(RoundStateType state)
        {
            _roundState = state;
            switch (_roundState)
            {
                case RoundStateType.Stop:

                    if (fighter1RoundWon >= maxRoundWon
                        || fighter2RoundWon >= maxRoundWon)
                    {
                        //GameManager.Instance.LoadTitleScene();
                    }

                    break;
                case RoundStateType.Intro:

                    fighter1.SetupBattleStart(fighterDataList[0], new Vector2(-2f, 0f), true);
                    fighter2.SetupBattleStart(fighterDataList[0], new Vector2(2f, 0f), false);

                    timer = introStateTime;

                    gameplaySceneUIManager.ShowMessageForState(_roundState, roundNumber);

                    //roundUIAnimator.SetTrigger("RoundStart");

                    //if (GameManager.Instance.isVsCPU)
                    //    battleAI = new BattleAI(this);

                    break;
                case RoundStateType.Fight:

                    gameplaySceneUIManager.ShowMessageForState(_roundState, roundNumber);
                    roundStartTime = Time.fixedTime;
                    frameCount = -1;

                    //currentRecordingInputIndex = 0;

                    break;
                case RoundStateType.KO:

                    //timer = koStateTime;

                    roundNumber++;

                    //CopyLastRoundInput();

                    fighter1.ClearInput();
                    fighter2.ClearInput();

                    //battleAI = null;

                    //roundUIAnimator.SetTrigger("RoundEnd");

                    break;
                case RoundStateType.End:

                    timer = endStateTime;

                    var deadFighter = _fighters.FindAll((f) => f.isDead);
                    if (deadFighter.Count == 1)
                    {
                        if (deadFighter[0] == fighter1)
                        {
                            fighter2RoundWon++;
                            fighter2.RequestWinAction();
                        }
                        else if (deadFighter[0] == fighter2)
                        {
                            fighter1RoundWon++;
                            fighter1.RequestWinAction();
                        }
                    }

                    break;
            }
        }

        void UpdateIntroState()
        {
            //var p1Input = GetP1InputData();
            //var p2Input = GetP2InputData();
            //RecordInput(p1Input, p2Input);
            //fighter1.UpdateInput(p1Input);
            //fighter2.UpdateInput(p2Input);

            _fighters.ForEach((f) => f.IncrementActionFrame());

            _fighters.ForEach((f) => f.UpdateIntroAction());
            _fighters.ForEach((f) => f.UpdateMovement());
            _fighters.ForEach((f) => f.UpdateBoxes());

            UpdatePushCharacterVsCharacter();
            UpdatePushCharacterVsBackground();
        }

        void UpdateFightState()
        {
            var p1Input = GetP1InputData();
            var p2Input = GetP2InputData();
            RecordInput(p1Input, p2Input);
            fighter1.UpdateInput(p1Input);
            fighter2.UpdateInput(p2Input);

            _fighters.ForEach((f) => f.IncrementActionFrame());

            _fighters.ForEach((f) => f.UpdateActionRequest());
            _fighters.ForEach((f) => f.UpdateMovement());
            _fighters.ForEach((f) => f.UpdateBoxes());

            UpdatePushCharacterVsCharacter();
            UpdatePushCharacterVsBackground();
            UpdateHitboxHurtboxCollision();
        }

        void UpdateKOState()
        {

        }

        void UpdateEndState()
        {
            _fighters.ForEach((f) => f.IncrementActionFrame());

            _fighters.ForEach((f) => f.UpdateActionRequest());
            _fighters.ForEach((f) => f.UpdateMovement());
            _fighters.ForEach((f) => f.UpdateBoxes());

            UpdatePushCharacterVsCharacter();
            UpdatePushCharacterVsBackground();
        }

        InputData GetP1InputData()
        {
            if (isReplayingLastRoundInput)
            {
                return lastRoundP1Input[currentReplayingInputIndex];
            }

            var time = Time.fixedTime - roundStartTime;

            InputData p1Input = new InputData();
            p1Input.input |= fighter1.InputManager.GetInput(InputDefine.Left) ? (int)InputDefine.Left : 0;
            p1Input.input |= fighter1.InputManager.GetInput(InputDefine.Right) ? (int)InputDefine.Right : 0;
            p1Input.input |= fighter1.InputManager.GetInput(InputDefine.Attack) ? (int)InputDefine.Attack : 0;
            p1Input.time = time;

            //Debug.Log(p1Input.input);

            //if (debugP1Attack)
            //    p1Input.input |= (int)InputDefine.Attack;
            //if (debugP1Guard)
            //    p1Input.input |= (int)InputDefine.Left;

            return p1Input;
        }

        InputData GetP2InputData()
        {


            if (isReplayingLastRoundInput)
            {
                return lastRoundP2Input[currentReplayingInputIndex];
            }

            var time = Time.fixedTime - roundStartTime;

            InputData p2Input = new InputData();
            
            //Hack
            return p2Input;

            //if (battleAI != null)
            //{
            //    p2Input.input |= battleAI.getNextAIInput();
            //}
            //else
            //{
            p2Input.input |= fighter2.InputManager.GetInput(InputDefine.Left) ? (int)InputDefine.Left : 0;
                p2Input.input |= fighter2.InputManager.GetInput(InputDefine.Right) ? (int)InputDefine.Right : 0;
                p2Input.input |= fighter2.InputManager.GetInput(InputDefine.Attack) ? (int)InputDefine.Attack : 0;
            //}

            p2Input.time = time;

            //if (debugP2Attack)
            //    p2Input.input |= (int)InputDefine.Attack;
            //if (debugP2Guard)
            //    p2Input.input |= (int)InputDefine.Right;

            return p2Input;
        }

        void RecordInput(InputData p1Input, InputData p2Input)
        {
            if (currentRecordingInputIndex >= maxRecordingInputFrame)
                return;

            recordingP1Input[currentRecordingInputIndex] = p1Input.Copy();
            recordingP2Input[currentRecordingInputIndex] = p2Input.Copy();
            currentRecordingInputIndex++;

            if (isReplayingLastRoundInput)
            {
                if (currentReplayingInputIndex < lastRoundMaxRecordingInput)
                    currentReplayingInputIndex++;
            }
        }

        void UpdatePushCharacterVsCharacter()
        {
            var rect1 = fighter1.pushbox.rect;
            var rect2 = fighter2.pushbox.rect;

            // Log world positions of the fighters
            Vector2 fighter1Pos = fighter1.transform.position;
            Vector2 fighter2Pos = fighter2.transform.position;

            bool isOverlapping = rect1.Overlaps(rect2);

            if (!isOverlapping)
            {
                // Calculate horizontal and vertical distances between rect edges
                float horizontalGap = Mathf.Max(rect2.xMin - rect1.xMax, rect1.xMin - rect2.xMax);
                float verticalGap = Mathf.Max(rect2.yMin - rect1.yMax, rect1.yMin - rect2.yMax);

                // Check if they are very close but not overlapping (possible floatingpoint precision issue)
                if (Mathf.Abs(horizontalGap) < 0.01f && Mathf.Abs(verticalGap) < 0.01f)
                {
                    Debug.LogWarning("Rects are extremely close but not overlapping Possible floating-point error.");
                }
            }
            else
            {
                // Your original overlap resolution logic here
                if (fighter1Pos.x < fighter2Pos.x)
                {
                    float pushAmount = (rect1.xMax - rect2.xMin) * -0.5f;
                    fighter1.ApplyPositionChange(pushAmount, 0);
                    fighter2.ApplyPositionChange(-pushAmount, 0);
                }
                else
                {
                    float pushAmount = (rect2.xMax - rect1.xMin) * 0.5f;
                    fighter1.ApplyPositionChange(pushAmount, 0);
                    fighter2.ApplyPositionChange(-pushAmount, 0);
                }
            }
        }

        void UpdatePushCharacterVsBackground()
        {
            var stageMinX = battleAreaWidth * -1 ;
            var stageMaxX = battleAreaWidth;

            float gravity = 9.8f;
            float groundLevel = -1.17f;

            _fighters.ForEach((f) =>
            {
                if (f.pushbox.xMin < stageMinX)
                {
                    f.ApplyPositionChange(stageMinX - f.pushbox.xMin, 0);
                }
                else if (f.pushbox.xMax > stageMaxX)
                {
                    f.ApplyPositionChange(stageMaxX - f.pushbox.xMax, 0);
                }

                // Handle gravity and ground collision
                if (f.transform.position.y > groundLevel)
                {
                    // Apply gravity - character is above ground, make them fall
                    float fallSpeed = gravity * Time.deltaTime;
                    f.transform.position = new Vector3(
                        f.transform.position.x,
                        f.transform.position.y - fallSpeed,
                        f.transform.position.z
                    );

                    // Ensure we don't fall through the ground
                    if (f.transform.position.y <= groundLevel)
                    {
                        f.transform.position = new Vector3(f.transform.position.x, groundLevel, 0);
                    }
                }
                else if (f.transform.position.y < groundLevel)
                {
                    // Character is below ground, push them up
                    f.transform.position = new Vector3(f.transform.position.x, groundLevel, 0);
                }
            });
        }

        void UpdateHitboxHurtboxCollision()
        {
            foreach (var attacker in _fighters)
            {
                Vector2 damagePos = Vector2.zero;
                bool isHit = false;
                bool isGrabbed = false;
                AttackID hitAttackID = 0;

                foreach (var damaged in _fighters)
                {
                    if (attacker == damaged)
                        continue;

                    foreach (var hitbox in attacker.hitboxes)
                    {
                        Debug.Log($"Checking Hurtbox : "+ damaged.hurtboxes.Count);
                        foreach (var hurtbox in damaged.hurtboxes)
                        {
                            if (hitbox.Overlaps(hurtbox))
                            {
                                isHit = true;
                                hitAttackID = hitbox.attackID;
                                float x1 = Mathf.Min(hitbox.xMax, hurtbox.xMax);
                                float x2 = Mathf.Max(hitbox.xMin, hurtbox.xMin);
                                float y1 = Mathf.Min(hitbox.yMax, hurtbox.yMax);
                                float y2 = Mathf.Max(hitbox.yMin, hurtbox.yMin);
                                damagePos.x = (x1 + x2) / 2;
                                damagePos.y = (y1 + y2) / 2;

                                Debug.Log($"Overlap detected! ");
                                break;

                            }
                            else 
                            {
                                Debug.Log($"No overlap. " +
                  $"Hitbox: [xMin={hitbox.xMin}, xMax={hitbox.xMax}, yMin={hitbox.yMin}, yMax={hitbox.yMax}] " +
                  $"Hurtbox: [xMin={hurtbox.xMin}, xMax={hurtbox.xMax}, yMin={hurtbox.yMin}, yMax={hurtbox.yMax}]");
                            }
                        }

                        if (isHit)
                            break;
                    }

                    foreach (var grabbox in attacker.grabboxes)
                    {

                        if (grabbox.Overlaps(damaged.pushbox))
                        {
                            isGrabbed = true;
                            hitAttackID = grabbox.attackID;
                            float x1 = Mathf.Min(grabbox.xMax, damaged.pushbox.xMax);
                            float x2 = Mathf.Max(grabbox.xMin, damaged.pushbox.xMin);
                            float y1 = Mathf.Min(grabbox.yMax, damaged.pushbox.yMax);
                            float y2 = Mathf.Max(grabbox.yMin, damaged.pushbox.yMin);
                            break;

                        }
                        else
                        {
                            Debug.Log("No overlap");
                        }

                        if (isGrabbed)
                            break;
                    }

                    if (isHit)
                    {
                        //attacker.NotifyAttackHit(damaged, damagePos);
                        //var damageResult = damaged.NotifyDamaged(attacker.getAttackData(hitAttackID), damagePos);

                        //var hitStunFrame = attacker.GetHitStunFrame(damageResult, hitAttackID);
                        //attacker.SetHitStun(hitStunFrame);
                        //damaged.SetHitStun(hitStunFrame);


                        //Debug.Log("It hurts");
                        //damaged.RequestAction(ActionID.Hurt);
                        damaged.HandleOnAttackHit();
                    }

                    if (isGrabbed)
                    {
                        damaged.HandleOnBeingGrabbed();
                    }
                }
            }
        }
    }
}


