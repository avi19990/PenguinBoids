using System;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private float timer;
    private float livingTime = 6f;

    private void Start()
    {
        transform.parent = null;
    }

    private void Update()
    {
        if (HasDied()) Destroy(gameObject);
        CountUpTimer();
    }

    private bool HasDied()
    {
        return timer >= livingTime;
    }

    private void CountUpTimer()
    {
        timer += Time.deltaTime;
    }
}