using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    [SerializeField] private float minimumTimeBetweenFarts = 0.5f;
    [SerializeField] private float maximumTimeBetweenFarts = 1.5f;
    void Update()
    {

    }
}
