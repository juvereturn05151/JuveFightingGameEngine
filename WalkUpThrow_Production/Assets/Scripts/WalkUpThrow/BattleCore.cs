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

        public uint fighter1RoundWon { get; private set; }
        public uint fighter2RoundWon { get; private set; }

        public List<Fighter> fighters { get { return _fighters; } }
        private List<Fighter> _fighters = new List<Fighter>();

        private float roundStartTime;
        private int frameCount;

        public RoundStateType roundState { get { return _roundState; } }
        private RoundStateType _roundState = RoundStateType.Stop;

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

                    //if (debugPlayLastRoundInput
                    //    && !isReplayingLastRoundInput)
                    //{
                    //    StartPlayLastRoundInput();
                    //}

                    break;
                case RoundStateType.Fight:

                    //if (CheckUpdateDebugPause())
                    //{
                    //    break;
                    //}

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

                    //roundUIAnimator.SetTrigger("RoundStart");

                    //if (GameManager.Instance.isVsCPU)
                    //    battleAI = new BattleAI(this);

                    break;
                case RoundStateType.Fight:

                    roundStartTime = Time.fixedTime;
                    frameCount = -1;

                    //currentRecordingInputIndex = 0;

                    break;
                case RoundStateType.KO:

                    //timer = koStateTime;

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
            //var p1Input = GetP1InputData();
            //var p2Input = GetP2InputData();
            //RecordInput(p1Input, p2Input);
            //fighter1.UpdateInput(p1Input);
            //fighter2.UpdateInput(p2Input);

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
            //_fighters.ForEach((f) => f.UpdateMovement());
            _fighters.ForEach((f) => f.UpdateBoxes());

            UpdatePushCharacterVsCharacter();
            UpdatePushCharacterVsBackground();
        }

        void UpdatePushCharacterVsCharacter()
        {
            //var rect1 = fighter1.pushbox.rect;
            //var rect2 = fighter2.pushbox.rect;

            //if (rect1.Overlaps(rect2))
            //{
            //    if (fighter1.position.x < fighter2.position.x)
            //    {
            //        fighter1.ApplyPositionChange((rect1.xMax - rect2.xMin) * -1 / 2, fighter1.position.y);
            //        fighter2.ApplyPositionChange((rect1.xMax - rect2.xMin) * 1 / 2, fighter2.position.y);
            //    }
            //    else if (fighter1.position.x > fighter2.position.x)
            //    {
            //        fighter1.ApplyPositionChange((rect2.xMax - rect1.xMin) * 1 / 2, fighter1.position.y);
            //        fighter2.ApplyPositionChange((rect2.xMax - rect1.xMin) * -1 / 2, fighter1.position.y);
            //    }
            //}
        }

        void UpdatePushCharacterVsBackground()
        {
            //var stageMinX = battleAreaWidth * -1 / 2;
            //var stageMaxX = battleAreaWidth / 2;

            //_fighters.ForEach((f) =>
            //{
            //    if (f.pushbox.xMin < stageMinX)
            //    {
            //        f.ApplyPositionChange(stageMinX - f.pushbox.xMin, f.position.y);
            //    }
            //    else if (f.pushbox.xMax > stageMaxX)
            //    {
            //        f.ApplyPositionChange(stageMaxX - f.pushbox.xMax, f.position.y);
            //    }
            //});
        }

        void UpdateHitboxHurtboxCollision()
        {
            foreach (var attacker in _fighters)
            {
                Vector2 damagePos = Vector2.zero;
                bool isHit = false;
                //bool isProximity = false;
                AttackID hitAttackID = 0;

                foreach (var damaged in _fighters)
                {
                    if (attacker == damaged)
                        continue;

                    foreach (var hitbox in attacker.hitboxes)
                    {
                        // continue if attack already hit
                        //if (!attacker.CanAttackHit(hitbox.attackID))
                        //{
                        //    continue;
                        //}

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
                                break;

                            }
                        }

                        if (isHit)
                            break;
                    }

                    if (isHit)
                    {
                        //attacker.NotifyAttackHit(damaged, damagePos);
                        //var damageResult = damaged.NotifyDamaged(attacker.getAttackData(hitAttackID), damagePos);

                        //var hitStunFrame = attacker.GetHitStunFrame(damageResult, hitAttackID);
                        //attacker.SetHitStun(hitStunFrame);
                        //damaged.SetHitStun(hitStunFrame);
                        //damaged.SetSpriteShakeFrame(hitStunFrame / 3);

                        //damageHandler(damaged, damagePos, damageResult);
                    }
                }
            }
        }
    }
}


