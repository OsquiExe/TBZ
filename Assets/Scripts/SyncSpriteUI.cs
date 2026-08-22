using UnityEngine;
using UnityEngine.UI;

public class SyncSpriteUI : MonoBehaviour
{
    private SpriteRenderer sr;
    private Image img;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        img = GetComponent<Image>();

        if (sr != null) sr.enabled = false;
    }

    void Update()
    {

        if (sr != null && img != null && sr.sprite != null)
        {
            if (img.sprite != sr.sprite)
            {
                img.sprite = sr.sprite;

                img.color = Color.white;
            }
        }
    }
}