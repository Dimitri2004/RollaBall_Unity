using UnityEngine;

public class PlayerMovimiento : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform camara;
    private CharacterController controlador;

    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento = 5f;

    [Header("Salto")]
    [SerializeField] private float fuerzaSalto = 5f;
    [SerializeField] private int maxSaltos = 2; 
    private int saltosRestantes;

    [Header("Gravedad")]
    [SerializeField] private float GravedadDelJugador = -9.8f;
    private Vector3 velocidadVertical;

    [Header("Respawn")]
    [Tooltip("Coordenadas X, Y, Z donde reaparecerá el jugador.")]
    [SerializeField] private Vector3 coordenadasReaparicion; 

    private void Awake()
    {
        controlador = GetComponent<CharacterController>();

        // Si no asignaste cámara manual, busca la principal
        if (camara == null && Camera.main != null)
            camara = Camera.main.transform;
    }

    private void Start()
    {
        // Si se te olvidó poner coordenadas, usa la posición inicial como seguridad
        if (coordenadasReaparicion == Vector3.zero)
        {
            coordenadasReaparicion = transform.position;
        }
    }

    void Update()
    {
        MoverJugadorEnPlano();
        Saltar();
        AplicarGravedad();
    }

    // ------------------------------------------------------------------------
    // PARTE IMPORTANTE: DETECCIÓN DE CHOQUE CON LA PARED
    // ------------------------------------------------------------------------
    // Esta función se ejecuta AUTOMÁTICAMENTE cuando el CharacterController
    // empuja contra otro collider mientras se mueve.
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Verificamos si el objeto que tocamos tiene el tag "Wall"
        if (hit.gameObject.CompareTag("Wall"))
        {
            Respawn();
        }
    }

    void Respawn()
    {
        Debug.Log("¡Muerte por pared! Reiniciando...");

        // 1. APAGAR EL CONTROLADOR
        // El CharacterController bloquea el cambio de posición si está encendido.
        controlador.enabled = false; 

        // 2. MOVER AL JUGADOR
        transform.position = coordenadasReaparicion;

        // 3. RESETEAR VELOCIDAD DE CAÍDA
        // Esto evita que sigas cayendo o saltando al reaparecer.
        velocidadVertical = Vector3.zero;
        saltosRestantes = maxSaltos;

        // 4. ENCENDER EL CONTROLADOR
        controlador.enabled = true;
    }

    // ------------------------------------------------------------------------
    // RESTO DE LÓGICA DE MOVIMIENTO
    // ------------------------------------------------------------------------

    private void MoverJugadorEnPlano()
    {
        float ValorHorizontal = Input.GetAxisRaw("Horizontal");
        float ValorVertical = Input.GetAxisRaw("Vertical");

        Vector3 adelante = (camara != null) ? camara.forward : Vector3.forward;
        Vector3 derecha = (camara != null) ? camara.right : Vector3.right;

        adelante.y = 0f;
        derecha.y = 0f;

        adelante.Normalize();
        derecha.Normalize();

        Vector3 direccionPlano = (derecha * ValorHorizontal + adelante * ValorVertical);

        if (direccionPlano.sqrMagnitude > 0.0001f)
            direccionPlano.Normalize();

        Vector3 desplazamientoXZ = direccionPlano * velocidadMovimiento * Time.deltaTime;
        controlador.Move(desplazamientoXZ);
    }

    private void Saltar()
    {
        if (Input.GetButtonDown("Jump") && saltosRestantes > 0)
        {
            velocidadVertical.y = Mathf.Sqrt(fuerzaSalto * -2f * GravedadDelJugador);
            saltosRestantes--;
        }
    }

    private void AplicarGravedad()
    {
        // Resetear saltos al tocar el suelo
        if (controlador.isGrounded && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;
            saltosRestantes = maxSaltos; 
        }

        velocidadVertical.y += GravedadDelJugador * Time.deltaTime;
        controlador.Move(velocidadVertical * Time.deltaTime);
    }

    public void ImpulsarVertical(float fuerza)
    {
        velocidadVertical.y = Mathf.Sqrt(fuerza * -2f * GravedadDelJugador);
    }

    // Dibuja una bola verde en la escena para ver dónde está el punto de respawn
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(coordenadasReaparicion, 0.5f);
    }
}