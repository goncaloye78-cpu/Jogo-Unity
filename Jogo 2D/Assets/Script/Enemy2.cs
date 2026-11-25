using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy2 : MonoBehaviour
{
    public float speed = 2f;
    public int moveDirection = 1; // 1 = direita, -1 = esquerda

    [Header("Sprites da Folha (8 frames)")]
    public Sprite idleSprite;          // L1 Esq - Parado / base
    public Sprite idleAltSprite;       // L1 Dir - Ligeiro ajuste/achatamento
    public Sprite prepSprite;          // L2 Esq - Preparação / bem achatado
    public Sprite liftoffSprite;       // L2 Dir - A levantar / início de salto
    public Sprite jumpAscendSprite;    // L3 Esq - A subir
    public Sprite jumpApexSprite;      // L3 Dir - Mais esticado / alto no salto
    public Sprite jumpDescendSprite;   // L4 Esq - A descer
    public Sprite recoverySprite;      // L4 Dir - Recuperação / aterragem

    [Header("Tempos do Salto")]
    public float jumpHeight = 1f;
    public float prepDuration = 0.24f;
    public float airDuration = 0.45f;
    public float recoveryDuration = 0.20f;
    public float idleTime = 1f;

    [Header("Dano e Knockback")]
    public Sprite damageSprite;
    public float damageSpriteDuration = 0.25f;
    public float knockbackForce = 4f;
    public float knockbackDuration = 0.2f;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public int health = 3;

    private Sprite normalSprite;
    private Sprite currentPhaseSprite;
    private bool isKnockedBack;
    private Coroutine damageCoroutine;
    private Coroutine knockbackCoroutine;

    private void Start()
    {
        moveDirection = moveDirection >= 0 ? 1 : -1;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            animator.enabled = false;
        }

        if (spriteRenderer != null)
        {
            normalSprite = spriteRenderer.sprite;
            currentPhaseSprite = idleSprite != null ? idleSprite : normalSprite;
            spriteRenderer.sprite = currentPhaseSprite;
        }

        StartCoroutine(JumpPatrol());
    }

    private void Update()
    {
        if (isKnockedBack || spriteRenderer == null) return;

        if (moveDirection > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveDirection < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private Sprite ResolveSprite(params Sprite[] candidates)
    {
        foreach (Sprite s in candidates)
        {
            if (s != null) return s;
        }
        return normalSprite;
    }

    private void SetPhaseSprite(Sprite preferredSprite)
    {
        currentPhaseSprite = preferredSprite != null ? preferredSprite : normalSprite;

        bool showingDamage = damageCoroutine != null;
        if (spriteRenderer != null && !showingDamage && currentPhaseSprite != null)
        {
            spriteRenderer.sprite = currentPhaseSprite;
        }
    }

    private IEnumerator JumpPatrol()
    {
        while (true)
        {
            while (isKnockedBack)
            {
                yield return null;
            }

            // 1. Preparação antes de saltar (comprime e começa a levantar)
            float prepTimer = 0f;
            while (prepTimer < prepDuration && !isKnockedBack)
            {
                prepTimer += Time.deltaTime;
                float p = Mathf.Clamp01(prepTimer / Mathf.Max(0.01f, prepDuration));

                Sprite stepSprite = p < 0.65f
                    ? ResolveSprite(prepSprite, idleAltSprite, idleSprite)
                    : ResolveSprite(liftoffSprite, jumpAscendSprite, prepSprite);

                SetPhaseSprite(stepSprite);
                yield return null;
            }

            if (isKnockedBack) continue;

            // 2. No ar (a subir -> ponto alto/esticado -> a descer)
            Vector3 startPos = transform.position;
            float airTimer = 0f;

            while (airTimer < airDuration && !isKnockedBack)
            {
                airTimer += Time.deltaTime;
                float t = Mathf.Clamp01(airTimer / Mathf.Max(0.01f, airDuration));

                Sprite airSprite;
                if (t < 0.35f)
                {
                    airSprite = ResolveSprite(jumpAscendSprite, liftoffSprite);
                }
                else if (t < 0.65f)
                {
                    airSprite = ResolveSprite(jumpApexSprite, jumpAscendSprite, jumpDescendSprite);
                }
                else
                {
                    airSprite = ResolveSprite(jumpDescendSprite, recoverySprite);
                }
                SetPhaseSprite(airSprite);

                float forwardDist = speed * airDuration * t * moveDirection;
                float arc = 4f * jumpHeight * t * (1f - t);

                transform.position = new Vector3(startPos.x + forwardDist, startPos.y + arc, transform.position.z);
                yield return null;
            }

            if (isKnockedBack) continue;

            // 3. Recuperação após cair
            SetPhaseSprite(ResolveSprite(recoverySprite, idleAltSprite, idleSprite));
            float recoveryTimer = 0f;
            while (recoveryTimer < recoveryDuration && !isKnockedBack)
            {
                recoveryTimer += Time.deltaTime;
                yield return null;
            }

            if (isKnockedBack) continue;

            // 4. Parado entre saltos (alterna ligeiramente se tiveres Idle Alt Sprite)
            float idleTimer = 0f;
            while (idleTimer < idleTime)
            {
                if (!isKnockedBack)
                {
                    idleTimer += Time.deltaTime;
                    bool useAlt = idleAltSprite != null && idleTimer > idleTime * 0.75f;
                    SetPhaseSprite(useAlt ? idleAltSprite : ResolveSprite(idleSprite));
                }
                yield return null;
            }
        }
    }

    private void FlipDirection()
    {
        moveDirection *= -1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                FlipDirection();
                break;
            }
        }
    }

    public void TakeDamage(int damage, Vector3? attackerPos = null)
    {
        health -= damage;

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }
        damageCoroutine = StartCoroutine(DamageFeedback());

        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }
        knockbackCoroutine = StartCoroutine(Knockback(attackerPos));

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFeedback()
    {
        if (spriteRenderer == null) yield break;

        if (damageSprite != null)
        {
            spriteRenderer.sprite = damageSprite;
        }
        else
        {
            spriteRenderer.color = Color.yellow;
        }

        yield return new WaitForSeconds(damageSpriteDuration);

        if (currentPhaseSprite != null)
        {
            spriteRenderer.sprite = currentPhaseSprite;
        }
        spriteRenderer.color = Color.white;
        damageCoroutine = null;
    }

    private IEnumerator Knockback(Vector3? attackerPos)
    {
        isKnockedBack = true;

        float dirX;
        if (attackerPos.HasValue)
        {
            dirX = transform.position.x >= attackerPos.Value.x ? 1f : -1f;
        }
        else
        {
            dirX = spriteRenderer != null && spriteRenderer.flipX ? 1f : -1f;
        }

        Vector3 knockDir = new Vector3(dirX, 0.25f, 0f).normalized;
        float timer = 0f;

        while (timer < knockbackDuration)
        {
            transform.position += knockDir * knockbackForce * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        isKnockedBack = false;
        knockbackCoroutine = null;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}