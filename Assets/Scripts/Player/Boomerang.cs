using EZCameraShake;
using UnityEngine;

// Ce composant decrit le comportement du boomerang une fois lancé

public class Boomerang : MonoBehaviour
{
    // Ces variables sont assignées lors de la creation de l'instance par PlayerBehaviour puisque elle doivent etre égales
    // au boomerang equipé dans PlayerBehaviour
    public float damages;
    public float speed;
    public float returnDistance;

    Rigidbody2D rigidbody2d;
    Transform player;

    private Vector2 startPlayerPos;
    private bool followPlayer = false;
    private float currentReturnDist;
    private bool canDestroyDebris = true;

    void Awake ()
    {
        player = GameObject.FindWithTag("Player").transform;
        rigidbody2d = GetComponent<Rigidbody2D>();
    }


    void Update ()
    {
        // Si je ne doit pas suivre le joueur alors j'avance tout droit
        if (!followPlayer)
        {
            rigidbody2d.velocity = transform.up * speed;

            // Si le boomerang depasse la distance max a laquelle il peut aller
            // alors il passe en mode "Suivre le joueur"
            if(Vector2.Distance(transform.position, startPlayerPos) > currentReturnDist)
            {
                followPlayer = true;
            }
        }
        // Si je dois suivre le joueur alos j'avance en direction de ce dernier
        else
        {   
            rigidbody2d.velocity = ((Vector2)player.position - (Vector2)transform.position).normalized * speed * 4;
        }
    }

    // Launch est appelée a la creation de l'objet, elle permet d'initialiser la course du boomerang
    public void Launch (Vector2 direction, float holdTime)
    {
        // Sauvegarde la position du joueur pour en calculer sa distance dans Update
        startPlayerPos = player.position;
        // La distance maximale a laquelle le bommerang peut aller depend aussi de la durée de maintien de la touche gauche de la souris
        // (plus le joueur maintenant la touche, plus le boomerang va loin)
        currentReturnDist = Mathf.Clamp(returnDistance + holdTime * 25, 0f, 100f);

        // S'oriente vers la direction choisi par PlayerBehaviour (direction de la souris)
        float angle = Vector2.SignedAngle(transform.up, direction);
        transform.eulerAngles = new Vector3(0, 0, angle);

        // Pour l'instant le boomerang ne revient pas vers le joueur
        followPlayer = false;
    }

    // Quand le boomerang touche quelque chose :
    private void OnTriggerEnter2D (Collider2D collision)
    {
        // Si il sagit du joueur cela veut dire qu'il peut maintenant relancer un boomerang
        if(collision.gameObject.tag == "Player" && followPlayer)
        {
            player.GetComponent<PlayerBehavior>().haveBoomerang = true;
            Destroy(gameObject);
        }
        // Si il sagit d'un ennemi alors il faut lui appliquer des degats
        else if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamages(gameObject, damages);
            // Screen shake
            CameraShaker.Instance.ShakeOnce(3f, 6f, 0.1f, 0.1f);
        }
        // Si il s'agit d'un obstacle alors il faut le detruire
        else if (collision.gameObject.tag == "Obstacles")
        {
            collision.gameObject.GetComponent<Obstacles>().OnCollide(transform.tag);
            canDestroyDebris = false;
        }
        // Si il sagit d'un debris (objet crée apres la destruction d'un obstacle) alors le detruire
        else if (collision.gameObject.tag == "ObstaclesDebris")
        {
            if (canDestroyDebris)
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
