using UnityEngine;

public class CamaraSeguidora : MonoBehaviour
{
    
    [Header("Referencia al jugador")]
    [SerializeField] private Transform jugador;

    [Header("Suavizado")]
    [SerializeField] private float suavizado = 5f;

    private Vector3[] offsets; // Las tres vistas
    private int vistaActual = 0;

    private void Start()
    {
        if (jugador == null)
        {
            Debug.LogError("Jugador no asignado en CamaraSeguidora.");
            return;
        }

        // Definir offsets de las tres vistas
        offsets = new Vector3[3];

        // 1. Vista lateral (derecha del jugador)
        offsets[0] = new Vector3(15f, 4f, 35f);

        // 2. Vista cenital (desde arriba)
        offsets[1] = new Vector3(1f, 25f, -5f);

        // 3. Vista perfil/tercera persona (detrás)
        offsets[2] = new Vector3(0f, 9f, -25f);
    }

    private void LateUpdate()
    {
        if (jugador == null) return;

        // Cambiar vista según teclas 1,2,3
        if (Input.GetKeyDown(KeyCode.Alpha1)) vistaActual = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) vistaActual = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) vistaActual = 2;

        // Posición deseada según offset actual
        Vector3 posicionDeseada = jugador.position + offsets[vistaActual];
    

        // Para vista lateral y perfil, mantener suavizado independiente de altura
        if (vistaActual != 1) // lateral o tercera persona
        {
            Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);
            // Mantener suavizado verticalmente solo un poco para que no “flote raro”
            transform.position = new Vector3(posicionSuavizada.x, posicionDeseada.y, posicionSuavizada.z);
        }
        else
        {
            // Vista cenital: seguir suavemente todo
            transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);
        }

        // LookAt: siempre mira un poco arriba del centro del círculo
        Vector3 objetivo = jugador.position + Vector3.up * 0.5f;

        transform.LookAt(objetivo);

    }
}
