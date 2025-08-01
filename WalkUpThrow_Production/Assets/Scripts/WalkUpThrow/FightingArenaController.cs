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

        private Vector2 battleAreaTopLeftPoint;
        private Vector2 battleAreaBottomRightPoint;

        private Vector2 fightPointToScreenScale;
        private float centerPoint;
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

            UpdateSprite();
        }

        void OnGUI()
        {
            battleCore.fighters.ForEach((f) => DrawFighter(f));
        }

        void UpdateSprite()
        {
            if (fighter1Sprite != null)
            {
                var sprite = battleCore.fighter1.GetCurrentAnimationSprite();
                if (sprite != null)
                    fighter1Sprite.sprite = sprite;

                //var position = fighter1Image.transform.position;
                //position.x = TransformHorizontalFightPointToScreen(battleCore.fighter1.position.x);
                //fighter1Image.transform.position = position;
            }

            if (fighter2Sprite != null)
            {
                var sprite = battleCore.fighter2.GetCurrentAnimationSprite();
                if (sprite != null)
                    fighter2Sprite.sprite = sprite;

                //var position = fighter2Image.transform.position;
                //position.x = TransformHorizontalFightPointToScreen(battleCore.fighter2.position.x);
                //fighter2Image.transform.position = position;

            }
        }

        void DrawFighter(Fighter fighter)
        {
            var labelRect = new Rect(0, Screen.height * 0.86f, Screen.width * 0.22f, 50);

            labelRect.y += Screen.height * 0.03f;
            //var frameAdvantage = battleCore.GetFrameAdvantage(fighter.isFaceRight);
            //var frameAdvText = frameAdvantage > 0 ? "+" + frameAdvantage : frameAdvantage.ToString();
            GUI.Label(labelRect, "Frame: " + fighter.currentActionFrame + "/" + fighter.currentActionFrameCount
               /* + "(" + frameAdvText + ")"*/, debugTextStyle);

            labelRect.y += Screen.height * 0.03f;
            GUI.Label(labelRect, "Stun: " + fighter.currentHitStunFrame, debugTextStyle);

            labelRect.y += Screen.height * 0.03f;
            GUI.Label(labelRect, "Action: " + fighter.currentActionID, debugTextStyle);
        }


        
    }

}
