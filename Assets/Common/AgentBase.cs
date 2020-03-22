using MLAgents;
using UnityEngine;

namespace Common
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class AgentBase : Agent
    {
        private Rigidbody2D _rigidbody2D;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public float Speed => _rigidbody2D.velocity.magnitude;
    }
}
