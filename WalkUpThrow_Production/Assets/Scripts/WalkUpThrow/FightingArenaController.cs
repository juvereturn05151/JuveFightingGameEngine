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
        private SpriteRenderer fighter1Sprite;
        [SerializeField]
        private SpriteRenderer fighter2Sprite;

        [SerializeField]
        private float _battleBoxLineWidth = 2f;

        [SerializeField]
        private GUIStyle debugTextStyle;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        private void FixedUpdate()
        {
            //if (Input.GetKeyDown(KeyCode.F12))
            //{
            //    drawDebug = !drawDebug;
            //}

            //CalculateBattleArea();
            //CalculateFightPointToScreenScale();
        }

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
