using System.Collections.Generic;
using UnityEngine;

namespace FightinGameEngine 
{
    public class HitBoxManager : MonoBehaviour
    {
        public List<HitBox> activeHitboxes = new();

        public void AddHitbox(Rect rect, HitboxType type)
        {
            GameObject hbObj = new GameObject("Hitbox");
            hbObj.transform.parent = this.transform;

            var hb = hbObj.AddComponent<HitBox>();
            hb.Setup(rect, type);

            activeHitboxes.Add(hb);
        }

        public void ClearHitboxes()
        {
            foreach (var hb in activeHitboxes)
                Destroy(hb.gameObject);
            activeHitboxes.Clear();
        }
    }

}
