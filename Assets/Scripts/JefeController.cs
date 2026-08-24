using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JefeController : MonoBehaviour
{
    [Header("Estadísticas")]
    public float vidaMaxima = 500f;
    private float vidaActual;
    public float velocidadCaminar = 2f;

    [Header("Daño de Contacto")]
    public float danoPorContacto = 15f;

    [Header("Ajustes de Embestida")]
    public float velocidadEmbestida = 20f;
    public float duracionEmbestida = 0.8f;

    [Header("Referencias")]
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public GameObject balaPrefab;
    public LineRenderer lineaEmbestida;
    public Animator animator;
    private Slider barraVida;

    [Header("Feedback Visual de Daño")]
    [SerializeField] private float flashDuration = 0.1f;
    private SpriteRenderer[] allRenderers;
    private Material[] originalMaterials;
    private Material whiteFlashMaterial;

    [Header("Ataques (Cooldowns)")]
    public float cooldownAtaquesBasicos = 3f;
    private float temporizadorAtaqueBasico;
    private float cooldownAtaqueCostados = 25f;
    private float temporizadorCostados;

    private enum EstadoJefe { Caminando, PreparandoAtaque, Atacando, Muerto }
    private EstadoJefe estadoActual;

    void Start()
    {
        vidaActual = vidaMaxima;

#if UNITY_EDITOR
        vidaActual = 1;
#endif
        rb = GetComponent<Rigidbody2D>();

 
        spriteRenderer = GetComponent<SpriteRenderer>();


        allRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalMaterials = new Material[allRenderers.Length];

        for (int i = 0; i < allRenderers.Length; i++)
        {
            originalMaterials[i] = allRenderers[i].material;
        }

        whiteFlashMaterial = new Material(Shader.Find("GUI/Text Shader"));
        whiteFlashMaterial.color = Color.white;
   

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        if (lineaEmbestida != null) lineaEmbestida.enabled = false;

        estadoActual = EstadoJefe.Caminando;
        temporizadorCostados = cooldownAtaqueCostados;


        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            Transform barraTransform = canvas.transform.Find("HealthBarBoss");

            if (barraTransform != null)
            {
                barraVida = barraTransform.GetComponent<Slider>();
                if (barraVida != null)
                {
                    barraVida.maxValue = vidaMaxima;
                    barraVida.value = vidaActual;
                    barraVida.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogWarning("No se encontró 'HealthBarBoss' dentro del Canvas. Fijate que el nombre sea exacto.");
            }
        }
    }

    void Update()
    {
        if (estadoActual == EstadoJefe.Muerto || player == null) return;

        ManejarTemporizadores();

        if (estadoActual == EstadoJefe.Caminando)
        {
            SeguirAlJugador();
        }
    }

    private void ManejarTemporizadores()
    {
        temporizadorAtaqueBasico -= Time.deltaTime;
        temporizadorCostados -= Time.deltaTime;

        if (temporizadorCostados <= 0 && estadoActual == EstadoJefe.Caminando)
        {
            StartCoroutine(AtaqueCaminarYDispararCostados());
        }
        else if (temporizadorAtaqueBasico <= 0 && estadoActual == EstadoJefe.Caminando)
        {
            ElegirAtaqueAleatorio();
        }
    }

    private void SeguirAlJugador()
    {
        Vector2 direccion = (player.position - transform.position).normalized;
        rb.velocity = direccion * velocidadCaminar;

        if (direccion.x < 0) spriteRenderer.flipX = true;
        else if (direccion.x > 0) spriteRenderer.flipX = false;

        animator.SetFloat("rot", (Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg) + 90, 0.1f, Time.deltaTime);
    }

    private void ElegirAtaqueAleatorio()
    {
        estadoActual = EstadoJefe.PreparandoAtaque;
        rb.velocity = Vector2.zero;

        int ataqueRandom = Random.Range(0, 2);
        if (ataqueRandom == 0)
            StartCoroutine(AtaquePisotonEspiral());
        else
            StartCoroutine(AtaqueEmbestida());
    }

    private IEnumerator AtaquePisotonEspiral()
    {
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(1.5f);

        estadoActual = EstadoJefe.Atacando;
        spriteRenderer.color = Color.white;

        animator.SetTrigger("attack");

        transform.position += new Vector3(0, 0.5f, 0);
        yield return new WaitForSeconds(0.2f);
        transform.position -= new Vector3(0, 0.5f, 0);

        int cantidadBalas = 12;
        float anguloPaso = 360f / cantidadBalas;

        for (int i = 0; i < cantidadBalas; i++)
        {
            float angulo = i * anguloPaso;
            Vector2 direccionBala = new Vector2(Mathf.Cos(angulo * Mathf.Deg2Rad), Mathf.Sin(angulo * Mathf.Deg2Rad));

            GameObject bala = Instantiate(balaPrefab, transform.position, Quaternion.identity);
            bala.GetComponent<Rigidbody2D>().velocity = direccionBala * 5f;
        }

        FinalizarAtaque();
    }

    private IEnumerator AtaqueEmbestida()
    {
        rb.velocity = Vector2.zero;
        spriteRenderer.color = Color.red;

        Vector2 posicionObjetivo = player.position;
        Vector2 direccionDash = (posicionObjetivo - (Vector2)transform.position).normalized;

        float distanciaVisual = velocidadEmbestida * duracionEmbestida;

        float tiempoCarga = 2f;
        float tiempoActual = 0f;

        if (lineaEmbestida != null) lineaEmbestida.enabled = true;

        while (tiempoActual < tiempoCarga)
        {
            if (lineaEmbestida != null)
            {
                lineaEmbestida.SetPosition(0, transform.position);
                lineaEmbestida.SetPosition(1, (Vector2)transform.position + (direccionDash * distanciaVisual));
            }

            rb.velocity = Vector2.zero;
            tiempoActual += Time.deltaTime;
            yield return null;
        }

        estadoActual = EstadoJefe.Atacando;
        if (lineaEmbestida != null) lineaEmbestida.enabled = false;
        spriteRenderer.color = Color.white;

        float tiempoPasado = 0f;

        while (tiempoPasado < duracionEmbestida)
        {
            rb.velocity = direccionDash * velocidadEmbestida;
            tiempoPasado += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        FinalizarAtaque();
    }

    private IEnumerator AtaqueCaminarYDispararCostados()
    {
        estadoActual = EstadoJefe.Atacando;
        temporizadorCostados = cooldownAtaqueCostados;

        spriteRenderer.color = Color.cyan;

        for (int i = 0; i < 5; i++)
        {
            SeguirAlJugador();

            GameObject balaDer = Instantiate(balaPrefab, transform.position, Quaternion.identity);
            balaDer.GetComponent<Rigidbody2D>().velocity = Vector2.right * 7f;

            GameObject balaIzq = Instantiate(balaPrefab, transform.position, Quaternion.identity);
            balaIzq.GetComponent<Rigidbody2D>().velocity = Vector2.left * 7f;

            yield return new WaitForSeconds(0.8f);
        }

        spriteRenderer.color = Color.white;
        FinalizarAtaque();
    }

    private void FinalizarAtaque()
    {
        temporizadorAtaqueBasico = cooldownAtaquesBasicos;
        estadoActual = EstadoJefe.Caminando;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && estadoActual != EstadoJefe.Muerto)
        {
            PlayerController lumina = collision.gameObject.GetComponent<PlayerController>();
            if (lumina != null)
            {
                lumina.TakeDamage(danoPorContacto);
            }
        }
    }

    public void RecibirDano(float cantidad)
    {
        if (estadoActual == EstadoJefe.Muerto) return;

        vidaActual -= cantidad;

        if (barraVida != null)
        {
            barraVida.value = vidaActual;
        }


        StartCoroutine(DamageFlashRoutine());

        if (vidaActual <= 0)
        {
            Morir();
        }
    }


    private IEnumerator DamageFlashRoutine()
    {

        for (int i = 0; i < allRenderers.Length; i++)
        {
            if (allRenderers[i] != null) allRenderers[i].material = whiteFlashMaterial;
        }

        yield return new WaitForSeconds(flashDuration);


        for (int i = 0; i < allRenderers.Length; i++)
        {
            if (allRenderers[i] != null) allRenderers[i].material = originalMaterials[i];
        }
    }

    private void Morir()
    {
        estadoActual = EstadoJefe.Muerto;
        rb.velocity = Vector2.zero;

        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetTrigger("death");

        if (barraVida != null)
        {
            barraVida.gameObject.SetActive(false);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerVictory();
        }

        Destroy(gameObject, 5);
    }
}