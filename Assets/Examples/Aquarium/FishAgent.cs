using Unity.MLAgents;
using Unity.MLAgents.Actuators;

using UnityEngine;

namespace Examples.Aquarium
{
    public class FishAgent : Agent
    {
        private float maxMoveForce = 8f;
        private float maxTorqueForce = 0.3f;
        private Rigidbody2D rigidBody;
        public float stuckTimeThreshold = 2.0f; // Time in seconds before considering stuck
        private float collisionStartTime;
        private bool isCheckingStuck = false;

        void Start()
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            float[] actions = { Input.GetAxis("Vertical"), -Input.GetAxis("Horizontal") };

            for (int i = 0; i < actionsOut.ContinuousActions.Length; i++)
            {
                var continuousActions = actionsOut.ContinuousActions;
                continuousActions[i] = actions[i];
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            if (IsInWater)
            {
                float forceCoeff = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
                rigidBody.AddForce(maxMoveForce * forceCoeff * transform.up);

                float torqueCoeff = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
                rigidBody.AddTorque(maxTorqueForce * torqueCoeff);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag(Tags.AquariumTop))
            {
                collisionStartTime = Time.time;
                isCheckingStuck = true;
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.collider.CompareTag(Tags.AquariumTop))
            {
                if (isCheckingStuck && !IsStuckOnAquariumTop)
                {
                    float elapsedTime = Time.time - collisionStartTime;

                    if (elapsedTime >= stuckTimeThreshold)
                    {
                        IsStuckOnAquariumTop = true;
                        isCheckingStuck = false;
                    }
                }
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.collider.CompareTag(Tags.AquariumTop))
            {
                isCheckingStuck = false;
                IsStuckOnAquariumTop = false;
            }
        }

        public bool IsInWater { get; set; }

        public bool IsStuckOnAquariumTop { get; set; }
    }
}