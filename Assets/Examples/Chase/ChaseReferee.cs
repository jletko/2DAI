using Common;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.Chase
{
    public class ChaseReferee : BaseReferee
    {
        [SerializeField] private List<HunterAgent> _hunters;
        [SerializeField] private HuntedAgent _hunted;

        public override void Restart()
        {
            Restart(0, 0);
        }

        private void FixedUpdate()
        {
            if (TimeSinceLastRestart > 15)
            {
                Restart(-1, 1);
            }

            if (_hunted.IsCatched)
            {
                Restart(1, -1);
            }

            if (_hunted.IsCrashed)
            {
                RestartHunted(-1);
            }

            foreach (HunterAgent hunter in _hunters)
            {
                if (hunter.IsCrashed)
                {
                    RestartHunter(hunter, -1);
                }
            }
        }

        private void RestartHunter(HunterAgent hunter, float hunterReward)
        {
            hunter.SetReward(hunterReward);
            hunter.Done();
            hunter.Restart(GetRandomPositionInGym());
        }

        private void RestartHunted(float huntedReward)
        {
            _hunted.SetReward(huntedReward);
            _hunted.Done();
            _hunted.Restart(GetRandomPositionInGym());
        }

        private void Restart(float huntersReward, float huntedReward)
        {
            base.Restart();
            _hunters.ForEach(o => RestartHunter(o, huntersReward));
            RestartHunted(huntedReward);
        }
    }
}