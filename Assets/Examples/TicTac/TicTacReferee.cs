using System;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.TicTac
{
    public class TicTacReferee : MonoBehaviour
    {
        [SerializeField] private TicTacGym _gym;
        [SerializeField] private TicTacPlayerAgent _playerO;
        [SerializeField] private TicTacPlayerAgent _playerX;
        [SerializeField] private TicTacPlayerAgent _startingPlayer;
        [SerializeField] private Text _oScoreText;
        [SerializeField] private Text _xScoreText;

        private bool _isRestartRequested;
        private int _oScore;
        private int _xScore;

        private void Start()
        {
            _oScoreText.text = _oScore.ToString();
            _xScoreText.text = _xScore.ToString();

            _isRestartRequested = true;
        }

        private void FixedUpdate()
        {
            if (_isRestartRequested)
            {
                _gym.Restart(_startingPlayer.tag);
                SwitchCurrentPlayer();
                _isRestartRequested = false;
            }

            if (_gym.IsTurnCompleted)
            {
                CheckIfGameIsFinished();
                SwitchCurrentPlayer();
                RequestTurn();
            }
        }

        private void SwitchCurrentPlayer()
        {
            switch (_gym.CurrentPlayer)
            {
                case Tags.PLAYER_O:
                    _gym.CurrentPlayer = Tags.PLAYER_X;
                    break;
                case Tags.PLAYER_X:
                    _gym.CurrentPlayer = Tags.PLAYER_O;
                    break;
            }
        }

        private void RequestTurn()
        {
            _gym.IsTurnCompleted = false;

            switch (_gym.CurrentPlayer)
            {
                case Tags.PLAYER_O:
                    _playerO.RequestDecisionEx();
                    return;
                case Tags.PLAYER_X:
                    _playerX.RequestDecisionEx();
                    return;
                default:
                    throw new Exception($"Unknown current player tag: {_gym.CurrentPlayer}");
            }
        }

        private void CheckIfGameIsFinished()
        {
            string winningPlayerTag = PositionEvaluator.GetWinningPlayerTag(_gym.Cells);

            switch (winningPlayerTag)
            {
                case Tags.PLAYER_O:
                    Done(1, -1);
                    _oScore++;
                    _oScoreText.text = _oScore.ToString();
                    return;
                case Tags.PLAYER_X:
                    Done(-1, 1);
                    _xScore++;
                    _xScoreText.text = _xScore.ToString();
                    return;
            }

            for (int i = 0; i < _gym.GymSize; i++)
            {
                for (int j = 0; j < _gym.GymSize; j++)
                {
                    if (_gym.Cells[i, j].State == CellState.EMPTY)
                    {
                        return;
                    }
                }
            }

            Done(1, 1);
        }

        private void Done(float rewardO, float rewardX)
        {
            _playerO.SetReward(rewardO);
            _playerO.EndEpisode();
            _playerX.SetReward(rewardX);
            _playerX.EndEpisode();
            _isRestartRequested = true;
        }
    }
}
