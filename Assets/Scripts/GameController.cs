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
    public float moveSpeedStart = 2f;
    public float moveSpeedMax = 4f;
    public float moveSpeedIncrease = 0.5f;
    public bool isGameOver = false;
    public Color customPipeColor;

    private AudioSource aS;
    private Transform worldTransform;
    private Transform gameOverUI;
    private float backgroundSpriteLength = 0f;
    private float totalPipeSpacing = 0f;
    private int currentSceneIndex = 0;

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
    }

    public void GameOver(Bird bird) {
        foreach (Transform child1 in bird.transform.parent.parent) {
            foreach (Transform child2 in child1) {
                if (child2 == bird.transform)
                    continue;

                Destroy(child2.gameObject);
            }
        }

        SetupWorld(bird.transform.parent.parent, false);
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

                Transform newWorld;
                for (int i = 1; i < 15; i++) {
                    newWorld = Instantiate(worldTransform.gameObject, i * 1.5f * backgroundSpriteLength * Vector3.up, Quaternion.identity).transform;
                    SetupWorld(newWorld);
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
        //+1 for scrolling buffer, *0.01f for "safety" overhead
        for (int index = (int)Mathf.Ceil(2.01f * Camera.main.orthographicSize * Camera.main.aspect / backgroundSpriteLength) + 1; index > 0; --index) {
            Instantiate(backgroundPrefab, ((index - 2) * backgroundSpriteLength) * Vector3.right + world.position, Quaternion.identity, world.Find("Background"));
        }

        //+1 for scrolling buffer, *0.01f for "safety" overhead
        for (int index = (int)Mathf.Ceil(2.01f * Camera.main.orthographicSize * Camera.main.aspect / totalPipeSpacing) + 1; index > 0; --index) {
            Instantiate(pipePrefab, new Vector3((index + 1) * totalPipeSpacing, Random.Range(pipeRandom.x, pipeRandom.y), 0f) + world.position, Quaternion.identity, world.Find("Pipes"));
        }

        if (spawnBird)
            Instantiate(cpuPrefab, world.position, Quaternion.identity, world.Find("Entities"));
    }

    public void UpdateWorld(Bird bird) {
        bird.currentMoveSpeed = Mathf.Min(bird.currentMoveSpeed + moveSpeedIncrease * Time.deltaTime, moveSpeedMax);

        foreach (Transform child in bird.transform.parent.parent.Find("Background")) {
            child.Translate(-bird.currentMoveSpeed * Time.deltaTime, 0f, 0f);

            if (Camera.main.orthographicSize * Camera.main.aspect + (backgroundSpriteLength * 0.5f) + child.localPosition.x < 0f) {
                child.Translate(backgroundSpriteLength * child.parent.childCount, 0f, 0f);
            }
        }

        bool forceSetPrevPipe = bird.GetClosestPipe() == null;
        float pipeLengthHalfBirdCollider = (totalPipeSpacing - pipeSpacing) * -0.5f - bird.col.radius;
        foreach (Transform child in bird.transform.parent.parent.Find("Pipes")) {
            child.Translate(-bird.currentMoveSpeed * Time.deltaTime, 0f, 0f);

            if (Camera.main.orthographicSize * Camera.main.aspect + (totalPipeSpacing * 0.5f) + child.localPosition.x < 0f) {
                child.localPosition = new Vector3(child.localPosition.x + totalPipeSpacing * child.parent.childCount, Random.Range(pipeRandom.x, pipeRandom.y), child.localPosition.z);
            }

            if (forceSetPrevPipe || Mathf.Abs(child.position.x - bird.transform.position.x + pipeLengthHalfBirdCollider) < Mathf.Abs(bird.GetClosestPipe().position.x - bird.transform.position.x + pipeLengthHalfBirdCollider))
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
