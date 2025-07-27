using UnityEngine;

namespace FightinGameEngine 
{
    public class HurtBox : CollisionComponent
    {
        public void Setup(Rect rect)
        {
            this.localBounds = rect;
        }

        public bool Overlaps(Rect other)
        {
            return localBounds.Overlaps(other);
        }

        override protected void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            base.OnDrawGizmos();
        }
    }

}
