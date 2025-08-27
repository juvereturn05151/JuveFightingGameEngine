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

        public Dictionary<ActionID, ActionData> actions { get { return _actions; } }
        private Dictionary<ActionID, ActionData> _actions = new Dictionary<ActionID, ActionData>();

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
        }
    }
}