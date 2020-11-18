using System.Collections.Generic;
using UnityEngine;

// Ce composant inflige des degats chaque a toutes les entités qui le touche (utile pour la lave du monde de sable)

public class KillZone : MonoBehaviour
{
    public float damagesPerSecondes;
    public AudioClip hurtClip;

    private float nextDamage;
    List<Enemy> enemies = new List<Enemy>(); // Ennemis present dans la killzone
    PlayerBehavior player;
    AudioSource source;
    bool playerInTrigger = false; // Defini si le joueur est dans la killzone

    private void Start()
    {
        player = PlayerBehavior.instance.GetComponent<PlayerBehavior>();
        source = GetComponent<AudioSource>();
    }

    // Enregistres les ennemis et le joueur si il touche la killzone
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (!enemies.Contains(enemy))
            {
                enemies.Add(enemy);
            }
        }
        else if (collision.transform.tag == "Player")
        {
            playerInTrigger = true;
        }
    }

    // Ne pas affecter les ennemis ou le joueur etant a l'exterieur de la killzone
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemies.Contains(enemy))
            {
                enemies.Remove(enemy);
            }
        }
        else if (collision.transform.tag == "Player")
        {
            playerInTrigger = false;
        }
    }


    private void Update()
    {
        if(nextDamage < Time.time) // A chaque secondes
        {
            nextDamage = Time.time + 1f; // Attendre ! seconde avant le prochain degat

            // Degats a tout les ennemis
            foreach (Enemy enemy in enemies)
            {
                enemy.TakeDamages(null, damagesPerSecondes);
            }

            // Degats au joueur
            if (playerInTrigger)
            {
                source.PlayOneShot(hurtClip);
                player.TakeDamages(damagesPerSecondes, false);
            }
        }
    }
}
