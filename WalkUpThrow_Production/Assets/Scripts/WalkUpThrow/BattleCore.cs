using FightingGameEngine;
using System.Collections.Generic;
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

        private uint maxRoundWon = 3;

        //public Fighter fighter1 { get; private set; }
        //public Fighter fighter2 { get; private set; }

        public uint fighter1RoundWon { get; private set; }
        public uint fighter2RoundWon { get; private set; }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}


