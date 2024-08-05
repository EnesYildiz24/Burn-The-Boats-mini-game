using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Virus : Enemy
{

    protected override void Start()
    {
        base.Start();
        speed = 1f;
        health = maxhealth = 150f;
        damage = 40f;
        numberOfSpikes = 16;
        spikeSpeed = 30f;
        points = 150f;
        randomFactor = 10;
    }
}