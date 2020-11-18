using UnityEngine;

// Cette class herite de la classe abstraite Enemy
// Ce composant gere les ennemis a distance

public class DistanceEnemy : Enemy
{
    public float fireRate;
    public GameObject bulletPrefab;

    private float nextFire;
    private float posUpdateRate = 7f;
    private float nextPosUpdate;

    private Vector2 targetPosition; // Position vers laquelle l'ennemis doit aller (change toutes les 7 secondes (posUpdateRate) )
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

        // Walk animation on move
        animator.SetBool("isMoving", rb.velocity != Vector2.zero);
    }

    // Override de la procédure MovePatern de la class Enemy
    public override void MovePatern()
    {
        // Change de position cible
        if(nextPosUpdate < Time.time || Vector2.Distance(transform.position, player.position) > 120f)
        {
            nextPosUpdate = Time.time + posUpdateRate;
            targetPosition = (Vector2)player.position + Random.insideUnitCircle.normalized * 70f;
        }
        
        // Tant que je ne suis pas proche de la position cible, j'avance
        if(Vector2.Distance(transform.position, targetPosition) > 5f)
        {
            Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;
            rb.AddForce(moveDirection * moveSpeed);
        }

        // Tire regulierement
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            // Calcul de l'angle vers le joueur
            Vector2 shootDirection = (player.position - transform.position).normalized;
            float lookAtAngle = Vector2.SignedAngle(transform.up, shootDirection);

            // Creation de la flehce
            GameObject instance = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            instance.transform.eulerAngles = new Vector3(0, 0, lookAtAngle);
            instance.GetComponent<EnemyBullet>().damages = damages; // Assignation de la variable damages de la fleche
        }
    }
}
