using Common;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Examples.Volleyball
{
    public class VolleyballReferee : RefereeBase
    {
        [FormerlySerializedAs("_leftPlayer")] [SerializeField] private PlayerAgent leftPlayer;
        [FormerlySerializedAs("_rightPlayer")] [SerializeField] private PlayerAgent rightPlayer;
        [FormerlySerializedAs("_ball")] [SerializeField] private Ball ball;
        [FormerlySerializedAs("_leftScoreText")] [SerializeField] private Text leftScoreText;
        [FormerlySerializedAs("_rightScoreText")] [SerializeField] private Text rightScoreText;
        [FormerlySerializedAs("_holdTimeout")] [SerializeField] private float holdTimeout;

        private float _holdTimeDelta;
        private float _lastTouchTime;
        private string _lastTouchPlayer;
        private string _oldBallCollisionTag;
        private int _leftScore;
        private int _rightScore;

        public override void Restart()
        {
            RestartRound();
        }

        private void Start()
        {
            RestartRound();
        }

        private void FixedUpdate()
        {
            leftPlayer.AddReward(0.001f);
            rightPlayer.AddReward(0.001f);

            leftPlayer.AddReward(-leftPlayer.Power * 0.001f);
            rightPlayer.AddReward(-rightPlayer.Power * 0.001f);

            if (leftPlayer.CollisionTag.Equals(Tags.Net) || leftPlayer.CollisionTag.Equals(Tags.Wall))
            {
                leftPlayer.AddReward(-0.1f);
            }

            if (rightPlayer.CollisionTag.Equals(Tags.Net) || rightPlayer.CollisionTag.Equals(Tags.Wall))
            {
                rightPlayer.AddReward(-0.1f);
            }

            switch (ball.CollisionTag)
            {
                case Tags.LeftFloor:
                    RestartRound(Tags.RightPlayer);
                    return;

                case Tags.RightFloor:
                    RestartRound(Tags.LeftPlayer);
                    return;

                case Tags.LeftPlayer:
                case Tags.RightPlayer:
                    if (CheckBallHold() || CheckBallDoubleTouch())
                    {
                        RestartRound(Tags.GetOtherPlayer(ball.CollisionTag));
                        return;
                    }
                    break;
            }

            _oldBallCollisionTag = ball.CollisionTag;
        }

        private bool CheckBallHold()
        {
            if (ball.CollisionTag == _oldBallCollisionTag)
            {
                _holdTimeDelta += Time.fixedDeltaTime;
                if (_holdTimeDelta > holdTimeout)
                {

                    return true;
                }
            }
            else
            {
                _holdTimeDelta = 0;
            }

            return false;
        }

        private bool CheckBallDoubleTouch()
        {
            if (ball.CollisionTag == _oldBallCollisionTag)
            {
                return false;
            }

            if (ball.CollisionTag != _lastTouchPlayer)
            {
                _lastTouchTime = Time.fixedTime;
                _lastTouchPlayer = ball.CollisionTag;
            }
            else if (Time.fixedTime - _lastTouchTime > holdTimeout)
            {
                return true;
            }

            return false;
        }

        private void RestartRound(string winningPlayer)
        {
            base.Restart();

            float playerSign = Tags.GetPlayerSign(winningPlayer);

            rightPlayer.AddReward(playerSign);
            leftPlayer.AddReward(-playerSign);

            leftPlayer.EndEpisode();
            rightPlayer.EndEpisode();

            if (winningPlayer.Equals(Tags.LeftPlayer))
            {
                leftPlayer.Restart();
                rightPlayer.RestartWithServing();
                _leftScore++;
                leftScoreText.text = _leftScore.ToString();
            }
            else
            {
                rightPlayer.Restart();
                leftPlayer.RestartWithServing();
                _rightScore++;
                rightScoreText.text = _rightScore.ToString();
            }

            _oldBallCollisionTag = string.Empty;
            _lastTouchPlayer = string.Empty;
            _holdTimeDelta = 0;
        }

        private void RestartRound()
        {
            base.Restart();

            leftPlayer.RestartWithServing();
            rightPlayer.Restart();
            _leftScore = 0;
            _rightScore = 0;
            leftScoreText.text = _leftScore.ToString();
            rightScoreText.text = _rightScore.ToString();
            rightPlayer.EndEpisode();
            leftPlayer.EndEpisode();

            _oldBallCollisionTag = string.Empty;
            _lastTouchPlayer = string.Empty;
            _holdTimeDelta = 0;
        }
    }
}
