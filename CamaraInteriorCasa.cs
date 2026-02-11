using UnityEngine;
using System.Collections; // Necesario para usar IEnumerator

public class CamaraInteriorCasa : MonoBehaviour
{
    [Header("Configuración Interior")]
    public Camera camaraInterior;
    
    [Header("Sincronización con Puerta")]
    [Tooltip("Pon aquí los segundos que dura la animación de abrir")]
    public float tiempoEsperaPuerta = 1.5f; 

    // Referencias automáticas
    private Camera camaraPrincipal;
    private CamaraSeguidora scriptSeguimiento;
    
    // Variable para controlar la rutina de espera
    private Coroutine rutinaDeEspera;

    private void Start()
    {
        camaraPrincipal = Camera.main;

        if (camaraPrincipal != null)
        {
            scriptSeguimiento = camaraPrincipal.GetComponent<CamaraSeguidora>();
        }

        if (camaraInterior != null) 
        {
            camaraInterior.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Si entramos, iniciamos la cuenta atrás antes de cambiar la cámara
            // Guardamos la referencia por si el jugador sale antes de tiempo
            if (rutinaDeEspera != null) StopCoroutine(rutinaDeEspera);
            rutinaDeEspera = StartCoroutine(EsperarYCambiarCamara());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Si el jugador se arrepiente y sale antes de que termine la animación,
            // cancelamos la espera para que la cámara no se cambie sola.
            if (rutinaDeEspera != null) StopCoroutine(rutinaDeEspera);

            // Restauramos la cámara principal inmediatamente al salir
            ActivarModoInterior(false);
        }
    }

    // ESTA ES LA MAGIA: Una función que puede "esperar"
    IEnumerator EsperarYCambiarCamara()
    {
        // 1. Esperamos los segundos que le digas
        yield return new WaitForSeconds(tiempoEsperaPuerta);

        // 2. Una vez pasado el tiempo, cambiamos la cámara
        ActivarModoInterior(true);
    }

    private void ActivarModoInterior(bool estado)
    {
        if (camaraInterior != null) 
            camaraInterior.enabled = estado;

        if (camaraPrincipal != null) 
            camaraPrincipal.enabled = !estado;

        if (scriptSeguimiento != null) 
            scriptSeguimiento.enabled = !estado;
    }
}