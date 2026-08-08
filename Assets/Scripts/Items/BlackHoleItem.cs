using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleItem : MonoBehaviour
{
    [SerializeField] float range = 10;
    [SerializeField] float force = 10;
    [SerializeField] float duration = 5;
    bool activated = false;
    List<GameObject> enemies;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] ParticleSystemForceField forceField;
    [SerializeField] ParticleSystem particles;
    [SerializeField] GameObject dmgTrigger;

    private void Awake() {
        dmgTrigger.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(activated || !collision.CompareTag("Player")) return;

        StartCoroutine(corrutine());
    }

    IEnumerator corrutine() {
        activated = true;
        enemies = new();

        //Obtener los enemigos cercanos al jugador

        particles.Play();
        dmgTrigger.SetActive(true);

        CameraMove.Instance.ShakeCamera(0.3f, 0.1f);

        float t = 0;
        while(t < duration) {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

            for(int i = 0; i < hits.Length; i++) {
                if(!hits[i].CompareTag("Enemy")) continue;

                if(!enemies.Contains(hits[i].gameObject) && hits[i].TryGetComponent(out Enemy enemy)) {
                    enemies.Add(hits[i].gameObject);
                    enemy.enabled = false;
                }

                Vector2 _force = (transform.position - hits[i].transform.position).normalized * force;

                hits[i].attachedRigidbody.velocity = _force;
                Debug.DrawLine(hits[i].transform.position, _force);
            }

            t += Time.deltaTime;
            yield return null;
        }

        forceField.rotationAttraction = 0f;

        CameraMove.Instance.ShakeCamera(0.3f, 0.5f);

        dmgTrigger.SetActive(false);
        particles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        yield return new WaitForSeconds(1);

        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(4);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
