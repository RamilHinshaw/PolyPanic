using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;

    public Camera cam;


    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player");

        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        transform.position = target.transform.position + offset;
    }
}
