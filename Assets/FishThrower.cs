using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FishThrower : MonoBehaviour
{
    [SerializeField] private Fish fishPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Transform appearingSpot = transform;
            appearingSpot.position += transform.forward;
            Fish fish = Instantiate(fishPrefab, appearingSpot);
            Vector3 endPosition = appearingSpot.position + transform.forward * 4;
            endPosition.y -= 5;
            fish.transform.DOMove(endPosition, 1f);
        }
    }
}