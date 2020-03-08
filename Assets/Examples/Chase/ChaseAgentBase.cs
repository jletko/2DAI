using Common;
using MLAgents;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.Chase
{
    public abstract class ChaseAgentBase : Agent
    {
        [SerializeField] private float _maxMoveForce;
        [SerializeField] private float _maxTorqueForce;

        protected Rigidbody2D RigidBody;
        protected RayPerception2D RayPerception;

        public bool IsCrashed { get; private set; }

        public virtual void Restart(Vector2 startPosition)
        {
            RigidBody.angularVelocity = 0;
            RigidBody.velocity = Vector2.zero;
            transform.position = startPosition;
            IsCrashed = false;
        }

        public override void InitializeAgent()
        {
            RigidBody = GetComponent<Rigidbody2D>();
            RayPerception = GetComponentInChildren<RayPerception2D>();
        }

        public override void AgentAction(float[] vectorAction)
        {
            RigidBody.AddForce(_maxMoveForce * Mathf.Clamp(vectorAction[0], -1f, 1f) * transform.right);
            RigidBody.AddTorque(_maxTorqueForce * Mathf.Clamp(vectorAction[1], -1f, 1f));
        }

        public override void CollectObservations()
        {
            string[] detectableObjects = GetDetectableObjectTags();
            IEnumerable<float> perceptions = RayPerception.Perceive(detectableObjects);
            AddVectorObs(perceptions);

            AddVectorObs(RigidBody.velocity);
        }

        protected virtual void OnCollisionStay2D(Collision2D collision)
        {
            IsCrashed = collision.collider.CompareTag(Tags.OBSTACLE);
        }

        protected virtual void OnCollisionExit2D()
        {
            IsCrashed = false;
        }

        protected abstract string[] GetDetectableObjectTags();
    }
}
