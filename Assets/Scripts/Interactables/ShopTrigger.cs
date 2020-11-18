using UnityEngine;

// Ce composant gere la porte qui mene vers la boutique

public class ShopTrigger : Interactable
{
    public AudioClip openClip; // Son a jouer lors de l'ouverture

    Shop shop;
    AudioSource source;

    private void Start()
    {
        shop = FindObjectOfType<Shop>();
        source = GetComponent<AudioSource>();
    }

    // Quand le joueur appuie sur E il ouvre la boutique
    public override void OnInteract()
    {
        // Toggle shop
        if (!PlayerBehavior.instance.dialogManager.inDialog)
        {
            shop.transform.GetChild(0).gameObject.SetActive(!shop.transform.GetChild(0).gameObject.activeSelf);

            // Si la boutique est maintenant ouverte alros appeler "OpenShop" de la class Shop
            if (shop.transform.GetChild(0).gameObject.activeSelf == true)
            {
                source.PlayOneShot(openClip);
                shop.GetComponent<Shop>().OpenShop();
            }
        }
    }

    // affiche le message "'E' pour interagir"
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            PlayerBehavior.instance.ActiveInteractMessage(true, "Boutique de Yako");
        }
    }

    // Cache le message
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            PlayerBehavior.instance.ActiveInteractMessage(false);
        }
    }

}
