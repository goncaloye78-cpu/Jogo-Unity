using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy2 : MonoBehaviour
{
    public float speed = 2f;
    public int moveDirection = 1; // 1 = direita, -1 = esquerda

    [Header("Deteção de Parede e Chão")]
    public LayerMask wallLayer = ~0;
    public LayerMask groundLayer = ~0;
    public float wallCheckDistance = 0.1f;

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
    public float knockbackUpForce = 3.5f;
    public float knockbackDuration = 0.25f;
    public float knockbackGravity = 22f;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Collider2D enemyCollider;
    public int health = 3;

    private Sprite normalSprite;
    private Sprite currentPhaseSprite;
    private bool isKnockedBack;
    private float lastFlipTime = -10f;
    private Coroutine damageCoroutine;
    private Coroutine knockbackCoroutine;

    private void Start()
    {
        moveDirection = moveDirection >= 0 ? 1 : -1;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        enemyCollider = GetComponentInChildren<Collider2D>();

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

    private bool CheckWallAhead()
    {
        if (enemyCollider == null) return false;

        Bounds bounds = enemyCollider.bounds;
        Vector2 boxSize = new Vector2(bounds.size.x * 0.9f, bounds.size.y * 0.7f);
        Vector2 dir = new Vector2(moveDirection, 0f);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(bounds.center, boxSize, 0f, dir, wallCheckDistance, wallLayer);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;
            if (hit.collider.isTrigger) continue;
            if (hit.collider.transform.IsChildOf(transform) || hit.collider.transform == transform) continue;
            if (hit.collider.CompareTag("Player")) continue;

            return true;
        }

        return false;
    }

    private bool IsTouchingGroundBelow()
    {
        if (enemyCollider == null) return false;

        Bounds bounds = enemyCollider.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 0.08f, groundLayer);

        if (hit.collider == null) return false;
        if (hit.collider.isTrigger) return false;
        if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform)) return false;

        return true;
    }

    private void TryFlipDirection()
    {
        if (Time.time - lastFlipTime < 0.15f) return;

        moveDirection *= -1;
        lastFlipTime = Time.time;
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

            // 2. No ar (movimento horizontal incremental para virar suavemente na parede)
            float baseY = transform.position.y;
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

                if (CheckWallAhead())
                {
                    TryFlipDirection();
                }

                float newX = transform.position.x + speed * moveDirection * Time.deltaTime;
                float arc = 4f * jumpHeight * t * (1f - t);

                transform.position = new Vector3(newX, baseY + arc, transform.position.z);
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

            // 4. Parado entre saltos
            float idleTimer = 0f;
            while (idleTimer < idleTime)
            {
                if (!isKnockedBack)
                {
                    idleTimer += Time.deltaTime;
                }
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

        float velX = dirX * knockbackForce;
        float velY = knockbackUpForce;
        float timer = 0f;

        while (timer < knockbackDuration || !IsTouchingGroundBelow())
        {
            velY -= knockbackGravity * Time.deltaTime;

            if (velY < 0f && IsTouchingGroundBelow())
            {
                velY = 0f;
                if (timer >= knockbackDuration)
                {
                    break;
                }
            }

            transform.position += new Vector3(velX, velY, 0f) * Time.deltaTime;
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