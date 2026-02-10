using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrera : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Escribe aquí las coordenadas X, Y, Z donde quieres que reaparezca el jugador")]
    public Vector3 coordenadasReaparicion; // CAMBIO: Ahora es un Vector3, no un Transform

    // ESTO DIBUJA UNA ESFERA VERDE EN LA ESCENA PARA QUE VEAS DÓNDE ES EL PUNTO
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(coordenadasReaparicion, 0.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            RespawnJugador(collision.gameObject);
        }
    }

    void RespawnJugador(GameObject jugador)
    {
        CharacterController cc = jugador.GetComponent<CharacterController>();
        Rigidbody rb = jugador.GetComponent<Rigidbody>();

        // CASO A: CharacterController
        if (cc != null)
        {
            cc.enabled = false;
            jugador.transform.position = coordenadasReaparicion; // Usamos el Vector3 directo
            cc.enabled = true;
        }
        // CASO B: Rigidbody
        else if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            jugador.transform.position = coordenadasReaparicion; // Usamos el Vector3 directo
        }
        // CASO C: Transform simple
        else
        {
            jugador.transform.position = coordenadasReaparicion;
        }
    }
}