using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Bacteria : Enemy
{

    // public GameObject deathPuddlePrefab;
    protected override void Start()
    {
        base.Start();
        speed = 2f;
        health = maxhealth = 20f;
        damage = 10f;
        points = 50f;
        randomFactor = 2;
    }

    // VERBUGGT
    /*
    protected override void OnDeath(Collision2D collision)
    {
        base.OnDeath();
        GameObject deathSpriteInstance = Instantiate(deathPuddlePrefab, transform.position, Quaternion.identity);
        StartCoroutine(DestroySpriteAfterDelay(deathSpriteInstance));
    }

    private IEnumerator DestroySpriteAfterDelay(GameObject spriteInstance)
    {
         yield return new WaitForSeconds(4f);
        if (spriteInstance)
        {
            Destroy(spriteInstance.gameObject);
        }
    }
    */
}