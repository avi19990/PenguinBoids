using UnityEngine;

public class Fish : MonoBehaviour
{
    private float timer;
    private float livingTime = 6f;

    private void Start()
    {
        transform.parent = null;
        GameDataHolder.FishAmount++;
    }

    private void Update()
    {
        if (HasDied())
        {
            GameDataHolder.FishAmount--;
            Destroy(gameObject);
        }
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