using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int health = 5;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public Image healthImagem;

    private Rigidbody2D rb;
    private bool isGrounded;

    private Animator animator;
    private SpriteRenderer SpriteRenderer;

    public int extraJumpValue = 1;
    private int extraJump;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();

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

        SetAnimation(moveInput);

        healthImagem.fillAmount = health / 5f;
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
            health -= 1;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(BlinkRed());

            if(health <= 0)
            {
                Die();
            }
        }
    }

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
