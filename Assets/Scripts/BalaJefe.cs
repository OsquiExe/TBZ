using UnityEngine;

public class BalaJefe : MonoBehaviour
{
    public float danoAlJugador = 10f; 

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {

                player.TakeDamage(danoAlJugador);
            }

            Destroy(gameObject);
        }
    }
}