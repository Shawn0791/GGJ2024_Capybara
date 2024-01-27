using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    [SerializeField] private float minimumTimeBetweenFarts = 2f;
    [SerializeField] private float maximumTimeBetweenFarts = 4f;
    [SerializeField] private float shortTimeBetweenFarts = 0.2f;
    [SerializeField] private float shortTimeBetweenFartsChance = 0.1f;
    [SerializeField] private float minimumFartForce = 1f;
    [SerializeField] private float maximumFartForce = 10f;
    [SerializeField] private float minimumDisturbanceAngle = -10f;
    [SerializeField] private float maximumDisturbanceAngle = 10f;
    private DateTime lastFartTime;
    private DateTime nextFartTime;
    private Capybara player;
    protected override void Awake()
    {
        base.Awake();
        lastFartTime = DateTime.Now;
        UpdateNextFartTime();
        player = FindObjectOfType<Capybara>();
    }

    private void Update()
    {
        DateTime now = DateTime.Now;
        if (now > nextFartTime)
        {
            lastFartTime = now;
            UpdateNextFartTime();
            player.EnqueueFartEvent(
                UnityEngine.Random.Range(minimumFartForce, maximumFartForce),
                UnityEngine.Random.Range(minimumDisturbanceAngle, maximumDisturbanceAngle));
            Debug.Log("Enqueued standard fart event");
            StartCoroutine(EnqueueSecondFart());
        }
    }

    private void UpdateNextFartTime()
    {
        float nextDelta = UnityEngine.Random.Range(minimumTimeBetweenFarts, maximumTimeBetweenFarts);
        nextFartTime = lastFartTime.AddSeconds(nextDelta);
    }

    private IEnumerator EnqueueSecondFart()
    {
        yield return new WaitForSeconds(shortTimeBetweenFarts);
        DateTime now = DateTime.Now;
        if (UnityEngine.Random.value < shortTimeBetweenFartsChance)
        {
            player.EnqueueFartEvent(
                UnityEngine.Random.Range(minimumFartForce, maximumFartForce) / 1.5f,
                UnityEngine.Random.Range(minimumDisturbanceAngle, maximumDisturbanceAngle) / 1.5f);
            lastFartTime = now;
            UpdateNextFartTime();
            Debug.Log("Enqueued short fart event");
        }
    }
}
