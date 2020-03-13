using Common;
using UnityEngine;

namespace Examples.Ball
{
    public class BallReferee : BaseReferee
    {
        [SerializeField] private BallAgent _ballAgent;
        [SerializeField] private Transform _target;

        public override void Restart()
        {
            Restart(0);
        }

        private void Start()
        {
            _target.position = GetRandomPositionInGym();
        }

        private void FixedUpdate()
        {
            _ballAgent.AddReward(-0.001f);

            if (TimeSinceLastRestart > 25)
            {
                Restart(-1);
            }

            if (_ballAgent.IsTouchingTarget)
            {
                Restart(1);
            }

            if (_ballAgent.IsCrashed)
            {
                _ballAgent.AddReward(-0.01f);
            }
        }

        private void Restart(float reward)
        {
            base.Restart();
            _ballAgent.SetReward(reward);
            _ballAgent.Done();
            _ballAgent.Restart(transform.position);
            _target.position = GetRandomPositionInGym();
        }
    }
}
