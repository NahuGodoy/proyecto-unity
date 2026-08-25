using UnityEngine;

public class FloorMovement : MonoBehaviour
{
    public enum TipoMovimiento { Vertical, Horizontal }
    public TipoMovimiento tipo = TipoMovimiento.Vertical;

    public float velocidad = 2f;
    public float distancia = 2f;

    Vector3 inicio;

    void Start()
    {
        inicio = transform.position;
    }

    void Update()
    {
        float mov = Mathf.Sin(Time.time * velocidad) * distancia;

        if (tipo == TipoMovimiento.Vertical)
            transform.position = inicio + new Vector3(0, mov, 0);
        else
            transform.position = inicio + new Vector3(mov, 0, 0);
    }
}
