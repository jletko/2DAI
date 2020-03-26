using Common;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.Chase
{
    public class ChaseReferee : RefereeBase
    {
        [SerializeField] private List<HunterAgent> _hunters;
        [SerializeField] private HuntedAgent _hunted;

        public override void Restart()
        {
            base.Restart();

            _hunted.Done();
            _hunted.Restart(GetRandomPositionInGym());

            _hunters.ForEach(o => o.Done());
            _hunters.ForEach(o => o.Restart(GetRandomPositionInGym()));
        }

        private void FixedUpdate()
        {
            ApplySpentEnergyPenaltyOnAgents(0.0001f);

            if (TimeSinceLastRestart > 30)
            {
                SetRewardAndRestart(-1f, 1f);
            }

            if (_hunted.IsCatched)
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
            _hunters.ForEach(o => o.AddReward(huntersReward));
            _hunted.AddReward(huntedReward);
        }

        private void SetReward(float huntersReward, float huntedReward)
        {
            _hunters.ForEach(o => o.SetReward(huntersReward));
            _hunted.SetReward(huntedReward);
        }

        private void ApplySpentEnergyPenaltyOnAgents(float coefficient)
        {
            _hunters.ForEach(o => o.AddReward(-coefficient * o.Power));
            _hunted.AddReward(-coefficient * _hunted.Power);
        }
    }
}
