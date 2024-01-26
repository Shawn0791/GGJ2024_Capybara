using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeTree : MonoBehaviour
{
    public List<GameObject> oranges = new List<GameObject>();
    public GameObject orangePrefab;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OrangeDrop();
        }
    }
    public void OrangeDrop()
    {
        if (oranges.Count > 0)
        {
            Instantiate(orangePrefab, oranges[0].transform.position, Quaternion.identity);
            Destroy(oranges[0].gameObject);
            oranges.Remove(oranges[0]);
        }
    }
}
