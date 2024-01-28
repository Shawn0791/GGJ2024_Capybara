using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgMovement : MonoBehaviour
{
    public Transform vCamera;
    public float paraX, paraY;
    private Vector3 originalPos;
    private void Start()
    {
        originalPos = transform.position;
    }
    private void Update()
    {
        transform.position = new Vector3(vCamera.position.x - (vCamera.position.x - originalPos.x) * paraX,
            vCamera.position.y - (vCamera.position.y - originalPos.y) * paraY, 1);
    }
}
