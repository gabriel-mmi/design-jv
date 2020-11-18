/* Author : Raphaël Marczak - 2018/2020, for MIAMI Teaching (IUT Tarbes) and MMI Teaching (IUT Bordeaux Montaigne)
 * 
 * This work is licensed under the CC0 License. 
 * 
 */

using Cinemachine;
using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Ce composant decrit le comportement du joueur
// IMPORTANT : 
//              - currentBoomerang : stats du boomerang (vitesse, degats...)
//              - bommerangSprite : boomerang dans la main du joueur suivant la camera
//              - boomerangPrefab : objet a spawner lors de l'attaque

public class PlayerBehavior : MonoBehaviour
{
    // Singleton
    public static PlayerBehavior instance;

    // Propriétés

    // Stats general du joueur
    [Header("Stats")]

    // Vie
    public float _health;
    public float health
    {
        get => _health;
        set
        {
            // Quand la variable health est modifiée il faut aussi mettre a jour l'interface
            _health = value;
            healthBar.value = health;
        }
    }

    [HideInInspector] private float _maxHealth;
    public float maxHealth
    {
        get => _maxHealth;
        set
        {
            // Quand la variable maxHealth (inutile ici) est modifiée il faut aussi mettre a jour l'interface
            _maxHealth = value;
            healthBar.maxValue = value;
        }
    }

    public float damagesPerSecondes; // Degats que prend le joueur par secondes (lien avec le theme TAC TIC)
    private float nextDamagesPersonds;

    // Argent
    public int _money;
    public int money
    {
        get => _money;
        set
        {
            // Quand la variable money est modifiée il faut aussi mettre a jour l'interface
            _money = value;
            moneyText.text = value.ToString();
        }
    }

    // Vitesse
    public float m_speed;
    [HideInInspector] public float startSpeed;

    // Bommerang actuellement équipé
    public BommerangProfile currentBoomerang;

    // Stats du dash
    [Header("Dash")]
    public float dashSpeed;
    public float dashRate;
    private float nextDash;
    public float dashDuration;
    
    // References a d'autres objets de la scene
    [Header("References")]
    public GameObject sprite; // Visuel du joueur, contient son Animator et le composant PlayerAnimatorEvent

    public GameObject boomerangPrefab; // La prefab du Boomerang a lancer lors que l'attaque
    public GameObject bommerangSprite; // Boomerang que le joueur a en main (il n'est que la que pour le visuel)

    public DialogManager dialogManager; // DialogManger permet d'afficher un dialogue

    // Particule de fumée derriere le joueur
    public ParticleSystem smokeEffect;
    private ParticleSystem.EmissionModule emission;

    // References aux elements d'interface
    [Header("UI")]
    public Slider healthBar; // Barre de vie
    public Animator dashRelaodUI; // Indication du rechargement du dash
    public GameObject interactMessage; // Message d'interaction ("'E' pour interagir")
    public Text moneyText;

    // References aux sons du joueur
    [Header("Sounds")]
    public AudioClip dashClip; // Son lors du dash
    public AudioClip hurtClip; // Son lorsque le joueur prend des degats

    // Propriétés publique
    Rigidbody2D m_rb2D;
    Animator animator;
    AudioSource source;
    private List<GameObject> currentTriggers = new List<GameObject>(); // Trigger en contact avec le joueur a un instant T
    private float aimHoldTime; // Temps durant lequel le joueur a maintenu la touche d'attaque (clic gauche)
    private CinemachineVirtualCamera cinemachineCamera; // Reference a la camera

    // Etats du joueur
    [HideInInspector] public bool canMove = true, canTakeDamages = true, canInteract = true;
    [HideInInspector] public bool haveBoomerang = true; // Decris si le joueur a le boomerang en main ou si il l'a lancé
    [HideInInspector] public bool isDashing = false; // Decris si le joueur est en train de dasher ou non

