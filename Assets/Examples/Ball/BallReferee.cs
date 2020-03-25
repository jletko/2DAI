using Common;
using UnityEngine;

namespace Examples.Ball
{
    public class BallReferee : RefereeBase
    {
        [SerializeField] private BallAgent _ballAgent;
        [SerializeField] private Transform _target;

        public override void Restart()
        {
            base.Restart();
            _ballAgent.Done();
            _ballAgent.Restart(transform.position);
            _target.position = GetRandomPositionInGym();
        }

        private void Start()
        {
            _target.position = GetRandomPositionInGym();
        }

        private void FixedUpdate()
        {
            _ballAgent.AddReward(-0.001f);
            _ballAgent.AddReward(-_ballAgent.Power * 0.001f);

            if (TimeSinceLastRestart > 15)
            {
                _ballAgent.SetReward(-1);
                Restart();
            }

            if (_ballAgent.IsTouchingTarget)
            {
                _ballAgent.SetReward(1);
                Restart();
            }

            if (_ballAgent.IsCrashed)
            {
                _ballAgent.AddReward(-0.1f);
            }
        }
    }
}
