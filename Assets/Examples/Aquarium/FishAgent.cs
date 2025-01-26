using Unity.MLAgents;
using Unity.MLAgents.Actuators;

using UnityEngine;

namespace Examples.Aquarium
{
    public class FishAgent : Agent
    {
        private float maxMoveForce = 8f;
        private float maxTorqueForce = 1f;
        private Rigidbody2D rigidBody;

        public override void Initialize()
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        public void Restart(Vector2 startPosition)
        {
            rigidBody.angularVelocity = 0;
            rigidBody.linearVelocity = Vector2.zero;
            transform.position = startPosition;
            IsOutsideAquarium = false;
            IsInWater = false;
            IsFoodFound = false;
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

        public bool IsCrashed { get; private set; }

        public bool IsInWater { get; set; }

        public bool IsOutsideAquarium { get; set; }

        public bool IsFoodFound { get; private set; }

        public Vector2 WaterUpwardAcceleration { get; set; }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (!IsFoodFound && other.otherCollider.CompareTag(Tags.Mouth) && other.collider.CompareTag(Tags.Target))
            {
                IsFoodFound = true;
            }

            if (!IsCrashed && other.collider.CompareTag(Tags.Wall))
            {
                IsCrashed = true;
            }
        }

        private void OnCollisionExit2D()
        {
            IsCrashed = false;
        }

        private void FixedUpdate()
        {
            if (IsInWater)
            {
                rigidBody.AddForce(rigidBody.mass * WaterUpwardAcceleration);
            }
        }
    }
}