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
            if (InputController.Instance.GetCurrentControllable() == controllable)
            {
                InputController.Instance.IsInWater = true;
            }

            if (!(controllable is Capybara) && !(controllable is Turtle))
            {
                controllable.OnInteract();
                Destroy(collision.gameObject);
            }
            Debug.Log($"isInWater {InputController.Instance.IsInWater}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IControllable controllable = collision.gameObject.GetComponent<IControllable>();
        if (controllable != null)
        {
            if (InputController.Instance.GetCurrentControllable() == controllable)
            {
                InputController.Instance.IsInWater = false;
            }
            Debug.Log($"isInWater {InputController.Instance.IsInWater}");
        }
    }
}
