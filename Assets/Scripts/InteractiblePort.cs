using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiblePort : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractible interactible = collision.gameObject.GetComponent<IInteractible>();
        if (interactible != null)
        {
            transform.parent.GetComponentInParent<Capybara>().AddInteractibleObject(interactible);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractible interactible = collision.gameObject.GetComponent<IInteractible>();
        if (interactible != null)
        {
            transform.parent.GetComponentInParent<Capybara>().RemoveInteractibleObject(interactible);
        }
    }
}
