using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Krebs : Enemy
{
    
    public GameObject cancerDeathPrefab;

    protected override void Start()
    {
        base.Start();
        speed = 0.5f;
        health = maxhealth = 250f;
        damage = 80f;
        points = 400f;
        randomFactor = 25;
    }

    protected override void OnDeath(Collision2D collision)
    {
        base.OnDeath();
        GameObject cancerDeathInstance = Instantiate(cancerDeathPrefab, transform.position, Quaternion.identity);
        StartCoroutine(DestroyAfterDelay(cancerDeathInstance));

    }

    private IEnumerator DestroyAfterDelay(GameObject instance)
    {
        yield return new WaitForSeconds(8f);

        if (instance)
        {
            Destroy(instance);
        }
    }
}