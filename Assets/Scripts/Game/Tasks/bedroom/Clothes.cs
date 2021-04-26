using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clothes : ATask
{

    public bool isTaskComplete = false;

    public string taskName;
    public float timeToTilCompletion;

    public float minTimeBetweenKeyPress;
    public float maxTimeBetweenKeyPress;

    public GameObject taskBar;
    public Image taskBarFillColor;
    public GameObject keyIndicator;

    public GameObject[] clothesOnGround;

    private Slider taskBarFill;

    private KeyCode[] possibleKeys = new KeyCode[]
    {
        KeyCode.Q,
        KeyCode.W,
        KeyCode.R,
        KeyCode.T
    };

    private float currentMainTimer;
    private float currentKeyPressTimer;

    private KeyCode randomKey;

    void Start()
    {
        taskBarFill = taskBar.GetComponentInChildren<Slider>();
    }

    public override void CompleteTask()
    {
        // Hide messy bed, enable clean bed
        // Spawn poof effect
        Debug.Log("Completed");
        taskBarFill.value = 0;
        taskBarFillColor.color = Color.red;
        taskBar.SetActive(false);

        keyIndicator.SetActive(false);

        foreach (GameObject clothes in clothesOnGround)
        {
            clothes.SetActive(false);
        }

        isTaskComplete = true;

        GameManager.playerState = GameManager.PlayerState.Traveling;

    }

    public override void FailTask()
    {
        Debug.Log("Failed");
        taskBarFill.value = 0;
        taskBarFillColor.color = Color.red;
        taskBar.SetActive(false);

        keyIndicator.SetActive(false);

        GameManager.playerState = GameManager.PlayerState.Traveling;
    }

    public override string GetNameOfTask()
    {
        return taskName;
    }
    public override bool isCompleted()
    {
        return isTaskComplete;
    }

    public override IEnumerator Interact()
    {
        yield return new WaitForSeconds(0.1f);
        taskBar.SetActive(true);
        keyIndicator.SetActive(true);
        GameManager.playerState = GameManager.PlayerState.InTask;
        GameManager.BeginKeypressMinigame(this, taskBarFill, taskBarFillColor, keyIndicator, possibleKeys, timeToTilCompletion, minTimeBetweenKeyPress, maxTimeBetweenKeyPress);
    }
}
