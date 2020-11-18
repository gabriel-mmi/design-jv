using UnityEngine;

// Tout les objets avec le quel le joueur peut interagir (via la touche E ou via la collision) héritent de cette classe abstraite

public abstract class Interactable : MonoBehaviour
{
    public virtual void OnInteract () // Lorsque le joueur appuie sur E
    {
        //...
    }

    public virtual void OnCollide (string tag) // Lorsque le joueur entre en collision
    {
        //...
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnCollide(collision.gameObject.tag);
    }
}
