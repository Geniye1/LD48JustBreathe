using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toilet : MonoBehaviour
{
    public GameObject waterSpray;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb.velocity.magnitude > 0.5f)
        {
            Debug.Log("Bruh");
            waterSpray.SetActive(true);
            waterSpray.GetComponent<ParticleSystem>().Play();
        }
    }
}
