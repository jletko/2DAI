using Common;
using MLAgents;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.Ball
{
    public class BallAgent : Agent
    {
        [SerializeField] private float _maxMoveForce;
        [SerializeField] private float _maxTorqueForce;

        private Rigidbody2D _rigidBody;
        private RayPerception2D _rayPerception;

        public bool IsCrashed { get; private set; }
        public bool IsTouchingTarget { get; private set; }

        public void Restart(Vector2 startPosition)
        {
            _rigidBody.angularVelocity = 0;
            _rigidBody.velocity = Vector2.zero;
            transform.position = startPosition;
        }

        public override void InitializeAgent()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _rayPerception = GetComponentInChildren<RayPerception2D>();
        }

        public override void CollectObservations()
        {
            string[] detectableObjects = { Tags.OBSTACLE, Tags.TARGET };
            IEnumerable<float> perceptions = _rayPerception.Perceive(detectableObjects);
            AddVectorObs(perceptions);

            AddVectorObs(_rigidBody.velocity);
        }

        public override void AgentAction(float[] vectorAction)
        {
            _rigidBody.AddForce(_maxMoveForce * Mathf.Clamp(vectorAction[0], -1f, 1f) * transform.right);
            _rigidBody.AddTorque(_maxTorqueForce * Mathf.Clamp(vectorAction[1], -1f, 1f));
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