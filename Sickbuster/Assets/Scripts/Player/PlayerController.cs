using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


public class PlayerController : MonoBehaviour
{
    public GameObject GameOverUI;

    //Movement
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Camera cam;
    Vector2 mousePos;
    Vector2 movement;
    public float health = 100f;
    public float damage = 5;

    // Shooting Cooldwn
    public float shootingCooldown = 1f;
    private float lastShootTime = 0f;

    // Dashing
    public float dashDistance = 10f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 2f;
    private float lastDashTime = 0f;

    //HUD
    //Wave UI
    public TextMeshProUGUI waveText;
    private WaveSystem wave;

    //Health UI
    private int currentWaveCount;
    public Image healthBar;

    //Score UI
    public TextMeshProUGUI scoreText;

    // Level UI
    public TextMeshProUGUI levelText;
    private int level = 1;

    // DashIcon UI
    public Image iconDashImage;
    public Image lockedDashImage;
    public TextMeshProUGUI levelTwoText;

    // ShieldIconUI
    public Image iconShieldImage;
    public Image lockedShieldImage;
    public TextMeshProUGUI levelThreeText;

    //Shooting
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject bulletPrefabRSB;
    public float bulletForce = 20f;

    private int lvlWeapon = 1;

    private bool isDashing = false;
    public bool isInstakill = false;

    public GameObject shieldPrefab;
    private bool shieldActive = false;
    private bool shieldCooldownActive = false;
    public TrailRenderer tr;
    public GameObject nukePrefab;

    public Image instaImage;
    public Image healthImage;

    public GameObject poisonEffectPrefab;
    public GameObject explosionPrefab;
    public GameObject explosionCirclePrefab;

    private bool exploding = false;
    private bool explodingCooldownActive = false;
    public Image iconExplosionImage;
    public Image lockedExplosionImage;
    public TextMeshProUGUI levelFourText;
    private bool rsbShooting = false;
    private bool rsbCooldownActive = false;
    public Image iconRSBImage;
    public Image lockedRSBImage;
    public TextMeshProUGUI levelFiveText;
    public Transform leftFirePoint;
    public Transform rightFirePoint;
    private bool doubleShootingPrevention = false;

    //SFX Sounds
    public SFXController sfx;

    void Start()
    {
        wave = FindObjectOfType<WaveSystem>();
        instaImage.enabled = false;
        healthImage.enabled = false;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }
    void Update()
    {
        ActivateDash();
        CountWave();
        ShootBullet();
        LevelUp();
        GrayOutIcons();
        CheckWeaponLevel();
        ActivateShield();
        CheckLevelForWeapon();
        CheckForDeath();
        CheckCollisionsWithTag("DeathPuddleBacteria");
        CheckForExplosionAbility();
        CheckRSBShooting();

        if (Input.GetKeyDown(KeyCode.P))
        {
            sfx.PlaySFX("EasterEgg1");
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            sfx.PlaySFX("EasterEgg2");
        }
    }

