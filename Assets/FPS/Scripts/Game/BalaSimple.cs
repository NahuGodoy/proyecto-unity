using UnityEngine;
using Unity.FPS.Game;

public class BalaSimple : MonoBehaviour
{
    public float velocidad = 25f;
    public float tiempoVida = 4f;

    [Header("Daño")]
    [Range(0f, 100f)]
    public float danoPorcentaje = 10f;

    [HideInInspector]
    public Transform origen; // quién disparó esta bala (se asigna al instanciar)

    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bala tocó: " + other.name); 
        // Si el objeto golpeado pertenece a la MISMA jerarquía que disparó la bala, ignorar
        if (origen != null && other.transform.root == origen.root) return;

        if (other.GetComponent<BalaSimple>()) return; // no chocar entre balas

        Health health = other.GetComponentInParent<Health>();
        if (health != null)
        {
            float danoReal = health.MaxHealth * (danoPorcentaje / 100f);
            health.TakeDamage(danoReal, gameObject);
        }

        Destroy(gameObject);
    }
}