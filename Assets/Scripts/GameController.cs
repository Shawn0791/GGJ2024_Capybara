using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    [Header("UI Data")]
    private float satiation;
    public float satiationMax;
    public float satiationReduceSpeed;
    private float restlessness;
    public float restlessnessMax;
    public float restReduceSpeed;
    public Slider satiationBar;
    public Slider restlessnessBar;
    protected override void Awake()
    {
        base.Awake();
        lastFartTime = DateTime.Now;
        UpdateNextFartTime();
        player = FindObjectOfType<Capybara>();

        satiation = 0;
        restlessness = restlessnessMax;
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

        DataUpdate();
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

    public void SatiationAdd(float amount)
    {
        satiation = Mathf.Clamp(satiation + amount, 0, satiationMax);
        minimumTimeBetweenFarts = Mathf.Lerp(4, 2, satiation / satiationMax);
        maximumTimeBetweenFarts = Mathf.Lerp(8, 4, satiation / satiationMax);
        minimumFartForce = Mathf.Lerp(50, 100, satiation / satiationMax);
        maximumFartForce = Mathf.Lerp(100, 200, satiation / satiationMax);
    }

    public void RestlessnessAdd(float amount)
    {
        restlessness += amount;
        if (restlessness > restlessnessMax)
            restlessness = restlessnessMax;
    }

    private void DataUpdate()
    {
        if (satiation >= 0)
            satiation -= satiationReduceSpeed * Time.deltaTime;
        satiationBar.value = satiation / satiationMax;

        if (restlessness >= 0)
            restlessness -= restReduceSpeed * Time.deltaTime;
        restlessnessBar.value = restlessness / restlessnessMax;
    }
}
