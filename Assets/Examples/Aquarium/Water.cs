using UnityEngine;

namespace Examples.Aquarium
{
    public class Water : MonoBehaviour
    {
        [SerializeField]
        private FishAgent fishAgent;

        private Rigidbody2D fishAgentRigidBody;
        private Collider2D waterTriggerArea;
        private float linearDamping = 3.0f;
        private float angularDamping = 70.0f;

        public void Restart()
        {
            fishAgent.WaterUpwardAcceleration = -0.96f * Physics2D.gravity;
            fishAgentRigidBody = fishAgent.GetComponent<Rigidbody2D>();
            waterTriggerArea = GetComponent<Collider2D>();
            if (waterTriggerArea != null && waterTriggerArea.isTrigger)
            {
                Collider2D fishCollider = fishAgent.GetComponent<Collider2D>();
                if (waterTriggerArea.bounds.Intersects(fishCollider.bounds))
                {
                    fishAgent.IsInWater = true;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (fishAgent.CompareTag(collision.tag))
            {
                fishAgentRigidBody.linearDamping = linearDamping;
                fishAgentRigidBody.angularDamping = angularDamping;
                fishAgent.IsInWater = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (fishAgent.CompareTag(collision.tag))
            {
                fishAgentRigidBody.linearDamping = 0;
                fishAgentRigidBody.angularDamping = 0;
                fishAgent.IsInWater = false;
            }
        }
    }
}
