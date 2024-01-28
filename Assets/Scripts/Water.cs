using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] AudioClip intoWaterClip;
    [SerializeField] AudioClip outOfWaterClip;
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IControllable controllable = collision.gameObject.GetComponent<IControllable>();
        if (controllable != null)
        {
            if (InputController.Instance.GetCurrentControllable() == controllable)
            {
                InputController.Instance.IsInWater = true;
                if (controllable is Turtle && !audioSource.isPlaying && intoWaterClip)
                {
                    audioSource.PlayOneShot(intoWaterClip);
                }
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
                if (controllable is Turtle && !audioSource.isPlaying && outOfWaterClip)
                {
                    audioSource.PlayOneShot(outOfWaterClip);
                }
            }
            Debug.Log($"isInWater {InputController.Instance.IsInWater}");
        }
    }
}
