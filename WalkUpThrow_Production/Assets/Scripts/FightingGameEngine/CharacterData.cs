using System.Collections.Generic;
using UnityEngine;

namespace FightinGameEngine 
{
    [CreateAssetMenu(menuName = "FightingGame/Character")]
    public class CharacterData : ScriptableObject
    {
        public string characterName;
        public List<MoveData> moveList;
    }
}
