using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orange : MonoBehaviour
{
    public float satiationIncrease;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("InteractablePort"))
        {
            if (InputController.Instance.IsConsuming)
            {
                GameController.Instance.SatiationAdd(satiationIncrease);
                Destroy(gameObject);
            }
        }
    }
}
