using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public static GameController instance;

    public GameObject playerPrefab;
    public GameObject cpuPrefab;
    public GameObject backgroundPrefab;
    public GameObject pipePrefab;
    public float pipeSpacing = 1f;
    public float moveSpeedStart = 2f;
    public float moveSpeedMax = 4f;
    public float moveSpeedIncrease = 0.5f;
    public bool isGameOver = false;

    private AudioSource aS;
    private Transform worldTransform;
    private Transform gameOverUI;
    private float backgroundSpriteLength = 0f;
    private float totalPipeSpacing = 0f;
    private int currentSceneIndex = 0;
    private float currentMoveSpeed = 0f;

    private void OnEnable() {
        if (instance != this)
            return;

        SceneManager.sceneLoaded += SetupScene;
    }

    private void OnDisable() {
        if (instance != this)
            return;

        SceneManager.sceneLoaded -= SetupScene;
    }

    private void Start() {
        //creates an instance of the script
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        aS = GetComponent<AudioSource>();

        //I dont know if I should like this solution or not...
        OnEnable();

        SetupScene(0);
    }

    private void Update() {
        if (currentSceneIndex == 0)
            return;

        currentMoveSpeed += moveSpeedIncrease * Time.deltaTime;
    }

    public void GameOver() {

    }

    private void SetupScene(int sceneIndex) {
        currentSceneIndex = sceneIndex;

        switch (sceneIndex) {
            case 0:
                GameObject.Find("Start Button").GetComponent<Button>().onClick.AddListener(delegate { LoadScene(1); });
                GameObject.Find("Quit Button").GetComponent<Button>().onClick.AddListener(delegate { QuitGame(); });
                break;
            case 1:
                worldTransform = GameObject.Find("World").transform;
                gameOverUI = GameObject.Find("UI Layer").transform.GetChild(0);
                currentMoveSpeed = moveSpeedStart;

                //Get largest sprite length
                foreach (SpriteRenderer sprite in backgroundPrefab.GetComponentsInChildren<SpriteRenderer>()) {
                    if (sprite.bounds.size.x > backgroundSpriteLength)
                        backgroundSpriteLength = sprite.bounds.size.x;
                }

                //+1 for scrolling buffer, *0.01f for "safety" overhead
                for (int index = (int)Mathf.Ceil(2.01f * Camera.main.orthographicSize * Camera.main.aspect / backgroundSpriteLength) + 1; index > 0; --index) {
                    Instantiate(backgroundPrefab, ((index - 2) * backgroundSpriteLength) * Vector3.right, Quaternion.identity, worldTransform.Find("Background"));
                }

                //Get largest sprite length
                foreach (SpriteRenderer sprite in pipePrefab.GetComponentsInChildren<SpriteRenderer>()) {
                    if (sprite.bounds.size.x > totalPipeSpacing)
                        totalPipeSpacing = sprite.bounds.size.x;
                }
                totalPipeSpacing += pipeSpacing;

                //+1 for scrolling buffer, *0.01f for "safety" overhead
                for (int index = (int)Mathf.Ceil(2.01f * Camera.main.orthographicSize * Camera.main.aspect / totalPipeSpacing) + 1; index > 0; --index) {
                    Instantiate(pipePrefab, ((index - 2) * totalPipeSpacing) * Vector3.right, Quaternion.identity, worldTransform.Find("Pipes"));
                }

                break;
            default:
                break;
        }
    }

    private void SetupScene(Scene scene, LoadSceneMode mode) {
        SetupScene(scene.buildIndex);
    }

    public void LoadScene(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
