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
            AddSpeedRewardForAgents(0.0001f, 0.1f, 3, _hunted);
            AddSpeedRewardForAgents(0.0001f, 0.1f, 3, _hunters.ToArray());

            if (TimeSinceLastRestart > 30)
            {
                AddRewardAndRestart(-1f, 1f);
            }

            if (_hunted.IsCatched)
            {
                AddRewardAndRestart(1f, -1f);
            }
        }

        private void AddRewardAndRestart(float huntersReward, float huntedReward)
        {
            AddReward(huntersReward, huntedReward);
            Restart();
        }

        private void AddReward(float huntersReward, float huntedReward)
        {
            _hunters.ForEach(o => o.AddReward(huntersReward));
            _hunted.AddReward(huntedReward);
        }
    }
}
