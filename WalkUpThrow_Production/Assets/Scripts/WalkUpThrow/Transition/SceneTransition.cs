using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionPP : MonoBehaviour
{
    public static SceneTransitionPP Instance;

    public Volume volume;              // Reference to your Global Volume
    private ColorAdjustments colorAdj; // The effect we’ll animate
    public float transitionDuration = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (volume.profile.TryGet(out colorAdj))
        {
            // Start scene dark (exposure -100) then fade to 0
            colorAdj.contrast.Override(-100f);
            StartCoroutine(FadeTo(0f));
        }
        else
        {
            Debug.LogError("No Color Adjustments in Volume profile!");
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        // Fade to dark
        yield return FadeTo(-100f);

        // Load next scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // Ensure reference is updated after scene load
        if (!volume.profile.TryGet(out colorAdj))
            volume.profile.TryGet(out colorAdj);

        // Fade back to normal
        yield return FadeTo(0f);
    }

    private IEnumerator FadeTo(float targetExposure)
    {
        float start = colorAdj.contrast.value;
        float t = 0f;

        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            colorAdj.contrast.Override(Mathf.Lerp(start, targetExposure, t / transitionDuration));
            yield return null;
        }

        colorAdj.contrast.Override(targetExposure);
    }
}
