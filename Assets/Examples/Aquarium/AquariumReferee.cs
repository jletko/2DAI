using Common;

using UnityEngine;

namespace Examples.Aquarium
{
    public class AquariumReferee : RefereeBase
    {
        [SerializeField]
        private FishAgent fishAgent;
        [SerializeField]
        private GameObject food;
        [SerializeField]
        private Water water;

        private const float allowedRoundTime = 60;
        private BoxCollider2D waterCollider;

        public override void Restart()
        {
            base.Restart();

            fishAgent.EndEpisode();
            bool switchedWater = Random.value > 0.5f;
            bool sameWater = Random.value > 0.5f;
            fishAgent.Restart(GetRandomPositionInsideBoxCollider2D(waterCollider, 0.1f, switchedWater));
            if (sameWater)
            {
                food.transform.position = GetRandomPositionInsideBoxCollider2D(waterCollider, 0.1f, switchedWater);
            }
            else
            {
                food.transform.position = GetRandomPositionInsideBoxCollider2D(waterCollider, 0.1f, !switchedWater);
            }

            water.Restart();
        }

        private void Start()
        {
            waterCollider = water.GetComponent<BoxCollider2D>();
            Restart();
        }

        private void FixedUpdate()
        {
            if (TimeSinceLastRestart > allowedRoundTime)
            {
                fishAgent.AddReward(-1);
                Restart();
            }

            if (fishAgent.IsFoodFound)
            {
                fishAgent.AddReward(2);
                Restart();
            }

            if (fishAgent.IsCrashed)
            {
                fishAgent.AddReward(-0.001f);
            }

            fishAgent.AddReward(-0.0001f);
        }

        public static Vector2 GetRandomPositionInsideBoxCollider2D(BoxCollider2D boxCollider, float padding, bool leftHalf)
        {
            if (boxCollider == null)
            {
                Debug.LogError("BoxCollider2D is null!");
                return Vector2.zero;
            }

            // Get the bounds of the BoxCollider2D
            Bounds bounds = boxCollider.bounds;

            // Calculate half width and half height
            float halfWidth = bounds.size.x / 2f;
            float halfHeight = bounds.size.y / 2f;

            // For each half, the effective width is halfWidth, so clamp padding accordingly.
            float maxPaddingX = halfWidth;
            float maxPaddingY = halfHeight;
            padding = Mathf.Clamp(padding, 0, Mathf.Min(maxPaddingX, maxPaddingY));

            // Determine the x-axis midpoint (center of the collider)
            float midX = bounds.center.x;

            // Define the x-range based on the chosen half
            float minX, maxX;
            if (leftHalf)
            {
                // For the left half, x goes from the collider's left edge to the midpoint
                minX = bounds.min.x + padding;
                maxX = midX - padding;
            }
            else
            {
                // For the right half, x goes from the midpoint to the collider's right edge
                minX = midX + padding;
                maxX = bounds.max.x - padding;
            }

            // Generate a random x within the specified half and a random y within the full vertical range
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(bounds.min.y + padding, bounds.max.y - padding);

            return new Vector2(randomX, randomY);
        }

    }
}