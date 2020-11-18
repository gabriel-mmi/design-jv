using UnityEngine;

// Ce composant gere la boutique de YAKO

public class Shop : MonoBehaviour
{
    public AudioClip buyClip, deniedClip; // Son d'achat et de refus d'achat

    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Quand le joueur ouvre la boutique
    public void OpenShop ()
    {
        PlayerBehavior.instance.canMove = false;
        PlayerBehavior.instance.canAttack = false;
        PlayerBehavior.instance.canInteract = false;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    // Quand le joueur appuie sur quitter (ou echap)
    public void OnQuitShop ()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        PlayerBehavior.instance.canAttack = true;
        PlayerBehavior.instance.canMove = true;
        PlayerBehavior.instance.canInteract = true;
        Time.timeScale = 1;
    }

    // Quand le joueur achete de la vie
    public void OnGetHeath ()
    {
        if(PlayerBehavior.instance.money >= 50)
        {
            GetComponent<AudioSource>().PlayOneShot(buyClip);
            PlayerBehavior.instance.money -= 50;
            PlayerBehavior.instance.health = PlayerBehavior.instance.maxHealth;

            OnQuitShop();
        }else
        {
            source.PlayOneShot(deniedClip);
        }
    }

    // Quand le joueur ameliore le boomerang
    public void OnGetNewBoomerang ()
    {
        if (PlayerBehavior.instance.money >= 100)
        {
            if (SaveManager.instance.boomerangsProfiles.IndexOf(PlayerBehavior.instance.currentBoomerang) < 3)
            {
                source.PlayOneShot(buyClip);
                PlayerBehavior.instance.money -= 100;
                BommerangProfile next = SaveManager.instance.boomerangsProfiles[SaveManager.instance.boomerangsProfiles.IndexOf(PlayerBehavior.instance.currentBoomerang) + 1];
                PlayerBehavior.instance.ChangeCurrentBoomerang(next);

                OnQuitShop();
            }
        }else
        {
            source.PlayOneShot(deniedClip);
        }
    }

    private void Update()
    {
        // Appuyer sur echap permet de quitter le shop
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnQuitShop();
        }
    }
}
