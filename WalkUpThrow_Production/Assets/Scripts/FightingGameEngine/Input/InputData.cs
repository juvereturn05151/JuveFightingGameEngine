using UnityEngine;

namespace FightingGameEngine
{
    public enum InputDefine
    {
        None = 0, //0000
        Left = 1 << 0, //0001
        Right = 1 << 1,//0010
        Attack = 1 << 2,    //0100
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
