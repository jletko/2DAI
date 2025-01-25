using Unity.MLAgents;

using UnityEngine;

namespace Examples.Aquarium
{
    public class FishAgent : Agent
    {
        private float moveForceSize = 8f;
        private float torqueForceSize = 0.3f;
        private Rigidbody2D rb;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            if (IsInWater)
            {
                float forward = Input.GetAxis("Vertical");
                float turn = Input.GetAxis("Horizontal");

                Vector2 moveForce = transform.up * (forward * moveForceSize);
                float torqueForce = -turn * torqueForceSize;

                rb.AddForce(moveForce, ForceMode2D.Force);
                rb.AddTorque(torqueForce);
            }
        }

        public bool IsInWater { get; set; }
    }
}