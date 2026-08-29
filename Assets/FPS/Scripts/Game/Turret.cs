using UnityEngine;
using Unity.FPS.Game;

public class Turret : MonoBehaviour
{
    [Header("Configuracion")]
    public Transform player;
    public Transform spawnPoint;
    public GameObject projectilePrefab;
    public float fireRate = 1.5f;
    public float detectionRange = 15f;

    [Header("Destruccion")]
    public GameObject vfxDestruccion;

    private float nextFireTime;
    private Health health;
    private bool destruida = false;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDie += OnTurretDestruida;
        }
    }

    void Update()
    {
        if (destruida || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            Vector3 direction = player.position - transform.position;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && spawnPoint != null)
        {
            Vector3 puntoObjetivo = player.position + Vector3.up * 1f;
            Vector3 direccionAlJugador = (puntoObjetivo - spawnPoint.position).normalized;
            Quaternion rotacionBala = Quaternion.LookRotation(direccionAlJugador);

            GameObject bala = Instantiate(projectilePrefab, spawnPoint.position, rotacionBala);

            // Asignar quién disparó, para que la bala se ignore a sí misma correctamente
            BalaSimple script = bala.GetComponent<BalaSimple>();
            if (script != null)
            {
                script.origen = transform;
            }

            // Ignorar colisión con TODOS los colliders de la torreta (raíz + hitboxes hijas)
            Collider colBala = bala.GetComponent<Collider>();
            if (colBala != null)
            {
                Collider[] collidersTorreta = GetComponentsInChildren<Collider>();
                foreach (Collider c in collidersTorreta)
                {
                    Physics.IgnoreCollision(colBala, c);
                }
            }
        }
        else
        {
            Debug.LogWarning("Falta asignar el ProjectilePrefab o el SpawnPoint en la Torreta.");
        }
    }

    void OnTurretDestruida()
    {
        destruida = true;

        if (vfxDestruccion != null)
        {
            Instantiate(vfxDestruccion, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (health != null)
        {
            health.OnDie -= OnTurretDestruida;
        }
    }
}