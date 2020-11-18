using UnityEngine;

// Ce composant gere les fleche tirées par les ennemis a distance

public class EnemyBullet : MonoBehaviour
{
    [HideInInspector] public float damages; // Degats de la flehce, variable assignée par l'ennemi al a creation de l'objet
    private float speed = 100f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();    
    }

    void FixedUpdate()
    {
        rb.velocity = transform.up * speed; // Avance
    }

    // Si la fleche touche le joueur alors il prend des degats, si elle touche des debris elle les detruit
    // et si elle touche le boomerang du joueur alors elle se detruit
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerBehavior>().TakeDamages(damages);
            Destroy(gameObject);
        }else if (collision.transform.tag == "ObstaclesDebris")
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.transform.tag == "Obstacles" || collision.transform.tag == "Boomerang")
        {
            Destroy(gameObject);
        }
    }
}
