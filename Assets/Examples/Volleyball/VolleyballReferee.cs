using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.Volleyball
{
    public class VolleyballReferee : Referee
    {
        [SerializeField] private PlayerAgent _leftPlayer;
        [SerializeField] private PlayerAgent _rightPlayer;
        [SerializeField] private Ball _ball;
        [SerializeField] private Text _leftScoreText;
        [SerializeField] private Text _rightScoreText;
        [SerializeField] private float _holdTimeout;

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
            if (_leftPlayer.CollisionTag.Equals(Tags.NET) || _leftPlayer.CollisionTag.Equals(Tags.WALL))
            {
                RestartRound(Tags.RIGHT_PLAYER);
                return;
            }

            if (_rightPlayer.CollisionTag.Equals(Tags.NET) || _rightPlayer.CollisionTag.Equals(Tags.WALL))
            {
                RestartRound(Tags.LEFT_PLAYER);
                return;
            }

            switch (_ball.CollisionTag)
            {
                case Tags.LEFT_FLOOR:
                    RestartRound(Tags.RIGHT_PLAYER);
                    return;

                case Tags.RIGHT_FLOOR:
                    RestartRound(Tags.LEFT_PLAYER);
                    return;

                case Tags.LEFT_PLAYER:
                case Tags.RIGHT_PLAYER:
                    if (CheckBallHoldAndTouch())
                    {
                        return;
                    }
                    break;
            }

            _oldBallCollisionTag = _ball.CollisionTag;
        }

        private bool CheckBallHoldAndTouch()
        {
            if (_ball.CollisionTag == _oldBallCollisionTag)
            {
                _holdTimeDelta += Time.fixedDeltaTime;
                if (_holdTimeDelta > _holdTimeout)
                {
                    RestartRound(Tags.GetOtherPlayer(_ball.CollisionTag));
                    return true;
                }
            }
            else
            {
                _holdTimeDelta = 0;

                if (_ball.CollisionTag != _lastTouchPlayer)
                {
                    _lastTouchTime = Time.fixedTime;
                    _lastTouchPlayer = _ball.CollisionTag;
                }
                else if (Time.fixedTime - _lastTouchTime > _holdTimeout)
                {
                    RestartRound(Tags.GetOtherPlayer(_ball.CollisionTag));
                    return true;
                }
            }
            return false;
        }

        private void RestartRound(string winningPlayer)
        {
            base.Restart();

            float playerSign = Tags.GetPlayerSign(winningPlayer);

            _rightPlayer.SetReward(playerSign);
            _leftPlayer.SetReward(-playerSign);

            _leftPlayer.Done();
            _rightPlayer.Done();

            if (winningPlayer.Equals(Tags.LEFT_PLAYER))
            {
                _leftPlayer.Restart();
                _rightPlayer.RestartWithServing();
                _leftScore++;
                _leftScoreText.text = _leftScore.ToString();
            }
            else
            {
                _rightPlayer.Restart();
                _leftPlayer.RestartWithServing();
                _rightScore++;
                _rightScoreText.text = _rightScore.ToString();
            }

            _oldBallCollisionTag = string.Empty;
            _lastTouchPlayer = string.Empty;
            _holdTimeDelta = 0;
        }

        private void RestartRound()
        {
            base.Restart();

            _leftPlayer.RestartWithServing();
            _rightPlayer.Restart();
            _leftScore = 0;
            _rightScore = 0;
            _leftScoreText.text = _leftScore.ToString();
            _rightScoreText.text = _rightScore.ToString();
            _rightPlayer.Done();
            _leftPlayer.Done();

            _oldBallCollisionTag = string.Empty;
            _lastTouchPlayer = string.Empty;
            _holdTimeDelta = 0;
        }
    }
}
