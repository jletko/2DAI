using System.Collections.Generic;

using UnityEngine;

namespace Examples.Aquarium
{
    public class AccelerationEffector2D : MonoBehaviour
    {
        [SerializeField]
        private Vector2 acceleration;
        private float linearDamping = 3.0f; // Additional damping for smoother motion
        private float angularDamping = 40.0f;
        private List<Rigidbody2D> affectedBodies;
        private Collider2D triggerArea;

        private void Awake()
        {
            // Initialize the acceleration vector and list of affected bodies
            acceleration = -0.96f * Physics2D.gravity;
            affectedBodies = new List<Rigidbody2D>();
        }

        private void Start()
        {
            // Get the trigger area collider
            triggerArea = GetComponent<Collider2D>();

            // Ensure the collider is set as a trigger
            if (triggerArea != null && triggerArea.isTrigger)
            {
                // Find all Rigidbody2D objects in the scene using the non-obsolete method
                Rigidbody2D[] allRigidbodies = FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);

                foreach (Rigidbody2D rb in allRigidbodies)
                {
                    Collider2D rbCollider = rb.GetComponent<Collider2D>();
                    if (rbCollider != null && rb.gameObject.activeInHierarchy && triggerArea.bounds.Intersects(rbCollider.bounds))
                    {
                        affectedBodies.Add(rb);
                        rb.linearDamping = linearDamping;
                        rb.angularDamping = angularDamping;
                        if (rb.CompareTag(Tags.Fish))
                        {
                            rb.GetComponent<FishAgent>().IsInWater = true;
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("The Collider2D is either missing or not set as a trigger.");
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Rigidbody2D rb = collision.attachedRigidbody;
            if (rb != null && !affectedBodies.Contains(rb))
            {
                affectedBodies.Add(rb);
                rb.linearDamping = linearDamping;
                rb.angularDamping = angularDamping;
                if (rb.CompareTag(Tags.Fish))
                {
                    rb.GetComponent<FishAgent>().IsInWater = true;
                    Debug.Log("IsInWater = true");
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Rigidbody2D rb = collision.attachedRigidbody;
            if (rb != null)
            {
                affectedBodies.Remove(rb);
                rb.linearDamping = 0f; // Reset damping to default when leaving the area
                rb.angularDamping = 0f;
                if (rb.CompareTag(Tags.Fish))
                {
                    rb.GetComponent<FishAgent>().IsInWater = false;
                    Debug.Log("IsInWater = false");
                }
            }
        }

        private void FixedUpdate()
        {
            foreach (Rigidbody2D rb in affectedBodies)
            {
                if (rb != null)
                {
                    Vector2 force = rb.mass * acceleration;
                    rb.AddForce(force);
                }
            }
        }
    }
}
