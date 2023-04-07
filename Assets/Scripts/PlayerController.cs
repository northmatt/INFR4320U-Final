using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public float jumpForce = 1f;

    private CircleCollider2D col;
    private Rigidbody2D rb2d;

    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
    }

    private void Update() {
        if (Input.GetButtonDown("Jump"))
            Jump();
    }

    private void FixedUpdate() {
        if (Camera.main.orthographicSize - col.radius - Mathf.Abs(transform.localPosition.y) < 0f) {
            Death();
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Death"))
            return;

        Death();
    }

    private void Jump() {
        rb2d.velocity = jumpForce * Vector3.up;
    }

    private void Death() {
        GameController.instance.playerWon = false;
        GameController.instance.GameOver(transform.parent.parent);
    }

    public void ResetPlayer() {
        transform.localPosition = Vector3.zero;
        rb2d.velocity = Vector2.zero;
    }
}
