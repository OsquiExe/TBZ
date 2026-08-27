using System.Collections;
using UnityEngine;

public class VFXPlayer : MonoBehaviour
{
    public void PlayAndDestroy(Sprite[] frames, float frameRate, Transform followTarget = null, Vector3 offset = default)
    {
        StartCoroutine(Routine(frames, frameRate, followTarget, offset));
    }

    private IEnumerator Routine(Sprite[] frames, float frameRate, Transform followTarget, Vector3 offset)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        foreach (Sprite frame in frames)
        {
            sr.sprite = frame;

          
            if (followTarget != null)
            {
                transform.position = followTarget.position + offset;
            }

            yield return new WaitForSeconds(frameRate);
        }

        Destroy(gameObject);
    }
}