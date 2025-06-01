using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndingSequence : MonoBehaviour
{
    public static EndingSequence Instance;

    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private CanvasGroup fakeDeathGroup;
    [SerializeField] private CanvasGroup realDeathGroup;

    [SerializeField] private Image fadeImage;

    [SerializeField] private float fadeTime;
    [SerializeField] private float musicTime;

    private Coroutine fadeCoroutine;

    private bool isFake = true;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        fadeGroup.alpha = 0f;
        fakeDeathGroup.alpha = 0f;
        realDeathGroup.alpha = 0f;
    }

    public void StartEdgingDeath()
    {
        StartCoroutine(StartEdgingDeathCoroutine());
    }

    public void EndEdgingDeath()
    {
        StartCoroutine(EndEdgingDeathCoroutine());
    }

    public void StartEndingSequence()
    {
        StartCoroutine(EndingSequenceBehaviour());
    }

    //Call first for fake death, then call again for real death.
    private IEnumerator EndingSequenceBehaviour()
    {
        fadeGroup.alpha = 0f;
        fakeDeathGroup.alpha = 0f;
        realDeathGroup.alpha = 0f;

        if(isFake)
        {
            fadeGroup.blocksRaycasts = true;

            for(float x = 0f; x <= 1; x += Time.deltaTime / fadeTime)
            {
                fadeGroup.alpha = x;

                yield return null;
            }

            fadeGroup.alpha = 1f;

            //angelic music

            yield return new WaitForSeconds(5f);

            for(float y = 0f; y <= 1; y += Time.deltaTime / 1)
            {
                fakeDeathGroup.alpha = y;

                yield return null;
            }

            fakeDeathGroup.alpha = 1f;

            //defibrillator sound

            yield return new WaitForSeconds(5f);

            fadeGroup.blocksRaycasts = false;

            fakeDeathGroup.alpha = 0f;
            fadeGroup.alpha = 0f;

            isFake = false;
        }
        else
        {
            fadeGroup.blocksRaycasts = true;

            for(float x = 0f; x <= 1; x += Time.deltaTime / fadeTime)
            {
                fadeGroup.alpha = x;

                yield return null;
            }

            fadeGroup.alpha = 1f;

            //angelic music

            yield return new WaitForSeconds(5f);

            for(float y = 0f; y <= 1; y += Time.deltaTime / 1)
            {
                realDeathGroup.alpha = y;

                yield return null;
            }

            realDeathGroup.alpha = 1f;
        }
    }

    private IEnumerator StartEdgingDeathCoroutine()
    {
        if(fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeCanvasGroup(fadeGroup, fadeGroup.alpha, 1f, fadeTime));

        yield return fadeCoroutine;
    }

    private IEnumerator EndEdgingDeathCoroutine()
    {
        if(fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeCanvasGroup(fadeGroup, fadeGroup.alpha, 0f, fadeTime));

        yield return fadeCoroutine;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            group.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            yield return null;
        }

        group.alpha = endAlpha;
    }
}
