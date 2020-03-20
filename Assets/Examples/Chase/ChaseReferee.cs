using Common;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.Chase
{
    public class ChaseReferee : Referee
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
            if (TimeSinceLastRestart > 15)
            {
                AddRewardAndRestart(-1, 1);
            }

            if (_hunted.IsCatched)
            {
                AddRewardAndRestart(1, -1);
            }

            if (_hunted.IsCrashed)
            {
                _hunted.AddReward(-0.002f);
            }

            foreach (HunterAgent hunter in _hunters)
            {
                if (hunter.IsCrashed)
                {
                    hunter.AddReward(-0.002f);
                }
            }
        }

        private void SetRewardAndRestart(float huntersReward, float huntedReward)
        {
            _hunters.ForEach(o => o.SetReward(huntersReward));
            _hunted.SetReward(huntedReward);

            Restart();
        }

        private void AddRewardAndRestart(float huntersReward, float huntedReward)
        {
            _hunters.ForEach(o => o.AddReward(huntersReward));
            _hunted.AddReward(huntedReward);

            Restart();
        }
    }
}
