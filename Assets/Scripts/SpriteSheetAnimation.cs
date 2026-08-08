using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetAnimation : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] SpriteRenderer renderer;

    [SerializeField] float timeBetweenFrames = 0.1f;
    [SerializeField] float size = 0.2f;

    public void Play() {

        StartCoroutine(corrutine());

        IEnumerator corrutine() {
            Transform t = transform.parent;
            transform.parent = null;

            yield return null;
            transform.localScale = Vector3.one * size;
            transform.parent = t;

            for(int i = 0; i < sprites.Length; i++) { 
                renderer.sprite = sprites[i];

                yield return new WaitForSeconds(timeBetweenFrames);
            }
        }
    }
}
