using UnityEngine;

namespace FightingGameEngine
{
    public enum InputDefine
    {
        None = 0, //0000
        Left = 1 << 0, //0001
        Right = 1 << 1,//0010
        Down = 1 << 2,
        Up = 1 << 3,
        Attack = 1 << 4,
        Special = 1 << 5,
    }

    public class InputData
    {
        public int input;
        public float time;

        public InputData ShallowCopy()
        {
            return (InputData)this.MemberwiseClone();
        }
    }

}
