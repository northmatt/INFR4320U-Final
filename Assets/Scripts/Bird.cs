using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class Bird : Agent {
    public float jumpForce = 1f;
    public float currentMoveSpeed = 0f;
    public CircleCollider2D col;

    protected Rigidbody2D rb2d;
    protected bool didJump = false;

    private Transform closestPipe;
    private Transform closestPipePrev;

    virtual public void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
    }

    virtual public void FixedUpdate() {
        if (Camera.main.orthographicSize - col.radius - Mathf.Abs(transform.localPosition.y) < 0f)
            Death();
    }

    public void SetClosestPipe(Transform pipe, bool forceSetPrevPipe) {
        if (forceSetPrevPipe)
            closestPipePrev = pipe;

        if (closestPipe != null && pipe != null) {
            foreach (SpriteRenderer sprite in closestPipe.gameObject.GetComponentsInChildren<SpriteRenderer>())
                sprite.color = pipe.gameObject.GetComponentInChildren<SpriteRenderer>().color;
        }
        else if (closestPipe != null && closestPipePrev != null) {
            foreach (SpriteRenderer sprite in closestPipe.gameObject.GetComponentsInChildren<SpriteRenderer>())
                sprite.color = closestPipePrev.gameObject.GetComponentInChildren<SpriteRenderer>().color;
        }

        closestPipe = pipe;

        if (closestPipe != null) {
            foreach (SpriteRenderer sprite in closestPipe.gameObject.GetComponentsInChildren<SpriteRenderer>())
                sprite.color = GameController.instance.customPipeColor;
        }
    }

    public Transform GetClosestPipe() {
        return closestPipe;
    }

    protected bool DidClosestPipeChange() {
        if (closestPipe == null || closestPipePrev == null || closestPipe == closestPipePrev)
            return false;

        closestPipePrev = closestPipe;
        return true;
    }

    public void Jump() {
        if (didJump)
            return;

        didJump = true;
        rb2d.velocity = jumpForce * Vector3.up;
    }

    public void JumpReset() {
        didJump = false;
    }

    virtual public void Death() {
        SetClosestPipe(null, true);
        GameController.instance.GameOver(this);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Death"))
            return;

        Death();
    }
}
