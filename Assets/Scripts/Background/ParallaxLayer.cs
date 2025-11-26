using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    float backgroundWidth;

    private void Start()
    {
        Sprite sp = GetComponent<SpriteRenderer>().sprite;
        backgroundWidth = sp.texture.width / sp.pixelsPerUnit;
    }
    private void Update()
    {
        float moveX = speed * Time.deltaTime;
        transform.position += new Vector3(moveX,0);
        if(Mathf.Abs(transform.position.x) >= backgroundWidth)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }
    }
}
