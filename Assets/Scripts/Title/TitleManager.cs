using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{

    public Animator imageAnimator;
    public GameObject controlsImage;

    private float timer;

    private bool hasBegun = false;

    void Awake()
    {
        Cursor.visible = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hasBegun = true;
            imageAnimator.SetBool("hasBegun", true);
            StartCoroutine(FadeAndLoad());
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            controlsImage.SetActive(!controlsImage.activeSelf);
        }

        if (hasBegun)
        {
            timer += Time.deltaTime;
        }
    }

    private IEnumerator FadeAndLoad()
    {
        yield return new WaitForSeconds(0.01f);
        if (timer > 2.5f)
        {
            SceneManager.LoadScene("Bedroom");
        }
        else
        {
            StartCoroutine(FadeAndLoad());
        }
    }

}
