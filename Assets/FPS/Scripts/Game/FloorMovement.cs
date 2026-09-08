using UnityEngine;

public class FloorMovement : MonoBehaviour
{
    public enum TipoMovimiento { Vertical, Horizontal }
    public TipoMovimiento tipo = TipoMovimiento.Vertical;

    public float velocidad = 2f;
    public float distancia = 2f;

    public float pausaEnPuntoAlto = 1.5f;

    public Vector3 DeltaMovimiento { get; private set; }

    private Vector3 inicio;
    private Vector3 posicionAnterior;

    private float tiempoInterno = 0f;
    private bool enPausa = false;
    private float tiempoPausaRestante = 0f;
    private bool topeAlcanzado = false; // evita repausar apenas retoma el movimiento

    void Start()
    {
        inicio = transform.position;
        posicionAnterior = transform.position;
    }

    void Update()
    {
        float mov;

        if (enPausa)
        {
            // Mientras pausa, se queda fija en el punto más alto
            mov = distancia;
            tiempoPausaRestante -= Time.deltaTime;
            if (tiempoPausaRestante <= 0f)
                enPausa = false;
        }
        else
        {
            // Solo avanza el tiempo del seno cuando NO está en pausa
            tiempoInterno += Time.deltaTime;
            mov = Mathf.Sin(tiempoInterno * velocidad) * distancia;

            if (tipo == TipoMovimiento.Vertical)
            {
                // Detecta que llegó (casi) al tope y todavía no pausó en esta vuelta
                if (!topeAlcanzado && mov >= distancia * 0.999f)
                {
                    enPausa = true;
                    topeAlcanzado = true;
                    tiempoPausaRestante = pausaEnPuntoAlto;
                }
                // Reset del flag una vez que bajó lo suficiente, para permitir pausar de nuevo en la próxima subida
                else if (mov < distancia * 0.9f)
                {
                    topeAlcanzado = false;
                }
            }
        }

        if (tipo == TipoMovimiento.Vertical)
            transform.position = inicio + new Vector3(0, mov, 0);
        else
            transform.position = inicio + new Vector3(mov, 0, 0);

        DeltaMovimiento = transform.position - posicionAnterior;
        posicionAnterior = transform.position;
    }
}