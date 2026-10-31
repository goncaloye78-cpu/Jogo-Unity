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

    // --- DOUBLE JUMP ------------------------
    public int extraJumpValue = 1;
    private int extraJump;

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

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocityY);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

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

    // ----------------------- ANIMAÇÂO -------------------------------

    private void SetAnimation(float moveInput)
    {
        // MODIFICADO: Símbolos do flip invertidos para corrigir o moonwalk!
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
            // Se o input e a velocidade física forem quase nulos, força o Idle
            if (Mathf.Abs(moveInput) < 0.1f && Mathf.Abs(rb.linearVelocity.x) < 0.1f)
            {
                //Não mudar para o playerIDLE, era isso q tava a bugar pq o nome 'Walk' fica guardado no sistema e se mudares o nome muda s visualmente
                ChangeAnimationState("Walk");
            }
            else
            {
                ChangeAnimationState("playerRun");
            }
        }
        else
        {
            if (rb.linearVelocity.y > 0)
            {
                ChangeAnimationState("playerJump");
            }
            else
            {
                ChangeAnimationState("playerFall");
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
