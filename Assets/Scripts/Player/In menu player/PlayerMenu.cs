using EZCameraShake;
using System.Collections;
using UnityEngine;

// Ce composant decrit le comportement du joueur mais uniquement dans le niveau MainMenu
// puisque la version In-game et la version MainMenu sont differentes
// aller voir PlayerBehaviour.cs pour la version In-game

public class PlayerMenu : MonoBehaviour
{
    // Public properties
    [Header("Stats")]
    public float m_speed;

    [Header("Dash")]
    public float dashSpeed;
    public float dashRate;
    [HideInInspector] public float startSpeed;
    private float nextDash;
    public float dashDuration;

    [Header("References")]
    public GameObject sprite;
    public GameObject boomerangPrefab;
    public GameObject bommerangSprite;

    public ParticleSystem smokeEffect;
    private ParticleSystem.EmissionModule emission;

    [Header("Sounds")]
    public AudioClip dashClip;

    // Private properties
    Rigidbody2D m_rb2D;
    Animator animator;
    AudioSource source;
    [HideInInspector] public bool isDashing = false;
    private float aimHoldTime;

    // Permissions
    [HideInInspector] public bool canMove = true, haveBoomerang = true;
    private bool tutoTextActive = true;

    private bool _canAttack = true;
    // Unactive look at mouse effect for boomerang if cant attack
    public bool canAttack
    {
        get => _canAttack;
        set
        {
            _canAttack = value;
            if (value == true)
            {
                GetComponentInChildren<LookAtMouse>().isActive = true;
            }
            else
            {
                GetComponentInChildren<LookAtMouse>().isActive = false;
            }
        }
    }

    #region Game logic
    void Awake()
    {
        m_rb2D = gameObject.GetComponent<Rigidbody2D>();
        animator = sprite.GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        startSpeed = m_speed;
        emission = smokeEffect.emission;
    }

    void Update()
    {
        // Rotate player to mouse position
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x > transform.position.x)
        {
            sprite.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (mousePos.x < transform.position.x)
        {
            sprite.transform.eulerAngles = new Vector3(0, 180, 0);
        }

        // Attack
        bommerangSprite.SetActive(haveBoomerang);
        if (canAttack)
        {
            // On start aim
            if (Input.GetMouseButtonDown(0))
            {
                if (haveBoomerang)
                {
                    aimHoldTime = 0f;
                }
            }
            // On aiming
            else if (Input.GetMouseButton(0))
            {
                if (haveBoomerang)
                {
                    // Get hold duration
                    aimHoldTime += Time.deltaTime;
                    // Zoom in
                    Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 3.5f, Time.deltaTime);
                    // Prevent player moving
                    canMove = false;
                }
            }
            // On release aim button
            else if (Input.GetMouseButtonUp(0))
            {
                if (haveBoomerang)
                {
                    Attack();
                    canMove = true;
                }
            }
            // Not pressing key
            else
            {
                // Zoom out
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 5, 5 * Time.deltaTime);
            }
        }

        // Dash
        if (Time.time > nextDash)
        {
            // Dash on press left mouse
            if (Input.GetMouseButtonDown(1) && canMove)
            {
                StartCoroutine(Dash(dashDuration));
            }
        }

        // Update smoke effect rotation (to follow move direction)
        float smokeEffectAngle = Vector2.SignedAngle(Vector2.right, new Vector2(m_rb2D.velocity.x, -m_rb2D.velocity.y)) + 180f;
        smokeEffect.transform.eulerAngles = new Vector3(smokeEffectAngle, 90f, 0);
        // Update emission rate on smoke effect (if player dont move there shouldn't be smoke)
        emission.rateOverTime = m_rb2D.velocity.magnitude != 0 ? 20f : 0f;

        // Show cursor on press escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = !Cursor.visible;
        }

        // Hide move inputs tuto text
        if (tutoTextActive)
        {
            if (Vector2.Distance(Vector2.zero, transform.position) > 2)
            {
                GameObject.FindGameObjectWithTag("ZQSDInformation").SetActive(false);
                tutoTextActive = false;
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 inputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        // Update 'isMoving' value of the animator
        animator.SetBool("isMoving", (inputs != Vector2.zero && m_rb2D.velocity != Vector2.zero && canMove));

        // if player is not allowed to move stop him
        if (!canMove)
        {
            m_rb2D.velocity = Vector2.zero;
            return;
        }

        // Move player
        if (!isDashing)
        {
            m_rb2D.velocity = inputs * m_speed;
        }
    }
    #endregion

    private void Attack()
    {
        // Create boomerang
        haveBoomerang = false;
        Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

        GameObject instance = Instantiate(boomerangPrefab, transform.position, Quaternion.identity);
        instance.GetComponent<BoomerangMenu>().Launch(direction, aimHoldTime);
    }

    private IEnumerator Dash(float duration)
    {
        float startTime = Time.time;
        Vector2 dashDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

        isDashing = true;

        // Sound
        source.PlayOneShot(dashClip);

        // Set realod dash UI to empty
        animator.SetBool("isDashing", true);

        // Camera shake
        CameraShaker.Instance.ShakeOnce(10f, 3f, 0.1f, 0.1f);

        // Player dash animation
        animator.SetBool("isDashing", true);

        // Dash physic
        while ((Time.time - startTime) < duration)
        {
            m_rb2D.velocity = dashDirection * dashSpeed;
            yield return null;
        }

        animator.SetBool("isDashing", false);

        isDashing = false;
        m_speed = startSpeed;

        // Reset nextDash (to wait before next dash)
        nextDash = Time.time + dashRate;
    }

}
