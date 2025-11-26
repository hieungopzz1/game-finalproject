using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshPro textMesh;

    public float riseSpeed = 1.5f;   // tốc độ bay lên
    public float lifetime = 0.5f;    // thời gian sống trước khi fade
    public float fadeTime = 0.3f;    // thời gian mờ dần

    private float timer;
    private Color startColor;

    void Awake()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

        startColor = textMesh.color;

        // ⭐ SET SORTING LAYER CHO SỐ DAMAGE
        var renderer = textMesh.GetComponent<MeshRenderer>();
        renderer.sortingLayerName = "UI";     // đổi thành layer bạn muốn
        renderer.sortingOrder = 500;          // càng cao càng nằm trên
    }


    public void Setup(float damage)
    {
        textMesh.text = Mathf.RoundToInt(damage).ToString();

        // random nhẹ vị trí cho vui
        transform.position += new Vector3(
            Random.Range(-0.2f, 0.2f),
            Random.Range(0.2f, 0.4f),
            0f
        );
    }

    void Update()
    {
        timer += Time.deltaTime;

        // bay lên
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // bắt đầu fade khi gần hết lifetime
        if (timer > lifetime)
        {
            float t = (timer - lifetime) / fadeTime;
            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            textMesh.color = c;

            if (t >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
