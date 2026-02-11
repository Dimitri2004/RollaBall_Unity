using UnityEngine;
using System.Collections;

public class ControladorEntrada : MonoBehaviour
{
    [Header("--- Control de la Puerta ---")]
    public Animator animadorPuerta; // Arrastra aquí el objeto que tiene el Animator
    public string parametroAnimator = "Abierta";

    [Header("--- Control de Cámaras ---")]
    public Camera camaraInterior;
    [Tooltip("Tiempo que tarda la puerta en abrirse antes de cambiar de cámara")]
    public float tiempoEsperaPuerta = 1.5f; 

    // Referencias internas
    private Camera camaraPrincipal;
    private CamaraSeguidora scriptSeguimiento;
    private Coroutine rutinaDeEspera;

    private void Start()
    {
        // 1. Configuración de Cámaras
        camaraPrincipal = Camera.main;
        if (camaraPrincipal != null)
        {
            scriptSeguimiento = camaraPrincipal.GetComponent<CamaraSeguidora>();
        }

        if (camaraInterior != null) camaraInterior.enabled = false;

        // 2. Autodetectar Animator si no lo has asignado manual y está en este objeto
        if (animadorPuerta == null)
        {
            animadorPuerta = GetComponent<Animator>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // PASO 1: Abrir la puerta inmediatamente
            if (animadorPuerta != null)
            {
                animadorPuerta.SetBool(parametroAnimator, true);
            }

            // PASO 2: Iniciar la cuenta atrás para la cámara
            if (rutinaDeEspera != null) StopCoroutine(rutinaDeEspera);
            rutinaDeEspera = StartCoroutine(EsperarYCambiarCamara());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Cancelar cambio de cámara si te arrepientes y sales rápido
            if (rutinaDeEspera != null) StopCoroutine(rutinaDeEspera);

            // PASO 1: Cerrar la puerta
            if (animadorPuerta != null)
            {
                animadorPuerta.SetBool(parametroAnimator, false);
            }

            // PASO 2: Volver a la cámara principal inmediatamente
            GestionarCamaras(false);
        }
    }

    // La Corrutina de espera
    IEnumerator EsperarYCambiarCamara()
    {
        // Esperamos a que la animación de la puerta termine
        yield return new WaitForSeconds(tiempoEsperaPuerta);

        // Cambiamos a la cámara interior
        GestionarCamaras(true);
    }

    // Función auxiliar para encender/apagar cosas
    private void GestionarCamaras(bool modoInterior)
    {
        if (camaraInterior != null) 
            camaraInterior.enabled = modoInterior;

        if (camaraPrincipal != null) 
            camaraPrincipal.enabled = !modoInterior;

        if (scriptSeguimiento != null) 
            scriptSeguimiento.enabled = !modoInterior;
    }
}