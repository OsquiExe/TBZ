using UnityEngine;

public class DamageIncreaseEffect : MonoBehaviour
{
    public int level = 1;
    private float vfxTimer = 0f;

    [HideInInspector] public Sprite[] strongSprites; 

    private void Update()
    {
        vfxTimer += Time.deltaTime;


        if (vfxTimer >= 15f)
        {
            vfxTimer = 0f;

            if (strongSprites != null && strongSprites.Length > 0)
            {
                GameObject vfx = new GameObject("StrongVFX");
             
                Vector3 offsetArriba = new Vector3(0, 1.2f, 0);
                vfx.transform.position = transform.position + offsetArriba;

                vfx.AddComponent<SpriteRenderer>().sortingOrder = 50;
                vfx.AddComponent<VFXPlayer>().PlayAndDestroy(strongSprites, 0.08f, transform, offsetArriba);
            }
        }
    }

    public float GetExtraDamage()
    {
        return 2f * level;
    }
}