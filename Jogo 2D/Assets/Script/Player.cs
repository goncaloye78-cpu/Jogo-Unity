using System.Collections;
using System.Reflection;
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
    public bool temEspada = false;
    public float timeBtwAttack;
    public float startTimeBtwAttack;
    public Transform attackPos;
    public float attackrange;
    public LayerMask whatIsEnemies;
    public int damage = 1;

    private float attackPosX;

    // --- DOUBLE JUMP ------------------------
    public int extraJumpValue = 1;
    private int extraJump;
    public bool temDoubleJump = false;

    private float groundGraceTime = 0.15f;  //pro efeito do coyote time (aquela cena dos milissegundos para aterrizar no chao)
    private float groundTimer;
    private bool coyoteJumpUsed;

    // --- Morte do Player ----
    // public GameOver gameOverManager;

    private GameObject orbPerto;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DoubleJump"))
        {
            orbPerto = collision.gameObject;
            Debug.Log("Carrega em E para desbloquear o Double Jump!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == orbPerto)
        {
            orbPerto = null;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        vida = vidaMax;
        extraJump = extraJumpValue;

        if (attackPos != null)
        {
            attackPosX = Mathf.Abs(attackPos.localPosition.x);
        }
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (orbPerto != null && Input.GetKeyDown(KeyCode.E))
        {
            temDoubleJump = true;
            extraJump = extraJumpValue;

            Destroy(orbPerto);
            orbPerto = null;

            Debug.Log("Double Jump desbloqueado!");
        }

        /*
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        */

        if (isGrounded)
        {
            groundTimer = groundGraceTime;
            extraJump = extraJumpValue;
            coyoteJumpUsed = false;
        }
        else
        {
            groundTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded || (groundTimer > 0 && !coyoteJumpUsed))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

                if (!isGrounded)
                    coyoteJumpUsed = true;

                groundTimer = 0;
            }
            else if (temDoubleJump && extraJump > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                extraJump--;
            }
        }

        // --- SISTEMA DE ATAQUE (Só ataca se tiver a espada) ---
        if (temEspada)
        {
            if (timeBtwAttack <= 0)
            {
                if (Input.GetKey(KeyCode.K) && attackPos != null)
                {
                    timeBtwAttack = startTimeBtwAttack;
                    Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackrange, whatIsEnemies);
                    for (int i = 0; i < enemiesToDamage.Length; i++)
                    {
                        if (enemiesToDamage[i].transform == transform || enemiesToDamage[i].transform.IsChildOf(transform))
                            continue;

                        ChamarTakeDamageNaHierarquia(enemiesToDamage[i].transform, damage, transform.position);
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

    private void ChamarTakeDamageNaHierarquia(Transform alvo, int dano, Vector3 origemAtaque)
    {
        Transform atual = alvo;

        while (atual != null)
        {
            MonoBehaviour[] scripts = atual.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour script in scripts)
            {
                if (script == null) continue;

                var metodoComPos = script.GetType().GetMethod(
                    "TakeDamage",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new System.Type[] { typeof(int), typeof(Vector3?) },
                    null
                );

                if (metodoComPos != null)
                {
                    metodoComPos.Invoke(script, new object[] { dano, (Vector3?)origemAtaque });
                    return;
                }

                var metodoSimples = script.GetType().GetMethod(
                    "TakeDamage",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new System.Type[] { typeof(int) },
                    null
                );

                if (metodoSimples != null)
                {
                    metodoSimples.Invoke(script, new object[] { dano });
                    return;
                }
            }

            atual = atual.parent;
        }
    }
    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // ----------------------- ANIMAÇÃO E DIREÇÃO -------------------------------

    private void SetAnimation(float moveInput)
    {
        if (moveInput > 0.1f)
        {
            spriteRenderer.flipX = true;
            if (attackPos != null)
            {
                attackPos.localPosition = new Vector3(attackPosX, attackPos.localPosition.y, attackPos.localPosition.z);
            }
        }
        else if (moveInput < -0.1f)
        {
            spriteRenderer.flipX = false;
            if (attackPos != null)
            {
                attackPos.localPosition = new Vector3(-attackPosX, attackPos.localPosition.y, attackPos.localPosition.z);
            }
        }

        if (isGrounded)
        {
            if (Mathf.Abs(moveInput) < 0.1f && Mathf.Abs(rb.linearVelocity.x) < 0.1f)
            {
                string idleAnim = temEspada ? "playerIdleSword" : "Walk";
                ChangeAnimationState(idleAnim);
            }
            else
            {
                string runAnim = temEspada ? "playerRunSword" : "playerRun";
                ChangeAnimationState(runAnim);
            }
        }
        else
        {
            if (rb.linearVelocity.y > 0)
            {
                string jumpAnim = temEspada ? "playerJumpSword" : "playerJump";
                ChangeAnimationState(jumpAnim);
            }
            else
            {
                string fallAnim = temEspada ? "playerFallSword" : "playerFall";
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

        GameOver gameOverAtual = FindAnyObjectByType<GameOver>();

        if (gameOverAtual != null)
        {
            gameOverAtual.MostrarMorte();
        }
        else
        {
            Debug.LogError("Não existe GameOver nesta cena!");
        }

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

