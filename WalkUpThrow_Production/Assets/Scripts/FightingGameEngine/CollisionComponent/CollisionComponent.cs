using UnityEngine;

namespace FightinGameEngine
{
    public class CollisionComponent : MonoBehaviour
    {
        public Rect localBounds;

        protected virtual void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)localBounds.center, localBounds.size);
        }
    }

}

