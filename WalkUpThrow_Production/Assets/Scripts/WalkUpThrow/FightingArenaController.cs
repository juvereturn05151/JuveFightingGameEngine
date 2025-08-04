using FightingGameEngine;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace WalkUpThrow 
{
    public class FightingArenaController : MonoBehaviour
    {
        [SerializeField]
        private BattleCore battleCore;

        [SerializeField]
        private float _battleBoxLineWidth = 2f;

        [SerializeField]
        private GUIStyle debugTextStyle;

        void OnGUI()
        {
            battleCore.fighters.ForEach((f) => DrawFighter(f));
        }

        void DrawFighter(Fighter fighter)
        {
            var labelRect = new Rect(0, Screen.height * 0.86f, Screen.width * 0.22f, 50);

            labelRect.y += Screen.height * 0.03f;
            GUI.Label(labelRect, "Frame: " + fighter.currentActionFrame + "/" + fighter.currentActionFrameCount, debugTextStyle);

            labelRect.y += Screen.height * 0.03f;
            GUI.Label(labelRect, "Stun: " + fighter.currentHitStunFrame, debugTextStyle);

            labelRect.y += Screen.height * 0.03f;
            GUI.Label(labelRect, "Action: " + fighter.currentActionID, debugTextStyle);
        }
    }

}
