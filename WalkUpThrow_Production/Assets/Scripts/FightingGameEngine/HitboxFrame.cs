/*
 Author: Ju-ve Chankasemporn
 E-Mail: juvereturn@gmail.com
 Date: 2023-07-27
 */

using System.Collections.Generic;
using UnityEngine;

namespace FightinGameEngine 
{
    [System.Serializable]
    public class HitboxData
    {
        public Rect box;
        public HitboxType type; // Hit, Hurt, Throw, etc.
    }

    [System.Serializable]
    public class HitboxFrame
    {
        public int frame;
        public List<HitboxData> hitboxes = new List<HitboxData>();
    }
}