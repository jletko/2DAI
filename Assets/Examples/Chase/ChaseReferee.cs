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

        private void AddSpeedRewardForAgents(float coefficient, float minspeed, float maxspeed, params ChaseAgentBase[] agents)
        {
            foreach (ChaseAgentBase agent in agents)
            {
                if (agent.Speed < minspeed)
                {
                    continue;
                }

                agent.AddReward(coefficient * Mathf.Clamp(agent.Speed, 0, maxspeed));
            }
        }
    }
}
