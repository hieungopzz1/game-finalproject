using System.Collections;
using UnityEngine;

public class RangedEnemy : EnemyBase
{

    [Header("Entrance")]
    public Vector2 arenaPosition;
    public float enterSpeed = 4f;
    public float introDelay = 1f;

    private bool hasEntered = false;
    private bool introDone = false;
    private float introTimer = 0f;

    [Header("Intro Transform Effect")]
    public float introScaleMultiplier = 1.3f;
    public Color introTargetColor = Color.red;
    public SpriteRenderer spriteRenderer;

    private Vector3 initialScale;
    private Color initialColor;

    [Header("Hover")]
    public float bobAmplitude = 0.5f;
    public float bobFrequency = 1f;

    private float baseY;
    private float bobTime;
    private bool canBob = false;

    [Header("Normal Shoot")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 8f;
    public float minNormalDamage = 3f;
    public float maxNormalDamage = 7f;

    [Header("Cooldown theo phase")]
    [Tooltip("Phase 1: HP > 70%")]
    public float phase1NormalCooldown = 0.9f;

    [Tooltip("Phase 2: 70% >= HP > 30%")]
    public float phase2NormalCooldown = 0.8f;
    public float phase2SpreadCooldown = 3f;

    [Tooltip("Phase 3: HP <= 30%")]
    public float phase3NormalCooldown = 0.6f;
    public float phase3SpreadCooldown = 1.8f;

    [Header("Spread Skill")]
    public bool useSpreadSkill = true;
    public int spreadBulletCount = 7;
    public float spreadAngle = 60f;
    public float minSpreadDamage = 6f;
    public float maxSpreadDamage = 10f;
    public GameObject bulletSkillPrefab;


    private float nextNormalTime;
    private float nextSpreadTime;

    private bool hasFiredFirstShot = false;

    [Header("Shield Skill (NEW)")]
    // --- THAY ĐỔI 2: Skill Khiên ---
    public GameObject shieldVisual;    // Kéo cái ShieldVisual vào đây
    public float shieldDuration = 10f; // Tồn tại 10 giây
    public float shieldCooldown = 15f; // Hồi chiêu 15 giây
    private float nextShieldTime;      // Thời điểm được bật khiên tiếp theo
    private bool isShieldActive = false;


    [Header("Dash Charge Skill")]
    public float dashSpeed = 20f;      // Tốc độ lướt (nhanh gấp 5-10 lần bình thường)
    public float dashDuration = 0.5f;  // Thời gian lướt (ngắn thôi)
    public float dashCooldown = 12f;   // Hồi chiêu
    public float dashWarningTime = 0.8f; // Thời gian cảnh báo trước khi lướt
    public float dashDamage = 10f;     // Dame khi húc trúng (phải đau!)
    public float dashDistance = 10f;

    private float nextDashTime;
    private bool isDashing = false;    // Cờ kiểm tra đang lướt

    [Header("Audio")]
    public AudioClip bossDied;


    protected override void Awake()
    {
        startInvulnerable = true;
        base.Awake();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        initialScale = transform.localScale;
        initialColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        hasEntered = false;
        introDone = false;
        canBob = false;

        nextNormalTime = Mathf.Infinity;
        nextSpreadTime = 0f;
    }

    // Trong RangedEnemy.cs

    protected override void Start() // Thêm Start
    {

        // ... giữ nguyên các logic Start cũ nếu có (như base.Start()) ...

        // GỌI UI: Tìm thằng BossHealthUI và tự gán mình vào đó
        if (BossHealthBar.instance != null)
        {
            BossHealthBar.instance.SetBoss(this);
        }
        else
        {
            Debug.LogWarning("Chưa có BossHealthUI trong Scene!");
        }
        base.Start();
        if (shieldVisual != null) shieldVisual.SetActive(false);
        nextShieldTime = Time.time + 13f;
    }
    protected override void Update()
    {
        if (isDead) return;

        if (!hasEntered)
        {
            HandleEntrance();
            return;
        }

        if (!introDone)
        {
            HandleIntroDelay();
            return;
        }

        if (canBob)
        {
            HandleBobbing();
        }

        base.Update();
    }

    private void HandleEntrance()
    {
        Vector3 targetPos = new Vector3(arenaPosition.x, arenaPosition.y, transform.position.z);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            enterSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) <= 0.05f)
        {
            hasEntered = true;
            baseY = transform.position.y;
            bobTime = 0f;
            introTimer = 0f;

            transform.localScale = initialScale;
            if (spriteRenderer != null)
                spriteRenderer.color = initialColor;
        }
    }

    private void HandleIntroDelay()
    {
        introTimer += Time.deltaTime;
        float t = Mathf.Clamp01(introTimer / introDelay);

        Vector3 targetScale = initialScale * introScaleMultiplier;
        transform.localScale = Vector3.Lerp(initialScale, targetScale, t);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(initialColor, introTargetColor, t);
        }

        if (introTimer >= introDelay)
        {
            introDone = true;
            canBob = true;

            nextNormalTime = Time.time + 0.5f;
        }
    }

    private void HandleBobbing()
    {
        bobTime += Time.deltaTime * bobFrequency;
        float offset = Mathf.Sin(bobTime * Mathf.PI * 2f) * bobAmplitude;

        Vector3 pos = transform.position;
        pos.y = baseY + offset;
        transform.position = pos;
    }


    protected override void HandleBehaviour()
    {
        if (target == null) return;
        if (isDashing) return; // Không làm gì khác khi đang lướt
        FaceTarget();

        HandleShieldSkill();

        HandleDashSkill();

        if (!IsTargetInRange(detectionRange)) return;

        float hpPercent = currentHealth / maxHealth;

        if (hpPercent <= 0.3f)
        {
            HandleRandomSingleSpread(phase3NormalCooldown, phase3SpreadCooldown);
            return;
        }

        if (hpPercent <= 0.7f)
        {
            HandleRandomSingleSpread(phase2NormalCooldown, phase2SpreadCooldown);
            return;
        }

        HandlePureNormal(phase1NormalCooldown);
    }


    private void HandleShieldSkill()
    {
        // Nếu đang bật khiên rồi thì thôi
        if (isShieldActive) return;

        // Nếu đủ thời gian hồi chiêu -> Bật khiên
        if (Time.time >= nextShieldTime)
        {
            StartCoroutine(ActivateShield());
        }
    }

    private IEnumerator ActivateShield()
    {
        isShieldActive = true;

        // 1. Bật Bất Tử
        //SetInvulnerable(true);

        // 2. Hiện hình ảnh cái khiên
        if (shieldVisual != null) shieldVisual.SetActive(true);

        Debug.Log("BOSS: Bật khiên năng lượng!");

        // 3. Chờ 10 giây
        yield return new WaitForSeconds(shieldDuration);

        // 4. Tắt khiên
        isShieldActive = false;
        //SetInvulnerable(false); // Tắt bất tử, lại đánh được
        if (shieldVisual != null) shieldVisual.SetActive(false);

        // 5. Đặt thời gian hồi chiêu tiếp theo
        nextShieldTime = Time.time + shieldCooldown;
        Debug.Log("BOSS: Hết khiên, bắt đầu hồi chiêu.");
    }

    public override void TakeDamage(float amount)
    {
        // Nếu đang bật khiên
        if (isShieldActive)
        {
            // Nếu damage to hơn 1 thì ép xuống 1
            if (amount > 1)
            {
                amount = 1;
            }
            // (Tuỳ chọn) Hiệu ứng âm thanh "Keng" vào khiên ở đây
        }

        // Gọi hàm gốc để trừ máu (với lượng damage đã bị giảm)
        base.TakeDamage(amount);
    }

    private void HandlePureNormal(float normalCd)
    {
        if (Time.time >= nextNormalTime)
        {
            FireSingle();
            nextNormalTime = Time.time + normalCd;
        }
    }

    private void HandleRandomSingleSpread(float normalCd, float spreadCd)
    {
        bool canNormal = Time.time >= nextNormalTime;
        bool canSpread = useSpreadSkill && Time.time >= nextSpreadTime;

        if (!canNormal && !canSpread) return;

        if (canNormal && !canSpread)
        {
            FireSingle();
            nextNormalTime = Time.time + normalCd;
            return;
        }

        if (!canNormal && canSpread)
        {
            FireSpread();
            nextSpreadTime = Time.time + spreadCd;
            return;
        }

        float r = Random.value;
        if (r < 0.6f)
        {
            FireSingle();
            nextNormalTime = Time.time + normalCd;
        }
        else
        {
            FireSpread();
            nextSpreadTime = Time.time + spreadCd;
        }
    }

    private void FaceTarget()
    {
        Vector3 dir = target.position - transform.position;

        if (dir.x != 0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(dir.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void FireSingle()
    {
        if (bulletPrefab == null || firePoint == null || target == null) return;

        Vector2 dir = (target.position - firePoint.position).normalized;

        Quaternion rot = Quaternion.FromToRotation(Vector3.right, dir);

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, rot);

        EnemyProjectile proj = bulletObj.GetComponent<EnemyProjectile>();
        if (proj != null)
        {
            float randDamage = Random.Range(minNormalDamage, maxNormalDamage);
            proj.Init(dir, bulletSpeed, randDamage);
        }
        else
        {
            Rigidbody2D rb = bulletObj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * bulletSpeed;
            }
        }
        if ( !hasFiredFirstShot)
        {
            hasFiredFirstShot = true;
            SetInvulnerable(false);
        }
    }


    private void FireSpread()
    {
        if (bulletPrefab == null || firePoint == null || target == null) return;

        Vector2 baseDir = (target.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        if (spreadBulletCount <= 1)
        {
            FireSingle();
            return;
        }

        float step = spreadAngle / (spreadBulletCount - 1);
        float startOffset = -spreadAngle / 2f;

        for (int i = 0; i < spreadBulletCount; i++)
        {
            float angle = baseAngle + startOffset + step * i;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);

            GameObject bulletObj = Instantiate(bulletSkillPrefab, firePoint.position, rot);

            EnemyProjectile proj = bulletObj.GetComponent<EnemyProjectile>();
            if (proj != null)
            {
                Vector2 dir = rot * Vector2.right;
                float randDamage = Random.Range(minNormalDamage, maxNormalDamage);
                proj.Init(dir, bulletSpeed, randDamage);
            }
            else
            {
                Rigidbody2D rb = bulletObj.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 dir = rot * Vector2.right;
                    rb.linearVelocity = dir * bulletSpeed;
                }
            }
        }
    }

    protected override void Die()
    {
        StopAllCoroutines();
        isDead = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false;
        }

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1.5f;
            rb.linearVelocity = new Vector2(0, 10f);
            rb.angularVelocity = Random.Range(-180f, 180f);
        }

        WaveUIController.instance.ShowWave("BOSS DEFEATED!");
        Invoke(nameof(TriggerWin), 3f);
        Destroy(gameObject, 3f);
        if (AudioManager.instance != null && bossDied != null)
        {
            AudioManager.instance.PlaySFX(bossDied);
        }
    }

    void TriggerWin()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.Victory();
        }
    }

    private void HandleDashSkill()
    {
        // Chỉ lướt khi còn dưới 70% máu (Phase 2 trở đi)
        if (currentHealth / maxHealth > 0.7f) return;

        // Nếu đang lướt hoặc chưa hồi chiêu thì thôi
        if (isDashing || Time.time < nextDashTime) return;

        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;

        Debug.Log("BOSS: Đang gồng, chuẩn bị húc!");

        Color originalColor = spriteRenderer.color;
        if (spriteRenderer != null) spriteRenderer.color = Color.green;

        Vector3 dashDir = (target.position - transform.position).normalized;
        yield return new WaitForSeconds(1f);
        Debug.Log("BOSS: HÚC!");

        bool hasDealtDamage = false;
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            transform.position += dashDir * dashSpeed * Time.deltaTime;

            if (!hasDealtDamage)
            {
                Collider2D hit = Physics2D.OverlapCircle(transform.position, 1.5f);

                if (hit != null && hit.CompareTag("Player"))
                {
                    PlayerHealth playerHp = hit.GetComponent<PlayerHealth>();
                    if (playerHp != null)
                    {
                        Debug.Log("BOSS: Húc trúng Player!");
                        playerHp.TakeDamage(dashDamage);
                        hasDealtDamage = true;
                    }
                }
            }
            yield return null;
        }

        if (spriteRenderer != null) spriteRenderer.color = originalColor;
        yield return new WaitForSeconds(1f);

        isDashing = false;
        nextDashTime = Time.time + dashCooldown;
    }

}
