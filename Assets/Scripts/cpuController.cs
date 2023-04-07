using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class cpuController : Agent {
    public float jumpForce = 1f;
    public CircleCollider2D col;

    private Rigidbody2D rb2d;
    private Transform closestPipe;
    private Transform closestPipePrev;
    private bool didJump = false;

    private void Start() {
        rb2d = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate() {
        if (Camera.main.orthographicSize - col.radius - Mathf.Abs(transform.localPosition.y) < 0f) {
            Death();
            return;
        }

        if (DidClosestPipeChange())
            AddReward(1f);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Death"))
            return;

        Death();
    }

    private void Jump() {
        if (didJump)
            return;

        didJump = true;
        rb2d.velocity = jumpForce * Vector3.up;
    }

    private void JumpReset() {
        didJump = false;
    }

    private void Death() {
        GameController.instance.playerWon = true;
        GameController.instance.GameOver(transform.parent.parent);
    }

    public void ResetAgent() {
        SetClosestPipe(null, true);
        SetReward(-1f);
        EndEpisode();
    }

    //Next pipe is always next in child index
    public void SetClosestPipe(Transform pipe, bool forceSetPrevPipe) {
        if (forceSetPrevPipe)
            closestPipePrev = pipe;

        if (GameController.instance.useCustomPipeColor && closestPipe != null && pipe != null) {
            foreach (SpriteRenderer sprite in closestPipe.gameObject.GetComponentsInChildren<SpriteRenderer>())
                sprite.color = pipe.gameObject.GetComponentInChildren<SpriteRenderer>().color;
        }
        else if (GameController.instance.useCustomPipeColor && closestPipe != null && closestPipePrev != null) {
            foreach (SpriteRenderer sprite in closestPipe.gameObject.GetComponentsInChildren<SpriteRenderer>())
                sprite.color = closestPipePrev.gameObject.GetComponentInChildren<SpriteRenderer>().color;
        }

        closestPipe = pipe;

        if (GameController.instance.useCustomPipeColor && closestPipe != null) {
            foreach (SpriteRenderer sprite in closestPipe.gameObject.GetComponentsInChildren<SpriteRenderer>())
                sprite.color = GameController.instance.customPipeColor;
        }
    }

    public Transform GetClosestPipe() {
        return closestPipe;
    }

    private bool DidClosestPipeChange() {
        if (closestPipe == null || closestPipePrev == null || closestPipe == closestPipePrev)
            return false;

        closestPipePrev = closestPipe;
        return true;
    }

    public override void OnEpisodeBegin() {
        transform.localPosition = Vector3.zero;
        rb2d.velocity = Vector2.zero;
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetButton("Jump") ? 1 : 0;
    }

    public override void OnActionReceived(ActionBuffers actions) {
        if (actions.DiscreteActions[0] == 1)
            Jump();
        else
            JumpReset();
    }

    public override void CollectObservations(VectorSensor sensor) {
        //This seems like a really bad way to make sure "closestPipe" is never null lol
        if (GetClosestPipe() == null)
            GameController.instance.UpdateWorld(transform.parent.parent);

        sensor.AddObservation(transform.localPosition.y);
        sensor.AddObservation(rb2d.velocity.y);
        sensor.AddObservation(new Vector2(GetClosestPipe().localPosition.x, GetClosestPipe().localPosition.y));
        //sensor.AddObservation(didJump);
        //sensor.AddObservation(currentMoveSpeed);
    }
}
