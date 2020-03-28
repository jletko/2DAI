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
        private float _oScore;
        private float _xScore;
        private byte[,] _byteGymCells;
        private PositionEvaluator _positionEvaluator;

        private void Start()
        {
            _oScoreText.text = _oScore.ToString();
            _xScoreText.text = _xScore.ToString();
            _byteGymCells = new byte[_gym.GymSize, _gym.GymSize];
            _positionEvaluator = new PositionEvaluator();

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
                UpdatePositionStats();
                if (CheckWinner() || CheckDraw())
                {
                    return;
                }

                AddPositionRewards();
                SwitchCurrentPlayer();
                RequestTurn();
            }
        }

        private void AddPositionRewards()
        {
            float currentPlayerPositionDelta = _positionEvaluator.PositionStatsDelta[GetCellStateByte(_gym.CurrentPlayer), 3];
            if (currentPlayerPositionDelta > 0)
            {
                GetPlayer(_gym.CurrentPlayer).AddReward(currentPlayerPositionDelta * 0.01f);
            }

            float otherPlayerPositionDelta = _positionEvaluator.PositionStatsDelta[GetCellStateByte(Tags.GetOtherPlayer(_gym.CurrentPlayer)), 3];
            if (otherPlayerPositionDelta < 0)
            {
                GetPlayer(_gym.CurrentPlayer).AddReward(-otherPlayerPositionDelta * 0.01f);
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

        private bool CheckWinner()
        {
            string winningPlayer = GetWinner(_positionEvaluator.PositionStats);

            switch (winningPlayer)
            {
                case Tags.PLAYER_O:
                    Done(1, -1);
                    _oScore++;
                    _oScoreText.text = _oScore.ToString();
                    return true;
                case Tags.PLAYER_X:
                    Done(-1, 1);
                    _xScore++;
                    _xScoreText.text = _xScore.ToString();
                    return true;
                default:
                    return false;
            }
        }

        private bool CheckDraw()
        {
            for (int i = 0; i < _gym.GymSize; i++)
            {
                for (int j = 0; j < _gym.GymSize; j++)
                {
                    if (_gym.Cells[i, j].State == CellState.EMPTY)
                    {
                        return false;
                    }
                }
            }

            Done(0.5f, 0.5f);

            _oScore += 0.5f;
            _oScoreText.text = _oScore.ToString();
            _xScore += 0.5f;
            _xScoreText.text = _xScore.ToString();

            return true;
        }

        private void UpdatePositionStats()
        {
            UpdateByteGymCells();
            _positionEvaluator.UpdatePositionStats(_byteGymCells);
        }

        private void UpdateByteGymCells()
        {
            for (int i = 0; i < _gym.GymSize; i++)
            {
                for (int j = 0; j < _gym.GymSize; j++)
                {
                    _byteGymCells[i, j] = GetCellStateByte(_gym.Cells[i, j].State);
                }
            }
        }

        private static string GetWinner(int[,] positionStats)
        {
            if (positionStats[GetCellStateByte(Tags.PLAYER_O), 5] > 0 && positionStats[GetCellStateByte(Tags.PLAYER_X), 5] > 0)
            {
                throw new Exception("Both players reported as winners which should not occure.");
            }

            if (positionStats[GetCellStateByte(Tags.PLAYER_O), 5] > 0)
            {
                return Tags.PLAYER_O;
            }

            if (positionStats[GetCellStateByte(Tags.PLAYER_X), 5] > 0)
            {
                return Tags.PLAYER_X;
            }

            return String.Empty;
        }

        private TicTacPlayerAgent GetPlayer(string playerTag)
        {
            switch (playerTag)
            {
                case Tags.PLAYER_O:
                    return _playerO;
                case Tags.PLAYER_X:
                    return _playerX;
                default:
                    throw new ArgumentException($"Invalid player tag: {_gym.CurrentPlayer}");
            }
        }

        private static byte GetCellStateByte(string cellState)
        {
            switch (cellState)
            {
                case CellState.EMPTY:
                    return 0;
                case CellState.PLAYER_O:
                    return 1;
                case CellState.PLAYER_X:
                    return 2;
                default:
                    throw new ArgumentException($"Invalid CellState: {cellState}");
            }
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
