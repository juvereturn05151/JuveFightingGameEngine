using UnityEngine;

namespace FightingGameEngine 
{
    [CreateAssetMenu]
    public class AnimationFrameDataSet : ScriptableObject
    {
        public int totalFrame;
        public AnimationFrame[] animationDataList;
    }

    [System.Serializable]
    public class AnimationFrame
    {
        public Vector2Int startEndFrame;
        public Sprite sprite;
        public AudioClip audioClip;
    }
}