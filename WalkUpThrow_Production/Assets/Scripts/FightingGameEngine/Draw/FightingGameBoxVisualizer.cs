using FightingGameEngine;
using UnityEngine;
using UnityEngine.UIElements;
using WalkUpThrow;

namespace FightingGameEngine
{
    public class FightingGameBoxVisualizer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BattleCore _battleCore;

        [Header("Visual Settings")]
        [SerializeField] private Color _pushboxColor = new Color(0, 0.5f, 1f, 0.3f);
        [SerializeField] private Color _hurtboxColor = new Color(0, 1f, 0.3f, 0.3f);
        [SerializeField] private Color _hitboxColor = new Color(1f, 0.2f, 0.2f, 0.4f);
        [SerializeField] private float _outlineThickness = 1.5f;

        private Texture2D _whiteTexture;

        private void Initialize()
        {
            if (_whiteTexture != null) return;

            _whiteTexture = new Texture2D(1, 1);
            _whiteTexture.SetPixel(0, 0, Color.white);
            _whiteTexture.Apply();
        }

        private void OnGUI()
        {
            if (_battleCore == null || _battleCore.fighters == null) return;

            Initialize();
            GUI.depth = -9999; // Draw on top of everything

            foreach (var fighter in _battleCore.fighters)
            {
                if (fighter == null) continue;

                // Draw order matters! (back to front)
                DrawHurtboxes(fighter);
                DrawPushbox(fighter);
                DrawHitboxes(fighter);
            }
        }

        private void DrawPushbox(Fighter fighter)
        {
            if (fighter.pushbox == null) return;

            Rect rect = GetScreenRect(fighter, fighter.pushbox.rect);
            DrawBox(rect, _pushboxColor, "Pushbox");
        }

        private void DrawHurtboxes(Fighter fighter)
        {
            if (fighter.hurtboxes == null || fighter.hurtboxes.Count == 0) return;

            foreach (var hurtbox in fighter.hurtboxes)
            {
                Rect rect = GetScreenRect(fighter, hurtbox.rect);
                DrawBox(rect, _hurtboxColor, "Hurtbox");
            }
        }

        private void DrawHitboxes(Fighter fighter)
        {
            if (fighter.hitboxes == null || fighter.hitboxes.Count == 0) return;

            foreach (var hitbox in fighter.hitboxes)
            {
                Rect rect = GetScreenRect(fighter, hitbox.rect);
                DrawBox(rect, _hitboxColor, "Hitbox");
            }
        }

        private Rect GetScreenRect(Fighter fighter, Rect boxRect)
        {
            // 1. Get base world position of fighter
            Vector3 fighterWorldPos = fighter.transform.position;

            // 2. Calculate box center in local space
            Vector2 boxCenterLocal = new Vector2(
                boxRect.x + boxRect.width * 0.5f,
                boxRect.y + boxRect.height * 0.5f
            );

            // 3. Flip X if facing left
            //if (!fighter.isFaceRight)
            //{
            //    boxCenterLocal.x *= -1;
            //}

            // 4. Convert box center to world
            Vector3 boxCenterWorld = fighterWorldPos + new Vector3(boxCenterLocal.x, boxCenterLocal.y, 0f);

            // 5. Convert corners to screen points
            Vector3 boxMinWorld = boxCenterWorld - new Vector3(boxRect.width * 0.5f, boxRect.height * 0.5f, 0f);
            Vector3 boxMaxWorld = boxCenterWorld + new Vector3(boxRect.width * 0.5f, boxRect.height * 0.5f, 0f);

            Vector3 screenMin = Camera.main.WorldToScreenPoint(boxMinWorld);
            Vector3 screenMax = Camera.main.WorldToScreenPoint(boxMaxWorld);

            // Convert Y since screen space in GUI is bottom-up
            screenMin.y = Screen.height - screenMin.y;
            screenMax.y = Screen.height - screenMax.y;

            // Create rect from screen min/max
            float x = Mathf.Min(screenMin.x, screenMax.x);
            float y = Mathf.Min(screenMin.y, screenMax.y);
            float width = Mathf.Abs(screenMax.x - screenMin.x);
            float height = Mathf.Abs(screenMax.y - screenMin.y);

            Debug.Log($"Fighter: {fighter.name}, Box Rect: {boxRect}, Screen Rect: {x}, {y}, {width}, {height}");

            return new Rect(x, y, width, height);
        }


        private void DrawBox(Rect rect, Color color, string label = "")
        {
            if (rect.width <= 0 || rect.height <= 0) return;

            // Draw fill
            GUI.color = color;
            GUI.DrawTexture(rect, _whiteTexture);

            // Draw outline
            DrawBoxOutline(rect, new Color(color.r, color.g, color.b, 1f), _outlineThickness);

            // Draw label
            if (!string.IsNullOrEmpty(label))
            {
                GUI.color = Color.white;
                GUI.Label(new Rect(rect.x, rect.y - 18, rect.width, 20), label);
            }
        }

        private void DrawBoxOutline(Rect rect, Color color, float thickness)
        {
            // Top
            DrawLine(new Vector2(rect.x, rect.y),
                    new Vector2(rect.x + rect.width, rect.y),
                    color, thickness);
            // Right
            DrawLine(new Vector2(rect.x + rect.width, rect.y),
                    new Vector2(rect.x + rect.width, rect.y + rect.height),
                    color, thickness);
            // Bottom
            DrawLine(new Vector2(rect.x + rect.width, rect.y + rect.height),
                    new Vector2(rect.x, rect.y + rect.height),
                    color, thickness);
            // Left
            DrawLine(new Vector2(rect.x, rect.y + rect.height),
                    new Vector2(rect.x, rect.y),
                    color, thickness);
        }

        private void DrawLine(Vector2 start, Vector2 end, Color color, float thickness)
        {
            GUI.color = color;
            Vector2 delta = end - start;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

            GUIUtility.ScaleAroundPivot(new Vector2(delta.magnitude, thickness), Vector2.zero);
            GUIUtility.RotateAroundPivot(angle, Vector2.zero);
            GUI.matrix = Matrix4x4.TRS(start, Quaternion.identity, Vector3.one) * GUI.matrix;

            GUI.DrawTexture(new Rect(0, 0, 1, 1), _whiteTexture);
            GUI.matrix = Matrix4x4.identity;
        }
    }
}
