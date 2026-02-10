using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MonedaReclamable : MonoBehaviour
{
    public float velocidadRotacion = 100f;

    private AudioSource audioSource;
    private MeshRenderer meshRenderer;
    private Collider miCollider; // CAMBIO: Usamos Collider genérico
    private bool yaRecogida = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Si hay un modelo 3D hijo, a veces el Renderer está en el hijo, no en el padre.
        // GetComponentInChildren busca en el objeto o en sus hijos.
        meshRenderer = GetComponentInChildren<MeshRenderer>(); 
        
        miCollider = GetComponent<Collider>(); // Esto agarra Box, Sphere, Capsule o Mesh Collider
    }

    void Update()
    {
        if (!yaRecogida)
        {
            transform.Rotate(Vector3.left * velocidadRotacion * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !yaRecogida)
        {
            RecogerMoneda();
        }
    }

    void RecogerMoneda()
    {
        yaRecogida = true;
        Debug.Log("Moneda recogida");

        // Verificación de seguridad por si olvidaste poner el archivo de audio
        if (audioSource.clip != null)
        {
            audioSource.Play();
            // Destruimos esperando lo que dure el audio
            Destroy(gameObject, audioSource.clip.length);
        }
        else
        {
            // Si no hay audio, destruimos inmediatamente para evitar errores
            Debug.LogWarning("¡Falta el AudioClip en el AudioSource!");
            Destroy(gameObject);
        }

        // Apagar visuales y colisiones
        if(meshRenderer != null) meshRenderer.enabled = false;
        if(miCollider != null) miCollider.enabled = false;
        
        // Apagar cualquier otro hijo (partículas, luces, mallas extra)
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}