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
    }
}

