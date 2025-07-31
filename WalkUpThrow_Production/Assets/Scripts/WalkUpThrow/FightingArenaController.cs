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
    }

}
