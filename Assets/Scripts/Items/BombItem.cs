using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombItem : MonoBehaviour
{
    [SerializeField] float maxRange = 12;
    [SerializeField] float expansionDelay = 0.1f;
    [SerializeField] float expansionDuration = 2f;
    [SerializeField] AnimationCurve smoothnes;

    [SerializeField] Transform deathTrigger;
    [SerializeField] SpriteRenderer spriteRenderer;

    bool activated = false;

    private void Awake() {
        deathTrigger.localScale = Vector3.one * maxRange;
        deathTrigger.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(activated || !collision.CompareTag("Player")) return;

        StartCoroutine(corrutine());
    }

    IEnumerator corrutine() {
        activated = true;

        deathTrigger.gameObject.SetActive(true);
        StartCoroutine(Utilidades.CreateExplosionVFX(transform.position, new Color(1f, 0.4f, 0f), maxRange / 2, expansionDuration));

        CameraMove.Instance.ShakeCamera(0.3f, 0.5f);

        yield return new WaitForSeconds(expansionDelay);
        StartCoroutine(Utilidades.CreateExplosionVFX(transform.position, Color.white, maxRange / 2, expansionDuration, 1));

        deathTrigger.gameObject.SetActive(false);
        spriteRenderer.enabled = false;


        yield return new WaitForSeconds(expansionDuration * 2);
        Destroy(gameObject);
    }
}
