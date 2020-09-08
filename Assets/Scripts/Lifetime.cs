using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    public float lifetime = 1f;
    private float lifetimeTimer;

    // Start is called before the first frame update
    void OnEnable()
    {
        lifetimeTimer = lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        lifetimeTimer -= Time.deltaTime;

        if (lifetimeTimer <= 0)
            gameObject.SetActive(false);
    }
}
