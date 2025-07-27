using UnityEngine;

namespace FightinGameEngine 
{
    public class HitBox : CollisionComponent
    {
        public HitboxType type;

        public void Setup(Rect rect, HitboxType type)
        {
            this.localBounds = rect;
            this.type = type;
        }

        override protected void OnDrawGizmos()
        {
            Gizmos.color = type == HitboxType.Attack ? Color.red : Color.blue;
            base.OnDrawGizmos();
        }
    }
}