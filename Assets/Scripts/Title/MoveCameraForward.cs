using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraForward : MonoBehaviour
{

    public float moveDuration;

    private Vector3 moveDirection;
    private float timer;

    void Awake()
    {
        moveDirection = transform.forward;
        StartCoroutine(MoveCamera());
    }

    void Update()
    {
        timer += Time.deltaTime;
    }

    private IEnumerator MoveCamera()
    {
        transform.position += moveDirection * 0.0005f;
        yield return new WaitForSeconds(0.01f);

        if (timer > moveDuration)
        {
            //moveDirection = -moveDirection;
            timer = 0;
        }

        StartCoroutine(MoveCamera());
    }

}
