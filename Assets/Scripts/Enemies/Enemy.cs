using UnityEngine;

// Cette classe abstraite pose les bases des deux ennemis (gestion de la vie, deplacements...)

public abstract class Enemy : MonoBehaviour
{
    // Propriétés
    public float health;
    [HideInInspector] public bool isDead;
    public float damages;
    public float moveSpeed;
    [HideInInspector] public float startSpeed;
    public int money;
    [Space]
    public GameObject coinPrefab;
    public AudioClip hurtClip, dieClip;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Transform player = null;

    private AudioSource source;

    // Appelé par la classe enfant lors de la fonction start
    public virtual void Initialize ()
    {
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();

        startSpeed = moveSpeed;
    }

    // Gestion de la vie
    public void TakeDamages (GameObject from, float damagesCount)
    {
        if (!isDead)
        {
            health -= damagesCount;

            // Sound
            source.PlayOneShot(hurtClip);

            // Flash effect
            GetComponent<SpriteFlash>().Flash();

            if (from != null)
            {
                // Recoil
                rb.AddForce((transform.position - from.transform.position).normalized * 300f, ForceMode2D.Impulse);
            }

            if (health <= 0)
            {
                health = 0;
                Die();
            }
        }
    }

    public void Die ()
    {
        if (!isDead)
        {
            isDead = true;
            moveSpeed = 0;

            // Add health to player
            player.GetComponent<PlayerBehavior>().AddHealth(player.GetComponent<PlayerBehavior>().damagesPerSecondes * 2);

            // Sound
            source.PlayOneShot(dieClip);

            // Spawn coin on ground
            GameObject instance = Instantiate(coinPrefab, transform.position, Quaternion.identity);
            instance.GetComponent<Coin>().money = money;

            Destroy(gameObject, dieClip.length);
        }
    }

    // Systeme de detection du joueur (detection actuellement instantanée)
    // Pour l'instant appelé dans la fonction Start par la class enfant
    public void TargetToPlayer ()
    {
        if(player == null)
        {
            player = PlayerBehavior.instance.transform;
        }
    }

    // Appelé dans l'Update par la class enfant
    // Decrit les deplacement des ennemis (ici, deplacements par defaut, l'ennemi court tout droit vers le joueur)
    public virtual void MovePatern ()
    {
        if(player != null)
        {
            Vector2 moveDirection = player.position - transform.position;
            moveDirection = moveDirection.normalized;
            rb.AddForce(moveDirection * (moveSpeed * (rb.drag > 0 ? rb.drag * 10 : 1)) * Time.deltaTime);
        }
    }
}
