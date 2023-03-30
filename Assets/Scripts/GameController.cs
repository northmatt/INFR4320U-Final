using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public static GameController instance;

    public bool isGameOver = false;
    public float gameOverWaitTime;
    public Transform gameOverUI;
    public Transform scoreUIParent;
    public GameObject scoreUIPrefab;
    public Transform playersMainParent;
    public Transform terrainMainParent;
    public Transform bulletsMainParent;
    public AudioClip[] BGM;

    private AudioSource aS;
    private float gameOverWaitTimeReset;
    private List<int> playerScore = new List<int>();

    private void Start() {
        //creates an instance of the script
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;

        aS = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Confined;

        SetupGame();
    }

    private void Update() {
        //New comment: OH GOD THE NESTING! WHY DID I DO THIS?
        //if not gameover and if only one player is in the "players" gameobject then find it's playerID and SpriteRenderColor, then input the info into the "GameOver" function
        //if there are no players remaining while it's not gameover then make it a tie
        if (!isGameOver) {
            if (playersMainParent.childCount == 1) {
                Transform player = playersMainParent.GetChild(0);
                GameOver(player.GetComponent<PlayerController>().playerID, player.GetComponent<SpriteRenderer>().color);
            }
            else if (playersMainParent.childCount == 0) {
                GameOver(-1, Color.white);
            }
            else if (Input.GetButtonDown("Cancel")) {
                SceneManager.LoadScene("main menu");
            }
        }
        //if it is a gameover then remove one unit per second from "gameOverWaitTime"
        else {
            gameOverWaitTime -= Time.deltaTime;

            //if "gameOverWaitTIme" and the "Submit" button is pressed then reset the game.
            if (gameOverWaitTime <= 0) {
                gameOverUI.Find("Restart").gameObject.SetActive(true);

                if (Input.GetButtonDown("Submit")) {
                    isGameOver = false;
                    gameOverWaitTime = gameOverWaitTimeReset;
                    gameOverUI.gameObject.SetActive(false);
                    gameOverUI.Find("Restart").gameObject.SetActive(false);

                    foreach (Transform curTransform in playersMainParent.GetComponentsInChildren<Transform>(true)) {
                        if (curTransform != playersMainParent)
                            Destroy(curTransform.gameObject);
                    }

                    foreach (Transform curTransform in bulletsMainParent.GetComponentsInChildren<Transform>(true)) {
                        if (curTransform != bulletsMainParent)
                            Destroy(curTransform.gameObject);
                    }

                    foreach (Transform curTransform in terrainMainParent.GetComponentsInChildren<Transform>(true)) {
                        if (curTransform != terrainMainParent)
                            Destroy(curTransform.gameObject);
                    }

                    TerrianGenerator.instance.GenerateTerrian();
                }
            }
        }
    }

    private void SetupGame() {
        gameOverWaitTimeReset = gameOverWaitTime;

        TerrianGenerator.instance.GenerateTerrian();
        PlayBGM();

        //set up UI
        for (int player = 0; player < playersMainParent.childCount; player++) {
            Vector3 pos = new Vector3((Screen.width / 2) + (player * 175) - ((playersMainParent.childCount - 1) * 87.5f), Screen.height * 0.98f, 0);
            Instantiate(scoreUIPrefab, pos, Quaternion.identity, scoreUIParent);
            playerScore.Add(0);

            Text playerText = scoreUIParent.GetChild(player).GetComponent<Text>();
            playerText.text = "Player " + playersMainParent.GetChild(player).GetComponent<PlayerController>().playerID + ": " + playerScore[player];
            playerText.color = playersMainParent.GetChild(player).GetComponent<SpriteRenderer>().color;
        }
    }

    //sets GameOver Overlay to active which will show the text on screen, finds the PlayerWin text and sets it to the given playerID and color, and adds one to the living player's score
    //If the given playerID is -1 then make it a tie
    private void GameOver(int livingPlayer, Color color) {
        gameOverUI.gameObject.SetActive(true);

        Text playerWinText = gameOverUI.Find("PlayerWin").GetComponent<Text>();

        if (livingPlayer == -1) {
            playerWinText.text = "It's a tie";
            playerWinText.color = color;
            return;
        }

        Text playerText = scoreUIParent.GetChild(livingPlayer - 1).GetComponent<Text>();
        playerScore[livingPlayer - 1] += 1;
        playerText.text = "Player " + livingPlayer + ": " + playerScore[livingPlayer - 1];

        playerWinText.text = "Player " + livingPlayer + " won!";
        playerWinText.color = color;

        isGameOver = true;
    }

    private void PlayBGM() {
        // pick & play a random song from the array,
        // excluding song at index 0
        int n = Random.Range(1, BGM.Length);
        aS.clip = BGM[n];
        aS.PlayOneShot(aS.clip);
        // move picked song to index 0 so it's not picked next time
        BGM[n] = BGM[0];
        BGM[0] = aS.clip;
    }
}
