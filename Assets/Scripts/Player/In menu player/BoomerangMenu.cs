using UnityEngine;

// Ce composant decrit le comportement du bommerang mais uniquement dans le niveau MainMenu
// puisque la version In-game et la version MainMenu sont differentes
// aller voir Boomerang.cs pour la version In-game

public class BoomerangMenu : MonoBehaviour
{
    public float speed;
    public float returnDistance;

    Rigidbody2D rigidbody2d;
    Transform player;
    private Vector2 startPlayerPos;
    private bool followPlayer = false;
    private float currentReturnDist;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!followPlayer)
        {
            rigidbody2d.velocity = transform.up * speed;

            if (Vector2.Distance(transform.position, startPlayerPos) > currentReturnDist)
            {
                followPlayer = true;
            }
        }
        else
        {
            Vector2 playerDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;
            Debug.DrawRay(transform.position, playerDirection, Color.green);
            rigidbody2d.velocity = (playerDirection * speed);
        }
    }

    // Called after objet's creation
    public void Launch(Vector2 direction, float holdTime)
    {
        startPlayerPos = player.position;
        currentReturnDist = Mathf.Clamp(returnDistance + holdTime * 25, 0, 3);

        float angle = Vector2.SignedAngle(transform.up, direction);
        transform.eulerAngles = new Vector3(0, 0, angle);

        followPlayer = false;
    }

    // Coming back
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player" && followPlayer)
        {
            collision.gameObject.GetComponent<PlayerMenu>().haveBoomerang = true;
            Destroy(gameObject);
        }
    }
}
