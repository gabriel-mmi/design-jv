using UnityEngine;

// Ce composant gere les piece lachées par les ennemis a leurs mort
// Elles peuvent etre ramassées en marchant dessus ou avec le boomerang

public class Coin : MonoBehaviour
{
    public AudioClip grabClip; // Son a jouer lors du ramssage
    [HideInInspector] public int money; // Quantité d'argent que raporte la piece (assignée par l'ennemi)

    AudioSource source;
    private float startTime;

    private void Start()
    {
        startTime = Time.time;
        source = GetComponent<AudioSource>();
        Destroy(gameObject, 9);
    }

    // Si le joueur ou le boomerang touche la piece alors elle se supprime et ajoute de l'argent au joueur
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            source.PlayOneShot(grabClip);
            PlayerBehavior.instance.AddMoney(money);
            Destroy(gameObject, grabClip.length); // Detruire une fois le son terminé
        }else if (collision.transform.tag == "Boomerang" && (Time.time - startTime) > 0.3f)
        {
            source.PlayOneShot(grabClip);
            PlayerBehavior.instance.AddMoney(money);
            Destroy(gameObject, grabClip.length);
        }
    }
}
