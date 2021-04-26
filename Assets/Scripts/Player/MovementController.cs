using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    public float walkSpeed;
    public float runSpeed;

    private float speed;

    private Rigidbody rb;
    private Animator anim;

    public AudioSource walkingAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.playerState == GameManager.PlayerState.Traveling || GameManager.playerState == GameManager.PlayerState.Free)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            if (Input.GetKey(KeyCode.LeftShift))
            {
                //h *= runMultipler;
                //v *= runMultipler;

                speed = runSpeed;
                anim.SetBool("isWalking", false);
                anim.SetBool("isRunning", true);
            }
            else
            {
                speed = walkSpeed;
                anim.SetBool("isRunning", false);
                anim.SetBool("isWalking", true);
            }

            if (h != 0 || v != 0)
            {
                Vector3 velVec = new Vector3(h * Time.deltaTime, 0, v * Time.deltaTime);
                velVec.Normalize();
                rb.velocity = velVec * speed;
                Quaternion lookRotation = Quaternion.LookRotation(velVec.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0.5f);
                anim.SetFloat("speed", velVec.magnitude);

                if (!walkingAudioSource.isPlaying)
                {
                    walkingAudioSource.Play();
                }
            }
            else
            {
                anim.SetFloat("speed", 0);
                anim.SetBool("isRunning", false);
                anim.SetBool("isWalking", false);
                rb.velocity = Vector3.zero;

                if (walkingAudioSource.isPlaying)
                {
                    walkingAudioSource.Stop();
                }
            }
        }
        else
        {
            if (walkingAudioSource.isPlaying)
            {
                walkingAudioSource.Stop();
            }

        }
    }
}
