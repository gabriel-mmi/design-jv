using System.Collections.Generic;
using UnityEngine;

// Ce composant gere les objet avec lesquels peut discuter (dialog system)

public class NPC : Interactable
{
    public string npcName = "Name"; // Nom du NPC
    public List<string> lines = new List<string>(); // Phrases du NPC

    private DialogManager dialogManager;
    private PlayerBehavior player;

    private void Start()
    {
        dialogManager = PlayerBehavior.instance.dialogManager;
        player = PlayerBehavior.instance.GetComponent<PlayerBehavior>();
    }

    // Quand le joueur appuie sur E on commence le dialogue
    public override void OnInteract()
    {
        dialogManager.StartDialog(npcName, lines);
    }

    // Affiche le message "'E' pour interagir" lorsque le joueur touche le NPC
    public override void OnCollide(string tag)
    {
        if (tag == "Player")
        {
            player.ActiveInteractMessage(true);
        }
    }

    // Cache le message quand le joueur ne touche plus le NPC
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            player.ActiveInteractMessage(false);
        }
    }
}
