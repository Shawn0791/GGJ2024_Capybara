using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orange : MonoBehaviour
{
    [SerializeField] AudioClip consumeSound;
    private AudioSource audioSource;
    public float satiationIncrease;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("InteractablePort"))
        {
            if (InputController.Instance.IsConsuming)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(consumeSound);
                }
                GameController.Instance.SatiationAdd(satiationIncrease);
                GameController.Instance.RestlessnessAdd(satiationIncrease);
                Destroy(gameObject);
            }
        }
    }
}
