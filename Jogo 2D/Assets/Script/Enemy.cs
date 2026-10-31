using UnityEngine;

public class PlataformaVoadora : MonoBehaviour
{
    /*
    public float speed = 2f;
    public Transform[] points;

    private int i;  

    void Start()
    {
        transform.position = points[i].position;
    }

    void Update()
    {
        if(Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;
            if (i == points.Length)
            {
                i = 0;
            }
            transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
        }
    }
    */

    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Vector3 nextposition;
    private SpriteRenderer spriteRenderer;
    private float previousX;

    private void Start()
    {
        nextposition = pointB.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        previousX = transform.position.x;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            nextposition,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, nextposition) < 0.25f)
        {
            nextposition = (nextposition == pointB.position)
                ? pointA.position
                : pointB.position;
        }

        if (nextposition.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else if (nextposition.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
    }

    /*
     Plataforma mover-se e quando o Player Ficar em Cima sofrer Inércia

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null, true);
        }
    }
    */
}
