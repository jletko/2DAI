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
        private Water leftWater;
        [SerializeField]
        private Water rightWater;

        private const float allowedRoundTime = 120;
        private BoxCollider2D leftWaterCollider;
        private BoxCollider2D rightWaterCollider;

        public override void Restart()
        {
            base.Restart();

            fishAgent.EndEpisode();
            bool switchedWater = Random.value > 0.5f;
            bool sameWater = Random.value > 0.5f;
            BoxCollider2D fishWater;
            BoxCollider2D foodWater;
            if (sameWater)
            {
                fishWater = switchedWater ? rightWaterCollider : leftWaterCollider;
                foodWater = fishWater;
            }
            else
            {
                fishWater = switchedWater ? rightWaterCollider : leftWaterCollider;
                foodWater = switchedWater ? leftWaterCollider : rightWaterCollider;
            }

            fishAgent.Restart(GetRandomPositionInsideBoxCollider2D(fishWater, 0.1f));
            food.transform.position = GetRandomPositionInsideBoxCollider2D(foodWater, 0.1f);

            leftWater.Restart();
            rightWater.Restart();
        }

        private void Start()
        {
            leftWaterCollider = leftWater.GetComponent<BoxCollider2D>();
            rightWaterCollider = rightWater.GetComponent<BoxCollider2D>();

            Restart();
        }

        private void FixedUpdate()
        {
            if (TimeSinceLastRestart > allowedRoundTime)
            {
                fishAgent.AddReward(-1);
                Restart();
            }

            if (fishAgent.IsOutsideAquarium)
            {
                fishAgent.AddReward(-0.5f);
                Restart();
            }

            if (fishAgent.IsFoodFound)
            {
                fishAgent.AddReward(1);
                Restart();
            }

            if (fishAgent.IsCrashed)
            {
                fishAgent.AddReward(-0.001f);
            }
        }

        public static Vector2 GetRandomPositionInsideBoxCollider2D(BoxCollider2D boxCollider, float padding)
        {
            if (boxCollider == null)
            {
                Debug.LogError("BoxCollider2D is null!");
                return Vector2.zero;
            }

            // Get the bounds of the BoxCollider2D
            Bounds bounds = boxCollider.bounds;

            // Ensure padding does not exceed half the width/height
            float maxPaddingX = (bounds.size.x / 2);
            float maxPaddingY = (bounds.size.y / 2);

            padding = Mathf.Clamp(padding, 0, Mathf.Min(maxPaddingX, maxPaddingY));

            // Generate random position within the bounds with padding
            float randomX = Random.Range(bounds.min.x + padding, bounds.max.x - padding);
            float randomY = Random.Range(bounds.min.y + padding, bounds.max.y - padding);

            return new Vector2(randomX, randomY);
        }
    }
}