using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarUI : MonoBehaviour
{
    public static BarUI instance;
    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(this);
    }

}
