using UnityEngine;
using Unity.FPS.Game;
using System.Collections.Generic;

public class Turret : MonoBehaviour
{
    [Header("Configuracion")]
    public Transform spawnPoint;
    public GameObject projectilePrefab;
    public float fireRate = 1.5f;
    public float detectionRange = 15f;

    [Header("Destruccion")]
    public GameObject vfxDestruccion;

    private Transform player; // el objetivo actual (el jugador más cercano)
    private List<Transform> jugadoresConocidos = new List<Transform>();
    private float proximaActualizacionLista = 0f;
    private float intervaloActualizacionLista = 1f; // cada cuánto reescanea jugadores conectados

    private float nextFireTime;
    private Health health;
    private bool destruida = false;

    void Start()
    {
        ActualizarListaDeJugadores();

        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDie += OnTurretDestruida;
        }
    }

    void Update()
    {
        if (destruida) return;

        // Reescanea periódicamente por si se conecta/desconecta un jugador
        if (Time.time >= proximaActualizacionLista)
        {
            ActualizarListaDeJugadores();
            proximaActualizacionLista = Time.time + intervaloActualizacionLista;
        }

        player = EncontrarJugadorMasCercano();

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        Debug.Log("Turret: distancia=" + distance + " | rango=" + detectionRange + " | player=" + (player != null ? player.name : "NULL"));
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

    void ActualizarListaDeJugadores()
    {
        jugadoresConocidos.Clear();
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Jugadores encontrados con tag Player: " + jugadores.Length);
        foreach (GameObject j in jugadores)
        {
            if (j != null)
            {
                jugadoresConocidos.Add(j.transform);
                Debug.Log("  -> " + j.name + " en posición " + j.transform.position);
            }
        }
    }

    Transform EncontrarJugadorMasCercano()
    {
        Transform masCercano = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (Transform t in jugadoresConocidos)
        {
            if (t == null) continue; // jugador desconectado/destruido desde el ultimo escaneo

            float distancia = Vector3.Distance(transform.position, t.position);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                masCercano = t;
            }
        }

        return masCercano;
    }

    void Shoot()
    {
        if (projectilePrefab != null && spawnPoint != null)
        {
            Vector3 puntoObjetivo = player.position + Vector3.up * 1f;
            Vector3 direccionAlJugador = (puntoObjetivo - spawnPoint.position).normalized;
            Quaternion rotacionBala = Quaternion.LookRotation(direccionAlJugador);

            GameObject bala = Instantiate(projectilePrefab, spawnPoint.position, rotacionBala);

            BalaSimple script = bala.GetComponent<BalaSimple>();
            if (script != null)
            {
                script.origen = transform;
            }

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