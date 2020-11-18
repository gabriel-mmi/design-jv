using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Ce composant gere le fonctionnement du teleporteur

public class Door : Interactable
{
    public int cost;
    public AudioClip openClip, deniedClip;

    private PlayerBehavior player;
    private bool used = false; // Defini si le teleporteur a deja ete utilisé (pour ne pas payer 2 fois)

    private void Start()
    {
        player = PlayerBehavior.instance.GetComponent<PlayerBehavior>();
    }

    // Quand le joueur appuie sur E
    public override void OnInteract()
    {
        if(player.money >= cost) // Si il a assé d'argent, le teleporteur s'ouvre
        {
            StartCoroutine(OpenDoor());
        }else // Sinon on joue un son signifiant le refus
        {
            GetComponent<AudioSource>().PlayOneShot(deniedClip);
        }
    }

    private IEnumerator OpenDoor ()
    {
        if(!used) // La porte (ou teleporteur) ne peut etre achetée que une seule fois
        {
            player.RemoveMoney(cost); // Retire l'argent au joueur (200 pieces)
            used = true;
            GetComponent<AudioSource>().PlayOneShot(openClip); // Joue le son de teleportation

            // Regen la vie du joueur si elle est trop basse
            if (player.health < 50)
            {
                player.health = 50;
            }

            SaveManager.instance.Save(); // Sauvegarde

            // Supprime tout les ennemis de la scene
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Destroy(enemy);
            }


            player.ActiveInteractMessage(false); // Cache le messsage "'E' pour interagir"

            // Apres avoir fini de jouer le son, je charge la scene suivante
            yield return new WaitForSeconds(openClip.length + 0.4f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    // Affiche le message "'E' pour interagir" lorsque le joueur entre dans le trigger du teleporteur (= porte)
    public override void OnCollide(string tag)
    {
        if(tag == "Player")
        {
            player.ActiveInteractMessage(true, "Coûte " + cost.ToString());
        }
    }

    // Cache le message quand il sort
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            player.ActiveInteractMessage(false);
        }
    }

}