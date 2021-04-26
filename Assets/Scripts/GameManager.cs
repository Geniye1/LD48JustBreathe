using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public enum PlayerState
    {
        Traveling,
        Ragdoll,
        InTask,
        GameOver,
        Free
    }
    public static PlayerState playerState = PlayerState.Traveling;

    public PostProcessVolume volume;
    public GameObject gameOverKeyGO;
    public Image gameOverKeySprite;
    public float maxDamage;

    private CinemachineVirtualCamera cam;
    private Vignette vignette;

    public static float currentMainTimer;
    public static float currentKeyPressTimer;
    public static float timeTilCompletion;
    public static float minTimeBetweenKeyPress;
    public static float maxTimeBetweenKeyPress;
    public static KeyCode randomKey;
    public static KeyCode[] possibleKeys;

    public static Slider taskFillBar;
    public static ATask currentTask;
    public static GameObject keyIndicator;

    public static Image taskBarFillColor;

    private static AudioManager audioManager;

    private static int numberOfTasksCompleted = 0;

    private static TextMeshProUGUI keyText;

    private static GameObject completedMessage;

    private static Vector3 keyIndicatorOriginalScale;
    private static Vector3 keyIndicatorNewScale;

    private static bool isInMinigame = false;

    void Awake()
    {
        Cursor.visible = false;
    }

    void Start()
    {
        volume.profile.TryGetSettings(out vignette);
        cam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();

        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();

        completedMessage = GameObject.Find("Completed Message");
        completedMessage.SetActive(false);

        playerState = PlayerState.Traveling;
    }

    public void RagdollPlayer(GameObject playerGO, GameObject ragdoll, float ragdollTime, Slider _damageFillBar)
    {
        audioManager.TriggerExplosionMusic();
        audioManager.PlayExplosion();

        playerState = PlayerState.Ragdoll;
        playerGO.SetActive(false);
        GameObject currentRagdoll = Instantiate(ragdoll, playerGO.transform.position, Quaternion.identity);

        float totalDamage = 0;

        Vector3 explosionCenter = playerGO.transform.position;
        Collider[] ragdollColliders = Physics.OverlapSphere(explosionCenter, 5);
        foreach(Collider hit in ragdollColliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 randomOffset = new Vector3(Random.Range(0, 3), 0, Random.Range(0, 3));
                rb.AddExplosionForce(25, explosionCenter + randomOffset, 5, 3, ForceMode.Impulse);
                totalDamage++;
            }
        }

        if (isInMinigame)
        {
            currentTask.FailTask();
        }

        float newDamageBarFill = _damageFillBar.value + totalDamage;
        StartCoroutine(FillDamageBar(newDamageBarFill, _damageFillBar));

        if (newDamageBarFill <= maxDamage)
        {
            StartCoroutine(ResetPlayer(currentRagdoll, playerGO, ragdollTime));
        }
        else
        {
            playerState = PlayerState.GameOver;
            GameOver();
        }
    }

    public static void BeginKeypressMinigame(ATask task, Slider taskBar, Image _taskBarFillColor, GameObject _keyIndicator, KeyCode[] keys, 
        float _timeTilCompletion, float _minTimeBetweenKeyPress, float _maxTimeBetweenKeyPress)
    {
        currentTask = task;
        taskFillBar = taskBar.GetComponentInChildren<Slider>();
        taskFillBar.maxValue = _timeTilCompletion;

        taskBarFillColor = _taskBarFillColor;

        keyIndicator = _keyIndicator;
        keyText = keyIndicator.GetComponentInChildren<TextMeshProUGUI>();
        keyIndicatorOriginalScale = keyIndicator.transform.localScale;
        keyIndicatorNewScale = new Vector3(keyIndicator.transform.localScale.x * 1.2f, keyIndicator.transform.localScale.y * 1.2f, 0);

        possibleKeys = keys;
        timeTilCompletion = _timeTilCompletion;
        minTimeBetweenKeyPress = _minTimeBetweenKeyPress;
        maxTimeBetweenKeyPress = _maxTimeBetweenKeyPress;

        currentMainTimer = 0;
        currentKeyPressTimer = 0;

        isInMinigame = true;

        GenerateKeyPress();
    }

    private IEnumerator ResetPlayer(GameObject currentRagdoll, GameObject playerGO, float ragdollTime)
    {
        yield return new WaitForSeconds(ragdollTime);
        Vector3 ragdollLastPosition = currentRagdoll.transform.position;
        // When the player spawns back in they are in the running state by default, make that not happen
        Destroy(currentRagdoll);
        playerGO.SetActive(true);
        playerGO.transform.position = ragdollLastPosition;
        playerGO.GetComponent<BreathingController>().ResetState();

        audioManager.TriggerTravellingMusic();

        playerState = PlayerState.Traveling;
    }

    private IEnumerator FillDamageBar(float newFill, Slider _damageFillBar)
    {
        _damageFillBar.value = Mathf.Lerp(_damageFillBar.value, newFill, 0.3f);
        yield return new WaitForSeconds(0.01f);

        if (_damageFillBar.value <= newFill - 0.1)
        {
            StartCoroutine(FillDamageBar(newFill, _damageFillBar));
        }
    }

    private void GameOver()
    {
        vignette.color.value = Color.black;

        StartCoroutine(BringInVignette());
        StartCoroutine(GameOverCamera());
    }

    private IEnumerator BringInVignette()
    {
        vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 1, 0.1f);
        yield return new WaitForSeconds(0.1f);

        if (vignette.intensity.value < 0.95f)
        {
            StartCoroutine(BringInVignette());
        }
    }

    private IEnumerator GameOverCamera()
    {
        cam.m_Lens.OrthographicSize = Mathf.Lerp(cam.m_Lens.OrthographicSize, 10, 0.5f);
        yield return new WaitForSeconds(0.1f);

        if (cam.m_Lens.OrthographicSize > 9.5f)
        {
            StartCoroutine(SlowCamera());
        }
        else
        {
            gameOverKeyGO.SetActive(true);
            StartCoroutine(FadeInGameOverKey());
            StartCoroutine(GameOverCamera());
        }
    }

    private IEnumerator SlowCamera()
    {
        cam.m_Lens.OrthographicSize += 0.005f;
        yield return new WaitForSeconds(0.01f);
        StartCoroutine(SlowCamera());
    }

    private IEnumerator FadeInGameOverKey()
    {
        gameOverKeySprite.color = Color.Lerp(gameOverKeySprite.color, Color.white, 0.01f);
        yield return new WaitForSeconds(0.1f);

        if (gameOverKeySprite.color.a < 0.95f)
        {
            StartCoroutine(FadeInGameOverKey());
        }
    }

    public static void taskCompleted()
    {
        numberOfTasksCompleted++;

        if (numberOfTasksCompleted >= 4)
        {
            playerState = PlayerState.Free;
            completedMessage.SetActive(true);
        }
    }

    void Update()
    {
        if (playerState == PlayerState.InTask)
        {
            currentMainTimer += Time.deltaTime;
            taskFillBar.value = currentMainTimer;

            currentKeyPressTimer += Time.deltaTime;
            if (currentMainTimer >= timeTilCompletion)
            {
                keyText.text = "";
                isInMinigame = false;
                keyIndicator.transform.localScale = keyIndicatorOriginalScale;
                currentTask.CompleteTask();
                taskCompleted();
            }

            if (currentKeyPressTimer > minTimeBetweenKeyPress && currentKeyPressTimer < maxTimeBetweenKeyPress)
            {
                taskBarFillColor.color = Color.Lerp(taskBarFillColor.color, Color.green, 0.05f);
                keyIndicator.transform.localScale = Vector3.Lerp(keyIndicator.transform.localScale, keyIndicatorNewScale, 0.05f);
            }

            if (Input.GetKeyDown(randomKey))
            {
                if (currentKeyPressTimer >= minTimeBetweenKeyPress && currentKeyPressTimer <= maxTimeBetweenKeyPress)
                {
                    taskBarFillColor.color = Color.red;
                    keyIndicator.transform.localScale = keyIndicatorOriginalScale;
                    GenerateKeyPress();
                }
                else
                {
                    isInMinigame = false;
                    keyIndicator.transform.localScale = keyIndicatorOriginalScale;
                    currentTask.FailTask();
                }
                currentKeyPressTimer = 0;
            }
            else if (Input.anyKeyDown && !Input.GetMouseButtonDown(0))
            {
                isInMinigame = false;
                keyIndicator.transform.localScale = keyIndicatorOriginalScale;
                currentTask.FailTask();
            }

            if (currentKeyPressTimer > maxTimeBetweenKeyPress)
            {
                isInMinigame = false;
                currentTask.FailTask();
                keyIndicator.transform.localScale = keyIndicatorOriginalScale;
                currentKeyPressTimer = 0;
            }
        } 
        else if (playerState == PlayerState.GameOver)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else if (playerState == PlayerState.Free)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private static void GenerateKeyPress()
    {
        randomKey = possibleKeys[Random.Range(0, 4)];
        keyText.text = randomKey.ToString().ToLower();
    }
}
