using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class cpuController : Bird {
    override public void Start() {
        base.Start();
    }

    void Update() {
        GameController.instance.UpdateWorld(this);
    }

    public override void FixedUpdate() {
        base.FixedUpdate();

        if (DidClosestPipeChange())
            AddReward(1f);
    }

    public override void Death() {
        base.Death();

        SetReward(-1f);
        EndEpisode();
    }

    public override void OnEpisodeBegin() {
        transform.localPosition = Vector3.zero;
        rb2d.velocity = Vector2.zero;
        currentMoveSpeed = GameController.instance.moveSpeedStart;
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetButton("Jump") ? 1 : 0;
    }

    public override void OnActionReceived(ActionBuffers actions) {
        if (actions.DiscreteActions[0] == 1)
            base.Jump();
        else
            base.JumpReset();
    }

    public override void CollectObservations(VectorSensor sensor) {
        //This seems like a really bad way to make sure "closestPipe" is never null lol
        if (GetClosestPipe() == null)
            GameController.instance.UpdateWorld(this);

        sensor.AddObservation(transform.localPosition.y);
        sensor.AddObservation(rb2d.velocity.y);
        sensor.AddObservation(new Vector2(GetClosestPipe().localPosition.x, GetClosestPipe().localPosition.y));
        //sensor.AddObservation(didJump);
        //sensor.AddObservation(currentMoveSpeed);
    }
}