    void MovePlayer()
    {
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void CheckForExplosionAbility()
    {
        if (level >= 4)
        {
            if (Input.GetKeyDown(KeyCode.Q) && !exploding && !explodingCooldownActive)
            {
                StartCoroutine(Explode());
            }

        }
    }
    void CheckRSBShooting()
    {
        if (level >= 5)
        {
            if (Input.GetButtonDown("Fire2") && !rsbShooting && !rsbCooldownActive)
            {
                StartCoroutine(RSB());
            }

        }
    }

    private IEnumerator RSB()
    {
        rsbShooting = true;
        iconRSBImage.color = new Color(1f, 1f, 1f, 0.3f);
        doubleShootingPrevention = true;
        for (int i = 0; i < 5; i++)
        {
            Transform currentFirePoint = i % 2 == 0 ? leftFirePoint : rightFirePoint;

            Quaternion additionalRotation = Quaternion.Euler(0, 0, +90);
            Quaternion playerRotation = transform.rotation;
            Quaternion bulletRotation = playerRotation * additionalRotation;

            sfx.PlaySFX("ShootingSound");
            GameObject bullet = Instantiate(bulletPrefabRSB, currentFirePoint.position, bulletRotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(currentFirePoint.up * bulletForce, ForceMode2D.Impulse);

            yield return new WaitForSeconds(0.1f);
        }
        doubleShootingPrevention = false;
        yield return new WaitForSeconds(1.5f);
        rsbShooting = false;
        iconRSBImage.color = new Color(1f, 1f, 1f, 1f);

    }

    void ExplosionAbility()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        StartCoroutine(Explode());
        sfx.PlaySFX("LaserSound");
    }

    private IEnumerator Explode()
    {
        exploding = true;
        iconExplosionImage.color = new Color(1f, 1f, 1f, 0.3f);
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        GameObject explosionParticle = Instantiate(explosionPrefab, mouseWorldPos, Quaternion.identity);
        yield return new WaitForSeconds(0.9f);
        GameObject explosionCircle = Instantiate(explosionCirclePrefab, mouseWorldPos, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        exploding = false;
        explodingCooldownActive = true;
        Destroy(explosionCircle.gameObject);
        Destroy(explosionParticle.gameObject);
        yield return new WaitForSeconds(15f);
        iconExplosionImage.color = new Color(1f, 1f, 1f, 1f);
        explodingCooldownActive = false;

    }

    private void CheckCollisionsWithTag(string tag)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag(tag))
            {
                sfx.PlaySFX("DamageSound");
                health -= 0.025f;
                health = Mathf.Clamp(health, 0, 100);
                healthBar.fillAmount = health / 100f;

            }
        }
    }

    void RotatePlayer()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    private IEnumerator DeactivateShield(float delay, GameObject shieldObject)
    {
        shieldActive = true;
        iconShieldImage.color = new Color(1f, 1f, 1f, 0.3f);
        yield return new WaitForSeconds(delay);
        shieldActive = false;
        Destroy(shieldObject.gameObject);
        shieldCooldownActive = true;
        yield return new WaitForSeconds(10f);
        iconShieldImage.color = new Color(1f, 1f, 1f, 1f);
        shieldCooldownActive = false;
    }

    void ActivateDash()
    {
        if (level >= 2)
        {
            if (Input.GetKeyUp(KeyCode.Space) && Time.time - lastDashTime >= dashCooldown)
            {
                sfx.PlaySFX("DashSound");
                DashDirection();
                lastDashTime = Time.time;
            }
            if (Time.time - lastDashTime >= dashCooldown)
            {
                iconDashImage.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                iconDashImage.color = new Color(1f, 1f, 1f, 0.3f);
            }
        }

    }

    void ActivateShield()
    {
        if (level >= 3)
        {
            if (Input.GetKeyDown(KeyCode.E) && !shieldActive && !shieldCooldownActive)
            {
                sfx.PlaySFX("ShieldSound");
                GameObject shieldObject = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
                shieldObject.transform.parent = transform;
                shieldObject.transform.localPosition = Vector3.zero;
                StartCoroutine(DeactivateShield(4f, shieldObject));
            }

        }


    }

    void GrayOutIcons()
    {
        lockedDashImage.enabled = true;
        lockedDashImage.color = new Color(1f, 1f, 1f, 0.6f);
        levelTwoText.text = "2";
        lockedShieldImage.enabled = true;
        lockedShieldImage.color = new Color(1f, 1f, 1f, 0.6f);
        levelThreeText.text = "3";
        lockedExplosionImage.enabled = true;
        lockedExplosionImage.color = new Color(1f, 1f, 1f, 0.6f);
        levelFourText.text = "4";
        lockedRSBImage.enabled = true;
        lockedRSBImage.color = new Color(1f, 1f, 1f, 0.6f);
        levelFiveText.text = "5";
        if (level == 2)
        {
            lockedDashImage.enabled = false;
            levelTwoText.text = "";
        }
        else if (level == 3)
        {
            lockedDashImage.enabled = false;
            levelTwoText.text = "";
            lockedShieldImage.enabled = false;
            levelThreeText.text = "";
        }
        else if (level == 4)
        {
            lockedDashImage.enabled = false;
            levelTwoText.text = "";
            lockedShieldImage.enabled = false;
            levelThreeText.text = "";
            lockedExplosionImage.enabled = false;
            levelFourText.text = "";

        }
        else if (level == 5)
        {
            lockedDashImage.enabled = false;
            levelTwoText.text = "";
            lockedShieldImage.enabled = false;
            levelThreeText.text = "";
            lockedExplosionImage.enabled = false;
            levelFourText.text = "";
            lockedRSBImage.enabled = false;
            levelFiveText.text = "";

        }
    }


    void CheckWeaponLevel()
    {
        if (lvlWeapon == 1)
        {
            return;
        }
        else if (lvlWeapon == 2)
        {
            damage = 10;
        }
        else
        {
            damage = 15;
        }
    }

    void CheckLevelForWeapon()
    {
        if (level == 1)
        {
            shootingCooldown = 1f;
        }
        else if (level == 2)
        {
            shootingCooldown = 0.5f;
        }
        else
        {
            shootingCooldown = 0.25f;
        }
    }

    void CountWave()
    {
        if (wave != null)
        {
            currentWaveCount = wave.currentWave;
            waveText.text = "Wave: " + currentWaveCount;
        }
    }

    void LevelUp()
    {
        if (GameManager.instance.GetScore() >= 250)
        {
            level = 2;
        }
        if (GameManager.instance.GetScore() >= 1500)
        {
            level = 3;
        }
        if (GameManager.instance.GetScore() >= 3000)
        {
            level = 4;
        }
        if (GameManager.instance.GetScore() >= 5000)
        {
            level = 5;
        }
        levelText.text = "Level: " + level;
    }

    void ShootBullet()
    {
        if (Input.GetButton("Fire1") && Time.time - lastShootTime >= shootingCooldown && !doubleShootingPrevention)
        {
            Quaternion playerRotation = transform.rotation;
            Quaternion additionalRotation = Quaternion.Euler(0, 0, +90);
            Quaternion bulletRotation = playerRotation * additionalRotation;
            sfx.PlaySFX("ShootingSound");
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
            lastShootTime = Time.time;
        }
    }

    void DashDirection()
    {
        Vector2 dashDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.A))
        {
            dashDirection += Vector2.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            dashDirection += Vector2.right;
        }
        if (Input.GetKey(KeyCode.W))
        {
            dashDirection += Vector2.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            dashDirection += Vector2.down;
        }

        // Check if any directional keys are pressed
        if (dashDirection != Vector2.zero)
        {
            Dash(dashDirection.normalized);  // Normalize to maintain consistent speed in all directions
        }
    }

    void Dash(Vector2 direction)
    {
        isDashing = true;
        StartCoroutine(PerformDash(direction));
    }

    IEnumerator PerformDash(Vector2 direction)
    {
        float elapsedTime = 0f;
        Vector2 originalPosition = transform.position;
        Vector2 dashTarget = originalPosition + direction * dashDistance;
        tr.emitting = true;
        while (elapsedTime < dashDuration)
        {
            transform.position = Vector2.Lerp(originalPosition, dashTarget, elapsedTime / dashDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
        tr.emitting = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (!shieldCooldownActive && shieldActive)
            {
                Destroy(collision.gameObject);
                return;
            }

            if (health - enemy.damage > 0)
            {
                sfx.PlaySFX("DamageSound");
                health -= enemy.damage;
                healthBar.fillAmount = health / 100f;
                Destroy(collision.gameObject);
                return;
            }
            Death(collision);
        }
        else if (collision.gameObject.tag == "VirusSpike")
        {
            if (health - 5f > 0)
            {
                sfx.PlaySFX("DamageSound");
                health -= 5f;
                healthBar.fillAmount = health / 100f;
                Destroy(collision.gameObject);
                return;
            }

            Death(collision);
        }
        else if (collision.gameObject.tag == "PowerUp")
        {
            Destroy(collision.gameObject);
            string powerup = GameManager.instance.GetPowerUp();
            switch (powerup)
            {
                case "WEAPON":
                    lvlWeapon++;
                    break;
                case "HEALTH":
                    OnHealthPowerUp();
                    break;
                case "NUKE":
                    OnNukePowerUp();
                    break;
                case "INSTAKILL":
                    OnInstakillPowerUp();
                    break;
            }
        }
        else if (collision.gameObject.tag == "Bullet" && isDashing)
        {
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "CancerDeath")
        {
            StartCoroutine(PoisonDamage(5f, 25f));
            Destroy(collision.gameObject);
            // EFFEKT FEHLT!
        }
    }

    private IEnumerator PoisonDamage(float duration, float damage)
    {
        float elapsedTime = 0f;
        GameObject poisonEffect = Instantiate(poisonEffectPrefab, transform.position, Quaternion.identity);
        poisonEffect.transform.parent = transform;
        poisonEffect.transform.localPosition = Vector3.zero;
        while (elapsedTime < duration)
        {
            float damageToMakeThisFrame = damage * Time.deltaTime / duration;

            health -= damageToMakeThisFrame;
            health = Mathf.Clamp(health, 0, 100);
            healthBar.fillAmount = health / 100f;

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        Destroy(poisonEffect.gameObject);
    }
    private void OnHealthPowerUp()
    {
        sfx.PlaySFX("HealingSound");
        float healthToAdd = 25f;
        StartCoroutine(IncreaseHealthOverTime(healthToAdd, 3f));
    }

    private IEnumerator IncreaseHealthOverTime(float amount, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            healthImage.enabled = true;
            float healthToAddThisFrame = amount * Time.deltaTime / duration;

            if (health + healthToAddThisFrame > 100f)
            {
                healthToAddThisFrame = 100f - health;
            }

            health += healthToAddThisFrame;
            health = Mathf.Clamp(health, 0, 100);
            healthBar.fillAmount = health / 100f;

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        healthImage.enabled = false;
    }

    public void OnInstakillPowerUp()
    {
        sfx.PlaySFX("InstantKillSound");
        instaImage.enabled = true;
        StartCoroutine(Instakill());
        instaImage.enabled = false;
    }

    IEnumerator Instakill()
    {
        float timer = 0f;
        float duration = 15f;

        while (timer < duration)
        {
            isInstakill = true;
            instaImage.enabled = true;
            timer += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        isInstakill = false;
        instaImage.enabled = false;
    }

    private void OnNukePowerUp()
    {
        sfx.PlaySFX("NukeSound");
        Enemy[] activeBacteria = FindObjectsOfType<Enemy>();
        Instantiate(nukePrefab, transform.position, Quaternion.identity);
        foreach (Enemy bacteria in activeBacteria)
        {
            sfx.PlaySFX("NukeSound");
            Destroy(bacteria.gameObject);
            bacteria.OnDeath();
        }
    }

    public void Death(Collision2D collision)
    {
        sfx.PlaySFX("DeathSound");
        Destroy(healthBar);
        Destroy(collision.gameObject);
        GameOverUI.SetActive(true);
        Time.timeScale = 0f;
        CheckForHighScore();
    }

    private void CheckForHighScore()
    {
        if (GameManager.instance.GetScore() > GameManager.instance.GetHighScore())
        {
            GameManager.instance.SetHighScore(GameManager.instance.GetScore());
            return;
        }
        return;
    }

    public void Death()
    {
        sfx.PlaySFX("DeathSound");
        Destroy(healthBar);
        GameOverUI.SetActive(true);
        Time.timeScale = 0f;
        CheckForHighScore();
    }

    private void CheckForDeath()
    {
        if (health <= 0)
        {
            sfx.PlaySFX("DeathSound");
            Death();
        }
    }
}
