using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class Water : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IControllable controllable = collision.gameObject.GetComponent<IControllable>();
        if (controllable != null)
        {
            if (controllable is Capybara || controllable is Turtle)
            {
                InputController.Instance.IsInWater = true;
            }
            else
            {
                controllable.OnInteract();
                Destroy(collision.gameObject);
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IControllable controllable = collision.gameObject.GetComponent<IControllable>();
        if (controllable != null)
        {
            InputController.Instance.IsInWater = false;
        }
    }
}
