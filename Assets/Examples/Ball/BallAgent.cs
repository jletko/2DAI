using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.Serialization;

namespace Examples.Ball
{
    public class BallAgent : Agent
    {
        [FormerlySerializedAs("_maxForce")] [SerializeField] private float maxForce;
        [FormerlySerializedAs("_maxTorque")] [SerializeField] private float maxTorque;

        private Rigidbody2D _rigidBody;

        public bool IsCrashed { get; private set; }
        public bool IsTouchingTarget { get; private set; }
        public float Power { get; private set; }

        public void Restart(Vector2 startPosition)
        {
            _rigidBody.angularVelocity = 0;
            _rigidBody.velocity = Vector2.zero;
            transform.position = startPosition;
        }

        public override void Initialize()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            Power = 0;

            float forceCoeff = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
            _rigidBody.AddForce(maxForce * forceCoeff * transform.up);
            Power += Mathf.Abs(forceCoeff);

            float torqueCoeff = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
            _rigidBody.AddTorque(maxTorque * torqueCoeff);
            Power += Mathf.Abs(torqueCoeff);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            float[] actions = { Input.GetAxis("Vertical"), -Input.GetAxis("Horizontal") };

            for (int i = 0; i < actionsOut.ContinuousActions.Length; i++)
            {
                //actionsOut.ContinuousActions. = actions[i];
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            IsCrashed = collision.collider.CompareTag(Tags.Obstacle);
            IsTouchingTarget = collision.collider.CompareTag(Tags.Target)
                               && collision.otherCollider.CompareTag(Tags.AgentHead);
        }

        private void OnCollisionExit2D()
        {
            IsCrashed = false;
            IsTouchingTarget = false;
        }
    }
}
