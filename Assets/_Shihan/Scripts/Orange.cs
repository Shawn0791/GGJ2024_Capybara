using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orange : MonoBehaviour
{
    public float satiationIncrease;
    void Start()
    {
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("InteractablePort"))
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                GameController.Instance.SatiationAdd(satiationIncrease);
                Destroy(gameObject);
            }
        }
    }

    void Update()
    {
        
    }
}
