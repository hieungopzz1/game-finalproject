using System.Collections;
using UnityEngine;
using TMPro;   // nếu dùng TextMeshPro

public class WaveUIController : MonoBehaviour
{
    public static WaveUIController instance;

    public TextMeshProUGUI waveText;  // kéo TextMeshProUGUI vào
    public CanvasGroup canvasGroup;   // kéo CanvasGroup vào

    [Header("Hiệu ứng")]
    public float fadeInTime = 0.3f;
    public float stayTime = 1.0f;
    public float fadeOutTime = 0.7f;

    private Coroutine currentRoutine;

    void Reset()
    {
        // auto tìm component khi add script
        waveText = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (waveText == null)
            waveText = GetComponent<TextMeshProUGUI>();
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    public void ShowWave(string text)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowWaveRoutine(text));
    }

    private IEnumerator ShowWaveRoutine(string text)
    {
        if (waveText == null || canvasGroup == null)
            yield break;

        waveText.text = text;

        // Fade in
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInTime);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        // Giữ nguyên 1 lúc
        yield return new WaitForSeconds(stayTime);

        // Fade out
        t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutTime);
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
