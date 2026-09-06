using UnityEngine;
using Unity.FPS.Game;
using System.Linq;

public class BalaSimple : MonoBehaviour
{
    public float velocidad = 25f;
    public float tiempoVida = 4f;
    public GameObject vfxImpacto;

    [Header("Daño")]
    [Range(0f, 100f)]
    public float danoPorcentaje = 10f;

    [HideInInspector]
    public Transform origen;

    private Vector3 posicionAnterior;

    void Start()
    {
        posicionAnterior = transform.position;
        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        float deltaTimeSeguro = Mathf.Min(Time.deltaTime, 0.05f);
        float distanciaFrame = velocidad * deltaTimeSeguro;
        Vector3 direccion = transform.forward;

        RaycastHit[] hits = Physics.RaycastAll(
        posicionAnterior,
        direccion,
        distanciaFrame,
        ~0,
        QueryTriggerInteraction.Collide
        );

        

        // Ordenar por distancia para procesar el impacto más cercano válido primero
        var hitsOrdenados = hits.OrderBy(h => h.distance);

        foreach (RaycastHit hit in hitsOrdenados)
        {
            // Ignorar cualquier collider que pertenezca a quien disparó la bala
            if (origen != null && hit.collider.transform.root == origen.root) continue;

            // Ignorar otras balas
            if (hit.collider.GetComponent<BalaSimple>() != null) continue;

            // Este es un impacto real: procesar y salir
            ProcesarImpacto(hit.collider, hit.point);
            return;
        }

        // Si no hubo ningún impacto válido, avanzar normalmente
        transform.position = posicionAnterior + direccion * distanciaFrame;
        posicionAnterior = transform.position;
    }

    void ProcesarImpacto(Collider other, Vector3 puntoImpacto)
    {
        Health health = other.GetComponentInParent<Health>();
        if (health != null)
        {
            float danoReal = health.MaxHealth * (danoPorcentaje / 100f);
            health.TakeDamage(danoReal, gameObject);
        }

        transform.position = puntoImpacto;
        if (vfxImpacto != null) Instantiate(vfxImpacto, puntoImpacto, Quaternion.identity);
        Destroy(gameObject);
    }
}