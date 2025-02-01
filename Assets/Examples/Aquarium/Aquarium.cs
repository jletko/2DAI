using UnityEngine;

namespace Examples.Aquarium
{
    public class Aquarium : MonoBehaviour
    {
        [SerializeField]
        private FishAgent fishAgent;
        private float IsOutsideWaterStartTime;
        private const float IsOutsideWaterLimitTime = 2;
        private bool WasInWater = true;

        private void FixedUpdate()
        {
            CheckOutsideWaterTime();
        }

        private void CheckOutsideWaterTime()
        {
            if (fishAgent.IsOutsideAquarium)
            {
                return;
            }

            if (!fishAgent.IsInWater && WasInWater)
            {
                IsOutsideWaterStartTime = Time.fixedTime;
            }

            if (!fishAgent.IsInWater)
            {
                var outOfWaterTime = Time.fixedTime - IsOutsideWaterStartTime;
                if (outOfWaterTime > IsOutsideWaterLimitTime)
                {
                    fishAgent.IsOutsideAquarium = true;
                }
            }

            WasInWater = fishAgent.IsInWater;
        }
    }
}
