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
    public Vector2 pipeRandom;
    public float moveSpeedMin = 2f;
    public float moveSpeedMax = 4f;
    public float moveSpeedIncrease = 0.5f;
    public bool isGameOver = false;
    public bool playerWon = false;
    public bool useCustomPipeColor = false;
    public Color customPipeColor;

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

        //I dont know if I should like this solution or not...
        OnEnable();

        SetupScene(0);
    }

    private void Update() {
        if (worldTransform && SceneManager.GetActiveScene().buildIndex == 1)
            UpdateWorld(worldTransform);

        if (Input.GetButtonDown("Submit"))
            isGameOver = false;
    }

    public void GameOver(Transform world) {
        isGameOver = true;
        Time.timeScale = 0f;
        gameOverUI.gameObject.SetActive(true);

        gameOverUI.GetChild(0).GetComponent<Text>().text = (playerWon ? "Player wins" : "AI wins");

        StartCoroutine(RestartWorld(world));
    }

    private IEnumerator RestartWorld(Transform world) {
        while (isGameOver) {
            yield return null;
        }
        
        foreach (Transform child1 in world) {
            foreach (Transform child2 in child1) {
                if (child2.gameObject.layer == LayerMask.NameToLayer("Bird"))
                    continue;

                Destroy(child2.gameObject);
            }
        }

        Time.timeScale = 1f;
        gameOverUI.gameObject.SetActive(false);

        SetupWorld(world, false);
        world.Find("Entities").GetComponentInChildren<cpuController>().ResetAgent();
        world.Find("Entities").GetComponentInChildren<PlayerController>().ResetPlayer();
    }

    private void SetupScene(int sceneIndex) {
        currentSceneIndex = sceneIndex;

        switch (sceneIndex) {
            case 0:
                GameObject.Find("Start Button").GetComponent<Button>().onClick.AddListener(delegate { LoadScene(1); });
                GameObject.Find("Quit Button").GetComponent<Button>().onClick.AddListener(delegate { QuitGame(); });
                break;
            case 1:
                gameOverUI = GameObject.Find("UI Layer").transform.GetChild(0);

                backgroundSpriteLength = 0f;
                //Get largest sprite length
                foreach (SpriteRenderer sprite in backgroundPrefab.GetComponentsInChildren<SpriteRenderer>()) {
                    if (sprite.bounds.size.x > backgroundSpriteLength)
                        backgroundSpriteLength = sprite.bounds.size.x;
                }

                totalPipeSpacing = 0f;
                //Get largest sprite length
                foreach (SpriteRenderer sprite in pipePrefab.GetComponentsInChildren<SpriteRenderer>()) {
                    if (sprite.bounds.size.x > totalPipeSpacing)
                        totalPipeSpacing = sprite.bounds.size.x;
                }
                totalPipeSpacing += pipeSpacing;

                for (int i = 0; i < 1; i++) {
                    worldTransform = Instantiate(GameObject.Find("World").transform, i * 1.5f * backgroundSpriteLength * Vector3.up, Quaternion.identity).transform;
                    SetupWorld(worldTransform);
                }

                break;
            default:
                break;
        }
    }

    private void SetupScene(Scene scene, LoadSceneMode mode) {
        SetupScene(scene.buildIndex);
    }

    public void SetupWorld(Transform world, bool spawnBird = true) {
        currentMoveSpeed = moveSpeedMin;

        //+1 for scrolling buffer, *0.01f for "safety" overhead
        for (int index = (int)Mathf.Ceil(2.01f * Camera.main.orthographicSize * Camera.main.aspect / backgroundSpriteLength) + 1; index > 0; --index) {
            Instantiate(backgroundPrefab, ((index - 2) * backgroundSpriteLength) * Vector3.right + world.position, Quaternion.identity, world.Find("Background"));
        }

        //+1 for scrolling buffer, *0.01f for "safety" overhead
        for (int index = (int)Mathf.Ceil(2.01f * Camera.main.orthographicSize * Camera.main.aspect / totalPipeSpacing) + 1; index > 0; --index) {
            Instantiate(pipePrefab, new Vector3((index + 1) * totalPipeSpacing, Random.Range(pipeRandom.x, pipeRandom.y), 0f) + world.position, Quaternion.identity, world.Find("Pipes"));
        }

        if (spawnBird) {
            Instantiate(cpuPrefab, world.position, Quaternion.identity, world.Find("Entities"));
            Instantiate(playerPrefab, world.position, Quaternion.identity, world.Find("Entities"));
        }
    }

    public void UpdateWorld(Transform world) {
        currentMoveSpeed = Mathf.Min(currentMoveSpeed + moveSpeedIncrease * Time.deltaTime, moveSpeedMax);
        cpuController bird = world.GetComponentInChildren<cpuController>();
        bool forceSetPrevPipe = bird.GetClosestPipe() == null;
        float pipeLengthHalfBirdCollider = (totalPipeSpacing - pipeSpacing) * 0.5f + bird.col.radius;

        foreach (Transform child in world.Find("Background")) {
            child.Translate(-currentMoveSpeed * Time.deltaTime, 0f, 0f);

            if (Camera.main.orthographicSize * Camera.main.aspect + (backgroundSpriteLength * 0.5f) + child.localPosition.x < 0f && !forceSetPrevPipe) {
                child.Translate(backgroundSpriteLength * child.parent.childCount, 0f, 0f);
            }
        }

        foreach (Transform child in world.Find("Pipes")) {
            child.Translate(-currentMoveSpeed * Time.deltaTime, 0f, 0f);

            if (Camera.main.orthographicSize * Camera.main.aspect + (totalPipeSpacing * 0.5f) + child.localPosition.x < 0f) {
                child.localPosition = new Vector3(child.localPosition.x + totalPipeSpacing * child.parent.childCount, Random.Range(pipeRandom.x, pipeRandom.y), child.localPosition.z);
            }

            if (forceSetPrevPipe || Mathf.Abs(child.localPosition.x - pipeLengthHalfBirdCollider) < Mathf.Abs(bird.GetClosestPipe().localPosition.x - pipeLengthHalfBirdCollider))
                bird.SetClosestPipe(child, forceSetPrevPipe);
        }
    }

    public void LoadScene(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
