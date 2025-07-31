using System.Collections.Generic;
using UnityEngine;

namespace FightingGameEngine 
{
    [CreateAssetMenu]
    public class FighterData : ScriptableObject 
    {
        public float forwardMoveSpeed = 2.2f;
        public float backwardMoveSpeed = 1.8f;

        [SerializeField]
        public Rect baseHurtBoxRect;

        [SerializeField]
        public Rect basePushBoxRect;

        [SerializeField]
        private ActionDataContainer actionDataContainer;

        //[SerializeField]
        //private AttackDataContainer attackDataContainer;

        [SerializeField]
        private AnimationDataContainer animationDataContainer;

        public Dictionary<ActionID, ActionData> actions { get { return _actions; } }
        private Dictionary<ActionID, ActionData> _actions = new Dictionary<ActionID, ActionData>();

        //public Dictionary<int, AttackData> attackData { get { return _attackData; } }
        //private Dictionary<int, AttackData> _attackData = new Dictionary<int, AttackData>();

        public Dictionary<AnimationID, AnimationData> animationData { get { return _animationData; } }
        private Dictionary<AnimationID, AnimationData> _animationData = new Dictionary<AnimationID, AnimationData>();

        public void SetupDictionary()
        {
            if (actionDataContainer == null)
            {
                Debug.LogError("ActionDataContainer is not set");
                return;
            }

            _actions = new Dictionary<ActionID, ActionData>();
            foreach (var action in actionDataContainer.actions)
            {
                _actions.Add(action.actionID, action);
            }

            //_attackData = new Dictionary<int, AttackData>();
            //foreach (var data in attackDataContainer.attackDataList)
            //{
            //    _attackData.Add(data.attackID, data);
            //}

            _animationData = new Dictionary<AnimationID, AnimationData>();
            foreach (var data in animationDataContainer.animationDataList)
            {
                _animationData.Add(data.animationID, data);
            }
        }
    }
}