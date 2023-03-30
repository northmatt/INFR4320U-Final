using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapSpawnData {
    public Color color;
    public GameObject gameObj;
    public int playerID = -1;
}

//use a picture to generate terrian
public class TerrianGenerator : MonoBehaviour {
    public static TerrianGenerator instance;

    public Transform spawnPlayers;
    public MapSpawnData[] ColorMappings;

    private Texture2D terrian;

    //creates an instance of the TerrianGenerator then generates the terrian
    private void Start() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }


    //Finds the color of the pixel given, if it's an alpha of 0 then it is air; so return, then look through all the colors in "ColorMappings" to see which color matches
    //once it finds a match then spawn the prefab given, if the playerID isn't at -1 then instantiate it into the "Players" gameobject, then set it's color and playerID to the given values
    void ReadPixel(int x, int y) {
        Color color = terrian.GetPixel(x, y);

        if (color.a == 0)
            return;

        foreach (MapSpawnData ColorMapping in ColorMappings) {
            if (ColorMapping.color != color)
                continue;

            Vector2 position = new Vector2(x, y);

            if (ColorMapping.playerID != -1) {
                Instantiate(ColorMapping.gameObj, position, Quaternion.identity, spawnPlayers);

                Transform currentPlayer = spawnPlayers.GetChild(spawnPlayers.childCount - 1);

                currentPlayer.transform.Rotate(Vector3.forward * Random.Range(0, 360));
                currentPlayer.gameObject.GetComponent<SpriteRenderer>().color = ColorMapping.color;
                currentPlayer.gameObject.GetComponent<PlayerController>().playerID = ColorMapping.playerID;
            }
            else {
                Instantiate(ColorMapping.gameObj, position, Quaternion.identity, transform);
            }
        }
    }

    //goes through all of the pixels in the image and gives them into "ReadPixel" then moves the players and terrian to the centre of the screen
    public void GenerateTerrian() {
        terrian = (Texture2D)Resources.Load("Maps/Map" + Random.Range(0, 5), typeof(Texture2D));

        for (int x = 0; x < terrian.width; x++) {
            for (int y = 0; y < terrian.height; y++) {
                ReadPixel(x, y);
            }
        }

        transform.position -= new Vector3(terrian.width / 2, terrian.height / 2, 0);
        spawnPlayers.position -= new Vector3(terrian.width / 2, terrian.height / 2, 0);
    }
}
