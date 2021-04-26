using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private ATask currentTask;
    private GameObject currentHighlight;
    private AudioManager audioManager;

    void Start()
    {
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
    }

    void OnTriggerEnter(Collider other)
    {

        currentTask = other.GetComponent<ATask>();

        if (currentTask != null)
        {
            if (!currentTask.isCompleted())
            {
                if (currentHighlight != null)
                {
                    currentHighlight.SetActive(false);
                    currentHighlight = null;
                }

                Transform parent = other.transform;
                foreach (Transform t in parent)
                {
                    if (t.tag == "Highlight")
                    {
                        currentHighlight = t.gameObject;
                        currentHighlight.SetActive(true);
                        break;
                    }
                }

                Debug.Log("TASK " + currentTask.GetNameOfTask() + " IN RANGE");
            }   
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (currentHighlight != null)
        {
            currentHighlight.SetActive(false);
        }
        
        currentHighlight = null;
        currentTask = null;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "Ground" && collision.transform.tag != "Wall")
        {
            audioManager.PlayHit();
        } 
    }

    void Update()
    {
        if (GameManager.playerState == GameManager.PlayerState.Traveling)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (currentTask != null)
                {
                    StartCoroutine(currentTask.Interact());
                }
            }
        }  
    }
}
