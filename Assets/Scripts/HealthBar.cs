using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    private int maxHealth;
    private float health;
    private GUIStyle healthBar;
    private GUIStyle healthBackground;

    public void SetVars(int hM, float hp)
    {
        maxHealth = hM;
        health = hp;
    }

    private void Start()
    {
        //sets the color of each bar up
        healthBar = new GUIStyle();
        healthBackground = new GUIStyle();

        Texture2D greenTextureTemp = new Texture2D(1, 1);
        greenTextureTemp.SetPixel(0, 0, Color.green);
        greenTextureTemp.Apply();

        healthBar.normal.background = greenTextureTemp;

        Texture2D backgroundTextureTemp = new Texture2D(1, 1);
        backgroundTextureTemp.SetPixel(0, 0, new Color(0, 0, 0, 0.5f));
        backgroundTextureTemp.Apply();

        healthBackground.normal.background = backgroundTextureTemp;
    }

    void OnGUI()
    {
        //gets the position that the gui should draw the box
        Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, -transform.position.y + 0.75f, transform.position.z));
        Vector2 guiPosition = new Vector2(screenPos.x, screenPos.y);

        //draws the box
        GUI.Box(new Rect(guiPosition.x - (Screen.width / 50), guiPosition.y, Screen.width / 25, Screen.height / 100), GUIContent.none, healthBackground);
        GUI.Box(new Rect(guiPosition.x - (Screen.width / 50), guiPosition.y, (Screen.width / 25) * (health / maxHealth), Screen.height / 100), GUIContent.none, healthBar);
    }
}
