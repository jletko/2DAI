using Unity.MLAgents;
using Unity.MLAgents.Actuators;

using UnityEngine;
using UnityEngine.Serialization;

namespace Examples.Chase
{
    public abstract class ChaseAgentBase : Agent
    {
        [FormerlySerializedAs("_maxMoveForce")] [SerializeField] private float maxMoveForce;
        [FormerlySerializedAs("_maxTorqueForce")] [SerializeField] private float maxTorqueForce;

        protected Rigidbody2D RigidBody;

        public bool IsCrashed { get; private set; }
        public float Power { get; private set; }

        public virtual void Restart(Vector2 startPosition)
        {
            RigidBody.angularVelocity = 0;
            RigidBody.velocity = Vector2.zero;
            transform.position = startPosition;
            IsCrashed = false;
        }

        public override void Initialize()
        {
            RigidBody = GetComponent<Rigidbody2D>();
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
            Power = 0;

            float forceCoeff = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
            RigidBody.AddForce(maxMoveForce * forceCoeff * transform.up);
            Power += Mathf.Abs(forceCoeff);

            float torqueCoeff = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
            RigidBody.AddTorque(maxTorqueForce * torqueCoeff);
            Power += Mathf.Abs(torqueCoeff);
        }

        protected virtual void OnCollisionStay2D(Collision2D collision)
        {
            IsCrashed = collision.collider.CompareTag(Tags.Obstacle);
        }

        protected virtual void OnCollisionExit2D()
        {
            IsCrashed = false;
        }
    }
}
