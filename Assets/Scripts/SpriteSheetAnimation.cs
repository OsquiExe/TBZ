using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetAnimation : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] SpriteRenderer renderer;

    [SerializeField] float timeBetweenFrames = 0.1f;
    [SerializeField] float size = 0.2f;

    [SerializeField] bool autoPlay = false;
    [SerializeField] bool loop = true;

    bool playing;
    bool justEnabled = false;

    private void OnEnable() {
        playing = false;

        if(autoPlay) Play();
    }

    private void Update() {
        if(!playing && autoPlay && loop) Play();
    }

    public void Play() {
        if(playing) return;

        StartCoroutine(corrutine());

        IEnumerator corrutine() {
            playing = true;

            Transform t = transform.parent;
            transform.parent = null;

            yield return null;
            transform.localScale = Vector3.one * size;
            transform.parent = t;

            for(int i = 0; i < sprites.Length; i++) { 
                renderer.sprite = sprites[i];

                yield return new WaitForSeconds(timeBetweenFrames);
            }

            playing = false;
        }
    }
}
