using Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Examples.Chase
{
    public class ChaseReferee : RefereeBase
    {
        [FormerlySerializedAs("_hunters")] [SerializeField] private List<HunterAgent> hunters;
        [FormerlySerializedAs("_hunted")] [SerializeField] private HuntedAgent hunted;

        public override void Restart()
        {
            base.Restart();

            hunted.EndEpisode();
            hunted.Restart(GetRandomPositionInGym());

            hunters.ForEach(o => o.EndEpisode());
            hunters.ForEach(o => o.Restart(GetRandomPositionInGym()));
        }

        private void FixedUpdate()
        {
            ApplySpentEnergyPenaltyOnAgents(0.0001f);

            if (TimeSinceLastRestart > 30)
            {
                SetRewardAndRestart(-1f, 1f);
            }

            if (hunted.IsCatched)
            {
                SetRewardAndRestart(1f, -1f);
            }
        }

        private void SetRewardAndRestart(float huntersReward, float huntedReward)
        {
            SetReward(huntersReward, huntedReward);
            Restart();
        }

        private void AddReward(float huntersReward, float huntedReward)
        {
            hunters.ForEach(o => o.AddReward(huntersReward));
            hunted.AddReward(huntedReward);
        }

        private void SetReward(float huntersReward, float huntedReward)
        {
            hunters.ForEach(o => o.SetReward(huntersReward));
            hunted.SetReward(huntedReward);
        }

        private void ApplySpentEnergyPenaltyOnAgents(float coefficient)
        {
            hunters.ForEach(o => o.AddReward(-coefficient * o.Power));
            hunted.AddReward(-coefficient * hunted.Power);
        }
    }
}
