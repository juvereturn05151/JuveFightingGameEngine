using UnityEngine;

namespace FightingGameEngine 
{
    public class BoxBase
    {
        public Rect rect;

        public float xMin { get { return rect.x - rect.width / 2; } }
        public float xMax { get { return rect.x + rect.width / 2; } }
        public float yMin { get { return rect.y; } }
        public float yMax { get { return rect.y + rect.height; } }

        public bool Overlaps(BoxBase otherBox)
        {
            var c1 = otherBox.xMax >= xMin;
            var c2 = otherBox.xMin <= xMax;
            var c3 = otherBox.yMax >= yMin;
            var c4 = otherBox.yMin <= yMax;

            return c1 && c2 && c3 && c4;
        }
    }

    public class Hitbox : BoxBase
    {
        public int attackID;
    }

    public class Hurtbox : BoxBase
    {
    }

    public class Pushbox : BoxBase
    {
    }
}