    private bool _canAttack = true;
    public bool canAttack
    {
        get => _canAttack;
        set
        {
            _canAttack = value;
            // Empeche le boomerang (qui est dans la main du joueur) de s'orienté si le joueur ne peut pas attaquer
            if (value == true)
            {
                GetComponentInChildren<LookAtMouse>().isActive = true;
            }
            else
            {
                if(GetComponentInChildren<LookAtMouse>() != null)
                {
                    GetComponentInChildren<LookAtMouse>().isActive = false;
                }
            }
        }
    }

    #region Game logic
    void Awake()
    {
        // Singleton
        if(instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }

        // Assignation des variables
        m_rb2D = gameObject.GetComponent<Rigidbody2D>();
        animator = sprite.GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        cinemachineCamera = FindObjectOfType<CinemachineVirtualCamera>();

        canAttack = true;

        // Vie
        maxHealth = health;
        healthBar.value = health;

        startSpeed = m_speed;

        // Le module emmission de l'effet de fumée permet de controller le nombre de particules emises en temps réel
        emission = smokeEffect.emission;

        Cursor.visible = false; // Cacher le curseur

        interactMessage.SetActive(false);
    }

    void Update()
    {

        // Le joueur perd 1 point de vie chaque secondes (lien theme TAC TIC)
        if (FindObjectOfType<EnemiesSpawnArea>() != null)
        {
            if (Time.time > nextDamagesPersonds)
            {
                // Mais le joueur perd de la vie uniquement pendant les vagues d'ennemis et non dans les temps de pause
                if (FindObjectOfType<EnemiesSpawnArea>().inWave)
                {
                    nextDamagesPersonds = Time.time + 1f; // Attendre une secondes avant le prochain degat
                    health -= damagesPerSecondes; // Retire de la vie
                }
            }
        }


        // Si la souris est a gauche : tourner le joueur a gauche, sinon a droite
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Obtenir la position de la souris
        if (canMove)
        {
            if (mousePos.x > transform.position.x)
            {
                sprite.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (mousePos.x < transform.position.x)
            {
                sprite.transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }


        // Dash
        if (Time.time > nextDash)
        {
            // Si je peux dasher alors cacher la barre de reachargement du dash
            dashRelaodUI.gameObject.SetActive(false);

            // Si j'appuie sur le bouton droit de la souris alors je dash
            if (Input.GetMouseButtonDown(1) && canMove)
            {
                StartCoroutine(Dash(dashDuration));
            }
        }


        // Attaque
        bommerangSprite.SetActive(haveBoomerang); // Si le joueur n'a pas le boomerang en main il faut cacher le sprite du boomerang

        if (canAttack)
        {
            // Quand je commence a viser (clic gauche)
            if (Input.GetMouseButtonDown(0))
            {
                if (haveBoomerang)
                {
                    aimHoldTime = 0f;
                }
            }
            // Quand je maintien la visée
            else if (Input.GetMouseButton(0))
            {
                if (haveBoomerang)
                {
                    // Compte le temps durant lequel je maintien la touche
                    aimHoldTime += Time.deltaTime;
                    // Zoom la camera
                    cinemachineCamera.m_Lens.OrthographicSize = Mathf.Lerp(cinemachineCamera.m_Lens.OrthographicSize, 50, Time.deltaTime);
                    // Empeche le joueur de se deplacer
                    canMove = false;
                }
            }
            // Quand je relache la visée je lanche le boomerang
            else if (Input.GetMouseButtonUp(0))
            {
                if (haveBoomerang)
                {
                    Attack();
                    canMove = true;
                }
            }
            // Si je ne vise pas
            else
            {
                // Dézoomer la camera
                if(Mathf.RoundToInt(cinemachineCamera.m_Lens.OrthographicSize) != 70)
                {
                    cinemachineCamera.m_Lens.OrthographicSize = Mathf.Lerp(cinemachineCamera.m_Lens.OrthographicSize, 70, 5 * Time.deltaTime);
                }
            }
        }


        // Interactions avec les triggers quand j'appuie sur E
        if (Input.GetKeyDown(KeyCode.E) && canInteract && !dialogManager.inDialog)
        {
            // Pour chaque triggers j'appel la fonction "OnInteract" qui decris le comportement du trigger lors de l'interaction avec le joueur
            foreach (GameObject trigger in currentTriggers)
            {
                if(trigger != null)
                {
                    if (trigger.GetComponent<Interactable>() != null)
                    {
                        trigger.GetComponent<Interactable>().OnInteract();
                    }
                }
            }
        }


        // Ouverture et fermeture du menu pause
        if (Input.GetKeyDown(KeyCode.Escape) && !dialogManager.inDialog)
        {
            if (FindObjectOfType<PauseManger>().transform.GetChild(0).gameObject.activeSelf)
            {
                FindObjectOfType<PauseManger>().OnResume();
            }else
            {
                FindObjectOfType<PauseManger>().OpenMenu();
            }
        }


        // Mettre a jour l'orientation des particules de fumée pour suivre la direction du joueur
        float smokeEffectAngle = Vector2.SignedAngle(Vector2.right, new Vector2(m_rb2D.velocity.x, -m_rb2D.velocity.y)) + 180f;
        smokeEffect.transform.eulerAngles = new Vector3(smokeEffectAngle, 90f, 0);
        // Mettre a jour la quantité de particules (si le joueur ne bouge pas il ne doit aps y avoir de fumée)
        emission.rateOverTime = m_rb2D.velocity.magnitude != 0 ? 20f : 0f;

    }


    // Pour les modifications l'ajout de force au Rigidbody
    void FixedUpdate()
    {
        // Axe x : Q et D, Axe y : Z et S
        Vector2 inputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        // Si je suis en mouvement alors joue l'animation de course du joueur
        animator.SetBool("isMoving", (inputs != Vector2.zero && m_rb2D.velocity != Vector2.zero && canMove));

        // Si le joueur n'a pas le droit de se deplacer alors arreter
        if (!canMove)
        {
            m_rb2D.velocity = Vector2.zero;
            return;
        }

        // Si je ne dash pas alors je marche normalement
        if (!isDashing)
        {
            m_rb2D.velocity = inputs * m_speed; // Applique la force
        }
    }
    #endregion


    private void Attack ()
    {
        haveBoomerang = false; // Cacher le bommerang en main

        Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized; // Direction du boomerang

        // Creation de l'objet boomerang (celui qui se deplace et non celui en main)
        GameObject instance = Instantiate(boomerangPrefab, (Vector2)FindObjectOfType<LookAtMouse>().transform.position, Quaternion.identity);

        instance.GetComponentInChildren<SpriteRenderer>().sprite = currentBoomerang.sprite; // Changement de son visuel (il doit etre le meme que celui equipé par le joueur)
        
        // Modifie les stats du boomerang pour qu'elles soit les memes que le boomerang équipé par le joueur 
        Boomerang script = instance.GetComponent<Boomerang>();
        script.damages = currentBoomerang.damages;
        script.speed = currentBoomerang.speed;
        script.returnDistance = currentBoomerang.returnDistance;

        // Appel la procédure Launch
        script.Launch(direction, aimHoldTime);
    }


    private IEnumerator Dash (float duration)
    {
        // Avant le dash
        float startTime = Time.time; // Sauvegarde de la date du debut de Dash
        Vector2 dashDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized; // Direction du dash (vers la souris)

        canTakeDamages = false; // Il est impossible de prendre des degats lors du dash
        isDashing = true;

        // Joue le son du dash
        source.PlayOneShot(dashClip);

        // Animation de dash
        animator.SetBool("isDashing", true);

        // Met a jour la barre de rechargement du dash
        dashRelaodUI.gameObject.SetActive(false);
        dashRelaodUI.SetBool("dashReloadIsFull", false);

        // Screen shake
        CameraShaker.Instance.ShakeOnce(10f, 3f, 0.1f, 0.1f);

        // Tant que le durée maximale du dash n'a pas été dépassée alors j'applique une force
        while ((Time.time - startTime) < duration)
        {
            m_rb2D.velocity = dashDirection * dashSpeed;
            yield return null;
        }

        // Une fois le dash terminé
        //Affiche la barre de rechargement du dash
        dashRelaodUI.gameObject.SetActive(true);
        dashRelaodUI.SetBool("dashReloadIsFull", true);

        // Joue l'animation d'idle
        animator.SetBool("isDashing", false);

        isDashing = false;
        canTakeDamages = true; // Peut de nouveau prendre des degats

        // Retrouve la vitesse normale (bug fix)
        m_speed = startSpeed;

        // Attends x secondes avant le prochain dash
        nextDash = Time.time + dashRate;
    }

    #region Vie
    public void TakeDamages(float count, bool playSound = true)
    {
        if (canTakeDamages) // Ne peut pas prendre de degats durant le dash
        {
            health -= count; // Prend les degats

            // Screen shake
            CameraShaker.Instance.ShakeOnce(6f, 4f, 0.1f, 0.1f);

            // Joue l'animation de rage (le joueur devient rouge, cela n'a pas d'impact sur le gameplay)
            StopCoroutine("RageMode");
            StartCoroutine(RageMode(4f));

            // Joue le son de degat
            if (playSound)
            {
                source.PlayOneShot(hurtClip);
            }

            // Si je n'ai plus de vie alors je meurs
            if (health < 0)
            {
                Die();
            }
        }
    }

    public void AddHealth(float count)
    {
        health += count;

        // Si je depasse la vie max
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void Die()
    {
        // Si je meurt alors je sauvegarde (la vie n'est aps sauvegardée) et je charge la scene de mort
        health = 0;
        SaveManager.instance.Save();
        SceneManager.LoadScene(1);
    }

    // Animation du mode rage (lorsque je prend des degats) durant x secondes
    private IEnumerator RageMode(float duration)
    {
        animator.SetBool("inRageMode", true);
        yield return new WaitForSeconds(duration);
        animator.SetBool("inRageMode", false);
    }
    #endregion

    #region Argent
    public void AddMoney(int count)
    {
        money += count;
    }

    public void RemoveMoney(int count)
    {
        money -= count;

        if (money < 0)
        {
            money = 0;
        }
    }
    #endregion

    #region Tiggers detection
    // Si je rencontre un nouveau trigger alors je l'enregistre dans "currentTriggers"
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!currentTriggers.Contains(collision.gameObject))
        {
            currentTriggers.Add(collision.gameObject);
        }
    }

    //Si je sors d'un trigger alors je le supprime de "currentTriggers"
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (currentTriggers.Contains(collision.gameObject))
        {
            currentTriggers.Remove(collision.gameObject);
        }
    }
    #endregion

    // Detruire les obstacles avec le dash
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing)
        {
            if (collision.transform.tag == "ObstaclesDebris")
            {
                Destroy(collision.gameObject);
            }
            else if (collision.transform.tag == "Obstacles")
            {
                collision.gameObject.GetComponent<Interactable>().OnCollide(transform.tag);
            }
        }
    }

    // Active / desactive les message d'interaction ("'E' pour interagir")
    public void ActiveInteractMessage (bool value, string secondaryMessage = "")
    {
        string text = "'E' pour interagir";
        if(secondaryMessage != "") // Il y a la possibilité d'afficher un message secondaire qui sera affiché en rouge par exemple : "Coute 25 pieces"
        {
            text += " \n <color=red>" + secondaryMessage + "</color>";
        }
        interactMessage.GetComponentInChildren<TextMesh>().text = text;

        interactMessage.SetActive(value); // Active ou desactive le text
    }

    // Change le boomerang actuellement équipé
    public void ChangeCurrentBoomerang (BommerangProfile newProfile)
    {
        currentBoomerang = newProfile;
        bommerangSprite.GetComponentInChildren<SpriteRenderer>().sprite = currentBoomerang.sprite;
    }
}
