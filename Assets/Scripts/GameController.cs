using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField]private float restlessness;
    public float restlessnessMax;
    public float restReduceSpeed;
    public Slider satiationBar;
    public Slider restlessnessBar;
    public Image sceneTransfer;
    public GameObject gameOverUI;
    private float fartPara;
    private AudioSource audioSource;
    protected override void Awake()
    {
        base.Awake();
        lastFartTime = DateTime.Now;
        UpdateNextFartTime();
        player = FindObjectOfType<Capybara>();
        audioSource = GetComponent<AudioSource>();

        satiation = satiationMax * 0.5f;
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
                UnityEngine.Random.Range(minimumFartForce * fartPara, maximumFartForce * fartPara),
                UnityEngine.Random.Range(minimumDisturbanceAngle, maximumDisturbanceAngle)) ;
            Debug.Log("Enqueued standard fart event");
            StartCoroutine(EnqueueSecondFart());
        }

        DataUpdate();
    }

    private void UpdateNextFartTime()
    {
        float nextDelta = UnityEngine.Random.Range(minimumTimeBetweenFarts * (1 - fartPara), maximumTimeBetweenFarts * (1 - fartPara));
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
        satiationBar.value = fartPara = satiation / satiationMax;


        if (restlessness >= 0)
        {
            restlessness -= restReduceSpeed * Time.deltaTime;
            if (restlessness < 0.2 * restlessnessMax)
            {
                sceneTransfer.color = new Color(sceneTransfer.color.r, sceneTransfer.color.g, sceneTransfer.color.b, 1 - (restlessness / (0.2f * restlessnessMax)));
            }
            else
                sceneTransfer.color = new Color(sceneTransfer.color.r, sceneTransfer.color.g, sceneTransfer.color.b, 0);
        }
        else
            gameOverUI.SetActive(true);

        restlessnessBar.value = restlessness / restlessnessMax;

        audioSource.pitch = (restlessness / restlessnessMax * (1.2f - 0.8f)) + 0.8f;
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(0);
    }
}
