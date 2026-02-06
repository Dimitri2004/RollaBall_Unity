using UnityEngine;

public class ScripDeImpulsorVertical : MonoBehaviour
{
    [Header("Potencia del impulso")]
    public float fuerzaImpulso = 10f;

    [Header("Animación de tamaño (opcional)")]
    public Vector3 tamañoExpandido = new Vector3(1.2f, 1.2f, 1.2f);
    public float tiempoAnimacion = 0.2f;

    private Vector3 tamañoOriginal;
    private bool animando = false;

    private void Start()
    {
        tamañoOriginal = transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica que el objeto tenga el script PlayerMovimiento
        PlayerMovimiento jugadorScript = other.GetComponent<PlayerMovimiento>();
        if (jugadorScript != null)
        {
            // Aplica impulso vertical
            jugadorScript.ImpulsarVertical(fuerzaImpulso);

            // Animación opcional
            if (!animando)
                StartCoroutine(AnimarImpulsor());
        }
    }

    private System.Collections.IEnumerator AnimarImpulsor()
    {
        animando = true;

        // Crece
        float tiempo = 0f;
        while (tiempo < tiempoAnimacion)
        {
            transform.localScale = Vector3.Lerp(tamañoOriginal, tamañoExpandido, tiempo / tiempoAnimacion);
            tiempo += Time.deltaTime;
            yield return null;
        }
        transform.localScale = tamañoExpandido;

        // Vuelve al tamaño original
        tiempo = 0f;
        while (tiempo < tiempoAnimacion)
        {
            transform.localScale = Vector3.Lerp(tamañoExpandido, tamañoOriginal, tiempo / tiempoAnimacion);
            tiempo += Time.deltaTime;
            yield return null;
        }
        transform.localScale = tamañoOriginal;

        animando = false;
    }
}
