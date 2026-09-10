using UnityEngine;
using UnityEngine.UI; // Agregamos esto para poder manejar imágenes del Canvas

public class LimitWarning : MonoBehaviour
{
    [Header("Configuración de UI")]
    [Tooltip("Arrastrá acá la Imagen del Canvas que va a titilar")]
    public Image warningImage;

    [Header("Ajustes del Efecto")]
    [Tooltip("Velocidad a la que titila el cartel")]
    public float fadeSpeed = 4f;

    [Tooltip("Distancia a la que te tenés que alejar para que desaparezca")]
    public float safeDistance = 3f;

    private bool isNearWall = false;
    private Transform playerTransform;
    private Collider2D wallCollider;

    private void Start()
    {
        wallCollider = GetComponent<Collider2D>();

        // Arrancamos con el cartel totalmente transparente (invisible)
        if (warningImage != null)
        {
            Color c = warningImage.color;
            c.a = 0f;
            warningImage.color = c;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Si el jugador choca la pared, activamos la alarma
        if (collision.gameObject.CompareTag("Player"))
        {
            playerTransform = collision.transform;
            isNearWall = true;
        }
    }

    private void Update()
    {
        if (warningImage == null) return;

        // 1. Lógica de Distancia: ¿El jugador se alejó de la pared?
        if (isNearWall && playerTransform != null)
        {
            // Buscamos el punto del EdgeCollider más cercano al jugador
            Vector2 closestPoint = wallCollider.ClosestPoint(playerTransform.position);

            // Medimos la distancia entre el jugador y ese punto
            float distance = Vector2.Distance(playerTransform.position, closestPoint);

            // Si se alejó superando la distancia segura, apagamos la alarma
            if (distance > safeDistance)
            {
                isNearWall = false;
            }
        }

        // 2. Lógica Visual (El Fade In/Out Suave de la UI)
        Color c = warningImage.color;

        if (isNearWall)
        {
            // Titila suavemente usando una onda matemática (Seno). 
            // Esto lo hace ir de transparente a opaco en un loop infinito sin cortar.
            c.a = (Mathf.Sin(Time.time * fadeSpeed) + 1f) / 2f;
        }
        else
        {
            // Si ya no está cerca de la pared, desaparece suavemente (Fade Out)
            c.a = Mathf.Lerp(c.a, 0f, Time.deltaTime * fadeSpeed * 1.5f);
        }

        warningImage.color = c;
    }
}