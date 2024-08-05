using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public float health, maxhealth, points, damage;
    public Vector3 directionToPlayer;
    public Vector3 localScale;
    public PlayerController player;
    public Rigidbody2D rb;


    public GameObject spikePrefab; // Referenz auf das Prefab deiner Bullet
    public float spikeSpeed; // Geschwindigkeit der Bullets

    public GameObject healthBarPrefab;

    public int numberOfSpikes;

    public GameObject healthBarInstance;

    public int randomFactor;
    public GameObject powerupPrefab;
    public GameObject explosionPrefab;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType(typeof(PlayerController)) as PlayerController;
        localScale = transform.localScale;

        healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
        healthBarInstance.transform.SetParent(transform);
        //  healthBarInstance.GetComponent<Image>().color = Color.red; MAKE BUG
    }

    void FixedUpdate()
    {
        MoveEnemy();
    }


    void Update()
    {

        if (float.IsNaN(health) || float.IsNaN(maxhealth) || float.IsInfinity(health) || float.IsInfinity(maxhealth) || maxhealth <= 0)
        {
            return;
        }

        float healthPercentage = health / maxhealth;


        if (float.IsNaN(healthPercentage) || float.IsInfinity(healthPercentage))
        {
            return;
        }

        healthBarInstance.transform.localScale = new Vector3(Mathf.Clamp01(healthPercentage), 1f, 1f);
    }
    public void MoveEnemy()
    {
        directionToPlayer = (player.transform.position - transform.position).normalized;
        rb.velocity = new Vector2(directionToPlayer.x, directionToPlayer.y) * speed;
    }

    protected virtual void OnDeath(Collision2D collision)
    {
        ShootBulletsOnDeath();
        GameManager.instance.AddPoints(points);
        Destroy(gameObject);
        Destroy(collision.gameObject);
        Destroy(healthBarInstance);
        RollTheDiceOnDeath();
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }

    public virtual void OnDeath()
    {
        ShootBulletsOnDeath();
        GameManager.instance.AddPoints(points);
        Destroy(gameObject);
        Destroy(healthBarInstance);
        RollTheDiceOnDeath();
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }
    
    private void ApplyKnockback(Vector2 collisionPoint)
    {
        Vector2 knockbackDirection = (Vector2)transform.position - collisionPoint;
        knockbackDirection.Normalize();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(knockbackDirection * 5f, ForceMode2D.Impulse);
        }
    }

    private void RollTheDiceOnDeath()
    {
        int randomNumber = Random.Range(1, 101);
        if (randomNumber <= randomFactor)
        {
            GameManager.instance.RandomPowerup();
            Instantiate(powerupPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            PlayerController playerControllerInstance = FindObjectOfType<PlayerController>();

            
            if (playerControllerInstance == null)
            {
                GameObject playerControllerObject = new GameObject("PlayerControllerObject");
                playerControllerInstance = playerControllerObject.AddComponent<PlayerController>();
            }

            if (playerControllerInstance.isInstakill)
            {
                OnDeath(collision);
                return;
            }

            if (health - playerControllerInstance.damage > 0)
            {
                health -= playerControllerInstance.damage;

                float healthPercentage = health / maxhealth;
                healthBarInstance.transform.localScale = new Vector3(healthPercentage, 1f, 1f);

                ApplyKnockback(collision.contacts[0].point);

                Destroy(collision.gameObject);
                return;
            }
            OnDeath(collision);
        }else if(collision.gameObject.tag == "Explosion"){
            OnDeath();
        }
    }

    private void ShootBulletsOnDeath()
    {
        // Anzahl der Bullets und der Winkel zwischen ihnen
        int numberOfSpikes = this.numberOfSpikes;
        float angleStep = 360f / numberOfSpikes;

        // Startwinkel für das erste Bullet
        float currentAngle = 0f;

        for (int i = 0; i < numberOfSpikes; i++)
        {
            // Berechne die Richtung des Bullets basierend auf dem Winkel
            Vector3 spikeDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * currentAngle), Mathf.Sin(Mathf.Deg2Rad * currentAngle), 0f);

            // Erzeuge das Bullet
            GameObject spike = Instantiate(spikePrefab, transform.position, Quaternion.identity);

            // Setze die Geschwindigkeit des Bullets
            spike.GetComponent<Rigidbody2D>().velocity = spikeDirection * spikeSpeed;

            // Inkrementiere den Winkel für das nächste Bullet
            currentAngle += angleStep;
            // }
        }
    }
}
