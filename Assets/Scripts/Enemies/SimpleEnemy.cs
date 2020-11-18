using UnityEngine;

// Cette class herite de la classe abstraite Enemy
// Ce composant gere les ennemis au corps a corps

public class SimpleEnemy : Enemy
{
    public float recoilForce; // Recul a appliquer lors de la collision avec le joueur

    private Animator animator;

    void Start()
    {
        // Appel des procédure de la classe Enemy
        Initialize();
        TargetToPlayer();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        MovePatern();
        animator.SetBool("isMoving", rb.velocity != Vector2.zero); // Mettre a jour l'animation de course
    }

    // Lors de la collision avec le joueur je lui applique des degats
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            if(!isDead)
            {
                player.GetComponent<PlayerBehavior>().TakeDamages(damages);

                // Applique les recul
                Vector2 recoilDirection = transform.position - player.position;
                recoilDirection = recoilDirection.normalized;
                rb.AddForce(recoilDirection * (recoilForce * (rb.drag > 0 ? rb.drag : 1)), ForceMode2D.Impulse);
            }
        }
    }
}
