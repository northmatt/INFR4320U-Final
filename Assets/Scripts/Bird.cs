using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {
    private Rigidbody2D rb2d;

    public void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Pipe"))
            return;

        Destroy(gameObject); 
        GameController.instance.GameOver();
    }
}
