using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WalkUpThrow
{
    public class GameplaySceneUIManager : MonoBehaviour
    {
        public event Action<BattleCore.RoundStateType> OnMessageComplete;

        [Header("UI References")]
        [SerializeField] private CanvasGroup messageCanvasGroup; // fade in/out
        [SerializeField] private TMP_Text messageText;           // message text (Round, Fight, KO, etc.)
        [SerializeField] private Animator bannerAnimator;        // optional animation for text/banner

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float messageDuration = 0.25f;

        [Header("Special Case Durations")]
        [SerializeField] private float fightMessageDuration = 0.1f; // faster hide for "Fight!"

        private Coroutine currentRoutine;

        public void ShowMessageForState(BattleCore.RoundStateType state, int roundNumber = 1)
        {
            string message = "";
            float durationOverride = messageDuration; // default

            switch (state)
            {
                case BattleCore.RoundStateType.Intro:
                    message = $"Round {roundNumber}";
                    break;
                case BattleCore.RoundStateType.Fight:
                    message = "Fight !";
                    durationOverride = fightMessageDuration; // special case
                    break;
                case BattleCore.RoundStateType.KO:
                    message = "KO!";
                    break;
            }

            if (!string.IsNullOrEmpty(message))
            {
                if (currentRoutine != null) StopCoroutine(currentRoutine);
                currentRoutine = StartCoroutine(ShowMessageRoutine(state, message, durationOverride));
            }
        }

        private IEnumerator ShowMessageRoutine(BattleCore.RoundStateType state, string message, float duration)
        {
            messageText.text = message;

            // Reset alpha
            messageCanvasGroup.alpha = 0f;
            messageCanvasGroup.gameObject.SetActive(true);

            // Fade in
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                messageCanvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
                yield return null;
            }

            // Play animator if assigned
            if (bannerAnimator != null)
            {
                bannerAnimator.SetTrigger("Play");
            }

            yield return new WaitForSeconds(duration);

            // Fade out
            t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                messageCanvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
                yield return null;
            }

            messageCanvasGroup.gameObject.SetActive(false);
            currentRoutine = null;

            // IMPORTANT: notify listeners that this message finished
            OnMessageComplete?.Invoke(state);
        }
    }
}
