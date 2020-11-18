using System.Collections.Generic;
using UnityEngine;

// Ce composant gere les obstacles, autrement dit les objets cassables par le joueur avec le boomerang ou le dash
// une fois cassé un obstacles se brise en plusieurs morceaux (dites "parts" dans le code)

public class Obstacles : Interactable
{
    public int partsCounts; // Nombre de morceaux a creer lors de la destruction
    public List<GameObject> partsPrefabs = new List<GameObject>(); // Prefabs des mocreaux
    public AudioClip breakClip; // Son a jouer lors de la destruction
    private bool canBeDestroyed = true;

    public override void OnCollide(string tag)
    {
        if (canBeDestroyed)
        {
            // Lorsque le boomerang ou le joueur (pendant le dash) touche l'obstacle alors il se detruit
            if (tag == "Boomerang" || tag == "Player")
            {
                canBeDestroyed = false;

                // Spawn des debris
                for (int i = 0; i < partsCounts; i++)
                {
                    GameObject instance = Instantiate(partsPrefabs[Random.Range(0, partsPrefabs.Count)], transform.position, Quaternion.identity);
                    instance.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 120f, ForceMode2D.Impulse);

                    // Le premier debris spawné joue un son
                    // Ce n'est pas l'obstacle en lui meme qui le joue car il est detruit juste apres
                    // Le son ne pourrait alors pas etre entendu
                    if (i == 0)
                    {
                        AudioSource source = instance.AddComponent<AudioSource>();
                        source.volume = 0.02f;
                        source.PlayOneShot(breakClip);
                    }
                }

                // Se detruit
                Destroy(gameObject);
            }
        }
    }
}
