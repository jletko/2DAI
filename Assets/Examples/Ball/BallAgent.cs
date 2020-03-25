using MLAgents;
using UnityEngine;

namespace Examples.Ball
{
    public class BallAgent : Agent
    {
        [SerializeField] private float _maxForce;
        [SerializeField] private float _maxTorque;

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

        public override void InitializeAgent()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        public override void AgentAction(float[] vectorAction)
        {
            Power = 0;

            float forceCoeff = Mathf.Clamp(vectorAction[0], -1f, 1f);
            _rigidBody.AddForce(_maxForce * forceCoeff * transform.up);
            Power += Mathf.Abs(forceCoeff);

            float torqueCoeff = Mathf.Clamp(vectorAction[1], -1f, 1f);
            _rigidBody.AddTorque(_maxTorque * torqueCoeff);
            Power += Mathf.Abs(torqueCoeff);
        }

        public override float[] Heuristic()
        {
            return new[] { Input.GetAxis("Vertical"), -Input.GetAxis("Horizontal") };
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            IsCrashed = collision.collider.CompareTag(Tags.OBSTACLE);
            IsTouchingTarget = collision.collider.CompareTag(Tags.TARGET)
                               && collision.otherCollider.CompareTag(Tags.AGENT_HEAD);
        }

        private void OnCollisionExit2D()
        {
            IsCrashed = false;
            IsTouchingTarget = false;
        }
    }
}
