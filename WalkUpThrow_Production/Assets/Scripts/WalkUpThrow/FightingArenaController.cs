using FightingGameEngine;
using UnityEngine;

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

        float xPos = 0f;

        void OnGUI()
        {
            xPos = Screen.width * 0.2f;
            DrawFighter(battleCore.fighters[0]);
            xPos = Screen.width * 0.8f;
            DrawFighter(battleCore.fighters[1]);
        }

        void DrawFighter(Fighter fighter)
        {
            var labelRect = new Rect(xPos, Screen.height * 0.86f, Screen.width * 0.22f, 50);

            labelRect.y += Screen.height * 0.03f;
            GUI.Label(labelRect, "Frame: " + fighter.currentActionFrame + "/" + fighter.currentActionFrameCount, debugTextStyle);

            labelRect.y += Screen.height * 0.03f;
            GUI.Label(labelRect, "Stun: " + fighter.currentHitStunFrame, debugTextStyle);

            labelRect.y += Screen.height * 0.03f;
            GUI.Label(labelRect, "Action: " + fighter.currentActionID, debugTextStyle);
        }
    }

}
