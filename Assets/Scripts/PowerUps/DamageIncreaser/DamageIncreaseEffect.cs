using UnityEngine;

public class DamageIncreaseEffect : MonoBehaviour
{
    public int level = 1;


    public float GetExtraDamage()
    {
        return 2f * level; 
    }
}