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

    // public Image healthImagem; ANTIGO SISTEMA DE VIDA

    private Rigidbody2D rb;
    private bool isGrounded;

    // --- ANIMAÇÃO ------------------------
    private Animator animator;
    private SpriteRenderer SpriteRenderer;

    // --- DOUBLE JUMP ------------------------
    public int extraJumpValue = 1;
    private int extraJump;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();

        vida = vidaMax;
        extraJump = extraJumpValue;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2 (moveInput * moveSpeed, rb.linearVelocityY);
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        /*  double jump desbloqueavel - após alguma condição mudamos para este « if »
         *  
         if (isGrounded)
         {
             extraJump = extraJumpValue;
         }
         if (Input.GetKeyDown(KeyCode.Space))
         {
             if (isGrounded)
             {
                 rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
             }
             else if(extraJump >0){
                 rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                 extraJump--;
             }
         }
        */

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < vida)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < vidaMax)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }

        SetAnimation(moveInput);

        // healthImagem.fillAmount = health / 5f;  ANTIGO SISTEMA DE VIDA
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // ----------------------- ANIMAÇÂO -------------------------------

    private void SetAnimation(float moveInput)
    {
        if ((isGrounded))
        {
            if (moveInput == 0) 
            {
                animator.Play("Walk");
            }
            else
            {
                animator.Play("playerRun");
            }
        }
        else
        {
            if(rb.linearVelocityY > 0)
            {
                animator.Play("playerJump");
            }
            else
            {
                animator.Play("playerFall");
            }
        }
    }

    // ----------------------- DANO -------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Damage")
        {
            vida -= 1;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(BlinkRed());

            if(vida <= 0)
            {
                Die();
            }
        }
    }

    // ---------------- Qunado Toma Dano ----------------------
    private IEnumerator BlinkRed()
    {
        SpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        SpriteRenderer.color = Color.white;
    }

    private void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

}
