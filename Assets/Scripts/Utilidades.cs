using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilidades
{
    public static Vector2 Rotate(this Vector2 v, float degrees) {
        return new Vector2(
            (float)(v.x * Mathf.Cos(degrees) - v.y * Mathf.Sin(degrees)),
            (float)(v.x * Mathf.Sin(degrees) + v.y * Mathf.Cos(degrees))
        );
    }

    public static IEnumerator CreateExplosionVFX(Vector2 pos, Color color, float explosionRadius = 8, float _duration = 0.3f, float circleWidth = 0.8f) {
        GameObject explosionObj = new GameObject("ExplosionVFX");
        explosionObj.transform.position = pos;

        LineRenderer line = explosionObj.AddComponent<LineRenderer>();
        line.positionCount = 30;
        line.loop = true;
        line.useWorldSpace = false;
        line.startWidth = circleWidth;
        line.endWidth = circleWidth;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.sortingOrder = 50;

        float duration = _duration;
        float elapsed = 0f;
        Color orange = color;

        while(elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float easeOut = 1f - Mathf.Pow(1f - t, 3f);
            float currentRadius = Mathf.Lerp(0f, explosionRadius, easeOut);

            for(int i = 0; i < 30; i++) {
                float angle = (i / 30f) * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * currentRadius;
                float y = Mathf.Sin(angle) * currentRadius;
                line.SetPosition(i, new Vector3(x, y, 0));
            }

            float alpha = Mathf.Lerp(1f, 0f, t);
            line.startColor = new Color(orange.r, orange.g, orange.b, alpha);
            line.endColor = new Color(orange.r, orange.g, orange.b, alpha);

            yield return null;
        }

        MonoBehaviour.Destroy(explosionObj);
    }
}
