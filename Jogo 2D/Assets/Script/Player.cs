using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // --- MOVIMENTO DO PLAYER ------------------------
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // --- VIDA DO PLAYER ------------------------
    public int vidaMax = 5;
    public int vida;
    public Sprite emptyHeart;
    public Sprite fullHeart;
    public Image[] hearts;

    private Rigidbody2D rb;
    private bool isGrounded;

    // --- ANIMAÇÃO ------------------------
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private string currentAnimation = "";

    // --- ESPADA / ATAQUE ------------------------
    public bool temEspada = false; // Booleano que controla se tem a espada
    public float timeBtwAttack;
    public float startTimeBtwAttack;
    public Transform attackPos;
    public float attackrange;
    public LayerMask whatIsEnemies;
    public int damage;

    // --- DOUBLE JUMP ------------------------
    public int extraJumpValue = 1;
    private int extraJump;

    // --- Morte do Player ----
    public GameOver gameOverManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        vida = vidaMax;
        extraJump = extraJumpValue;
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // --- SISTEMA DE ATAQUE (Só ataca se tiver a espada) ---
        if (temEspada)
        {
            if (timeBtwAttack <= 0)
            {
                if (Input.GetKey(KeyCode.K))
                {
                    timeBtwAttack = startTimeBtwAttack;
                    Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackrange, whatIsEnemies);
                    for (int i = 0; i < enemiesToDamage.Length; i++)
                    {
                        enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);
                    }
                }
            }
            else
            {
                timeBtwAttack -= Time.deltaTime;
            }
        }

        // Atualizar corações da UI
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < vida) hearts[i].sprite = fullHeart;
            else hearts[i].sprite = emptyHeart;

            if (i < vidaMax) hearts[i].enabled = true;
            else hearts[i].enabled = false;
        }

        SetAnimation(moveInput);
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // ----------------------- ANIMAÇÃO -------------------------------

    private void SetAnimation(float moveInput)
    {
        if (moveInput > 0.1f)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveInput < -0.1f)
        {
            spriteRenderer.flipX = false;
        }

        if (isGrounded)
        {
            if (Mathf.Abs(moveInput) < 0.1f && Mathf.Abs(rb.linearVelocity.x) < 0.1f)
            {
                // Muda o nome da animação idle dependendo de ter ou não a espada
                string idleAnim = temEspada ? "playerIdleSword" : "Walk"; // Ajusta "playerIdleSword" para o nome real da tua animação com espada
                ChangeAnimationState(idleAnim);
            }
            else
            {
                // Muda o nome da animação de andar/correr dependendo de ter ou não a espada
                string runAnim = temEspada ? "playerRunSword" : "playerRun"; // Ajusta para o nome real
                ChangeAnimationState(runAnim);
            }
        }
        else
        {
            if (rb.linearVelocity.y > 0)
            {
                string jumpAnim = temEspada ? "playerJumpSword" : "playerJump"; // Ajusta para o nome real
                ChangeAnimationState(jumpAnim);
            }
            else
            {
                string fallAnim = temEspada ? "playerFallSword" : "playerFall"; // Ajusta para o nome real
                ChangeAnimationState(fallAnim);
            }
        }
    }

    private void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimation == newAnimation) return;

        animator.Play(newAnimation);
        currentAnimation = newAnimation;
    }

    // ----------------------- DANO -------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            vida -= 1;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(BlinkRed());

            if (vida <= 0)
            {
                Die();
            }
        }
    }

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        StopAllCoroutines();
        spriteRenderer.color = Color.white;
        rb.linearVelocity = Vector2.zero;

        gameOverManager.MostrarMorte();

        enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPos.position, attackrange);
        }
    }
}