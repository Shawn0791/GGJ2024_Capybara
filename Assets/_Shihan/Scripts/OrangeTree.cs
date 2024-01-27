using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeTree : MonoBehaviour
{
    public List<GameObject> oranges = new List<GameObject>();
    public GameObject orangePrefab;
    public float dropIntervalMin, dropIntervalMax;
    private Coroutine dropCoroutine;
    private void Update()
    {

    }

    private IEnumerator RandomDropOrange()
    {
        float i = Random.Range(dropIntervalMin, dropIntervalMax);
        yield return new WaitForSeconds(i);

        OrangeDrop();

        dropCoroutine = StartCoroutine(RandomDropOrange());
    }
    private void OrangeDrop()
    {
        if (oranges.Count > 0)
        {
            Instantiate(orangePrefab, oranges[0].transform.position, Quaternion.identity);
            Destroy(oranges[0].gameObject);
            oranges.Remove(oranges[0]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ViewPort"))
        {
            dropCoroutine = StartCoroutine(RandomDropOrange());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ViewPort"))
        {
            StopCoroutine(dropCoroutine);
        }
    }
}
