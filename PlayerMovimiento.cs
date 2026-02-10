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

    private void Awake()
    {
        controlador = GetComponent<CharacterController>();

        if (camara == null && Camera.main != null)
            camara = Camera.main.transform;
    }

    void Update()
    {
        MoverJugadorEnPlano();
        Saltar();
        AplicarGravedad();
    }

    private void MoverJugadorEnPlano()
    {
        float ValorHorizontal = Input.GetAxisRaw("Horizontal");
        float ValorVertical = Input.GetAxisRaw("Vertical");

        Vector3 adelanteCamara = camara.forward;
        Vector3 derechaCamara = camara.right;

        

        adelanteCamara.y = 0f;
        derechaCamara.y = 0f;

        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        Vector3 direccionPlano = (derechaCamara * ValorHorizontal + adelanteCamara * ValorVertical);

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
        if (controlador.isGrounded && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;
            saltosRestantes = maxSaltos;
        }

        velocidadVertical.y += GravedadDelJugador * Time.deltaTime;
        controlador.Move(velocidadVertical * Time.deltaTime);
    }
    // Dentro de PlayerMovimiento
public void ImpulsarVertical(float fuerza)
{
    // Reemplaza o aumenta la velocidad vertical
    velocidadVertical.y = Mathf.Sqrt(fuerza * -2f * GravedadDelJugador);
}
}
