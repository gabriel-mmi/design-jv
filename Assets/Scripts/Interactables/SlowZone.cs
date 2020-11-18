using UnityEngine;

// Ce composant gere les mares d'eau qui ralentissent le joueur

public class SlowZone : MonoBehaviour
{

    // Quand le joueur / ennemis entrent, les ralentir fortement
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            PlayerBehavior script = collision.gameObject.GetComponent<PlayerBehavior>();
            script.m_speed = script.m_speed * 0.3f;
        }else if (collision.transform.tag == "Enemy")
        {
            Enemy script = collision.gameObject.GetComponent<Enemy>();
            script.moveSpeed = script.moveSpeed * 0.3f;
        }
    }

    // Quand le joueur / ennemis sortent alors leurs redonner leurs vitesse initiale
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            PlayerBehavior script = collision.gameObject.GetComponent<PlayerBehavior>();
            script.m_speed = script.startSpeed;
        }
        else if (collision.transform.tag == "Enemy")
        {
            Enemy script = collision.gameObject.GetComponent<Enemy>();
            script.moveSpeed = script.startSpeed;
        }
    }
}
