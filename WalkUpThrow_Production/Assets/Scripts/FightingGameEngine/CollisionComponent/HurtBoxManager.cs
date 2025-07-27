using System.Collections.Generic;
using UnityEngine;

namespace FightinGameEngine 
{
    public class HurtBoxManager : MonoBehaviour
    {
        public List<HurtBox> hurtboxes;

        public bool CheckHit(Rect incoming)
        {
            foreach (var hurtbox in hurtboxes)
            {
                if (hurtbox.Overlaps(incoming))
                    return true;
            }
            return false;
        }
    }

}