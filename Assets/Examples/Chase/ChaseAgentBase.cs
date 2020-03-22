﻿using Common;
using UnityEngine;

namespace Examples.Chase
{
    public abstract class ChaseAgentBase : AgentBase
    {
        [SerializeField] private float _maxMoveForce;
        [SerializeField] private float _maxTorqueForce;

        protected Rigidbody2D RigidBody;

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
        }

        public override float[] Heuristic()
        {
            return new[] { Input.GetAxis("Vertical"), -Input.GetAxis("Horizontal") };
        }

        public override void AgentAction(float[] vectorAction)
        {
            RigidBody.AddForce(_maxMoveForce * Mathf.Clamp(vectorAction[0], -1f, 1f) * transform.up);
            RigidBody.AddTorque(_maxTorqueForce * Mathf.Clamp(vectorAction[1], -1f, 1f));
        }

        protected virtual void OnCollisionStay2D(Collision2D collision)
        {
            IsCrashed = collision.collider.CompareTag(Tags.OBSTACLE);
        }

        protected virtual void OnCollisionExit2D()
        {
            IsCrashed = false;
        }
    }
}
