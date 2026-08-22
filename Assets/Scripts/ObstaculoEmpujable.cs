using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ObstaculoEmpujable : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Configuración de Daño")]
    [Tooltip("El daño que le hará al jugador si lo golpea")]
    public float danioAlJugador = 5f;

    [Tooltip("Velocidad mínima a la que debe salir volando para hacer daño")]
    public float velocidadMinimaParaDanio = 3f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    
        if (collision.gameObject.CompareTag("Player"))
        {
        
            if (rb.velocity.magnitude >= velocidadMinimaParaDanio)
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();

                if (player != null && !player.isInvulnerable)
                {
                    player.TakeDamage(danioAlJugador);
                }
            }
        }
    }
}