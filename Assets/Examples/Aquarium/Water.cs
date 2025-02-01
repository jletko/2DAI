using UnityEngine;

namespace Examples.Aquarium
{
    public class Water : MonoBehaviour
    {
        [SerializeField]
        private FishAgent fishAgent;

        private Rigidbody2D fishAgentRigidBody;
        private Collider2D fishCollider;
        private Collider2D waterTriggerArea;
        private float linearDamping = 5.0f;
        private float angularDamping = 70.0f;

        public void Restart()
        {
            fishAgent.WaterUpwardAcceleration = -0.96f * Physics2D.gravity;
            fishAgentRigidBody = fishAgent.GetComponent<Rigidbody2D>();
            waterTriggerArea = GetComponent<Collider2D>();
            fishCollider = fishAgent.GetComponent<Collider2D>();
        }

        private void FixedUpdate()
        {
            if (waterTriggerArea != null)
            {

                if (waterTriggerArea.bounds.Intersects(fishCollider.bounds))
                {
                    if (!fishAgent.IsInWater)
                    {
                        fishAgent.IsInWater = true;
                        fishAgentRigidBody.linearDamping = linearDamping;
                        fishAgentRigidBody.angularDamping = angularDamping;
                    }
                }
                else
                {
                    if (fishAgent.IsInWater)
                    {
                        fishAgent.IsInWater = false;
                        fishAgentRigidBody.linearDamping = 0;
                        fishAgentRigidBody.angularDamping = 0;
                    }
                }
            }
        }
    }
}
