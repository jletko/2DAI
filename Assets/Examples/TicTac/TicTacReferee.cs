using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Examples.TicTac
{
    public class TicTacReferee : MonoBehaviour
    {
        [FormerlySerializedAs("_gym")] [SerializeField] private TicTacGym gym;
        [FormerlySerializedAs("_playerO")] [SerializeField] private TicTacPlayerAgent playerO;
        [FormerlySerializedAs("_playerX")] [SerializeField] private TicTacPlayerAgent playerX;
        [FormerlySerializedAs("_startingPlayer")] [SerializeField] private TicTacPlayerAgent startingPlayer;
        [FormerlySerializedAs("_oScoreText")] [SerializeField] private Text oScoreText;
        [FormerlySerializedAs("_xScoreText")] [SerializeField] private Text xScoreText;

        private bool _isRestartRequested;
        private float _oScore;
        private float _xScore;
        private byte[,] _byteGymCells;
        private PositionEvaluator _positionEvaluator;

        private void Start()
        {
            oScoreText.text = _oScore.ToString();
            xScoreText.text = _xScore.ToString();
            _byteGymCells = new byte[gym.GymSize, gym.GymSize];
            _positionEvaluator = new PositionEvaluator();

            _isRestartRequested = true;
        }

        private void FixedUpdate()
        {
            if (_isRestartRequested)
            {
                gym.Restart(startingPlayer.tag);
                SwitchCurrentPlayer();
                _isRestartRequested = false;
            }

            if (gym.IsTurnCompleted)
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
            float currentPlayerPositionDelta = _positionEvaluator.PositionStatsDelta[GetCellStateByte(gym.CurrentPlayer), 3];
            if (currentPlayerPositionDelta > 0)
            {
                GetPlayer(gym.CurrentPlayer).AddReward(currentPlayerPositionDelta * 0.01f);
            }

            float otherPlayerPositionDelta = _positionEvaluator.PositionStatsDelta[GetCellStateByte(Tags.GetOtherPlayer(gym.CurrentPlayer)), 3];
            if (otherPlayerPositionDelta < 0)
            {
                GetPlayer(gym.CurrentPlayer).AddReward(-otherPlayerPositionDelta * 0.01f);
            }
        }

        private void SwitchCurrentPlayer()
        {
            switch (gym.CurrentPlayer)
            {
                case Tags.PlayerO:
                    gym.CurrentPlayer = Tags.PlayerX;
                    break;
                case Tags.PlayerX:
                    gym.CurrentPlayer = Tags.PlayerO;
                    break;
            }
        }

        private void RequestTurn()
        {
            gym.IsTurnCompleted = false;

            switch (gym.CurrentPlayer)
            {
                case Tags.PlayerO:
                    playerO.RequestDecisionEx();
                    return;
                case Tags.PlayerX:
                    playerX.RequestDecisionEx();
                    return;
                default:
                    throw new Exception($"Unknown current player tag: {gym.CurrentPlayer}");
            }
        }

        private bool CheckWinner()
        {
            string winningPlayer = GetWinner(_positionEvaluator.PositionStats);

            switch (winningPlayer)
            {
                case Tags.PlayerO:
                    Done(1, -1);
                    _oScore++;
                    oScoreText.text = _oScore.ToString();
                    return true;
                case Tags.PlayerX:
                    Done(-1, 1);
                    _xScore++;
                    xScoreText.text = _xScore.ToString();
                    return true;
                default:
                    return false;
            }
        }

        private bool CheckDraw()
        {
            for (int i = 0; i < gym.GymSize; i++)
            {
                for (int j = 0; j < gym.GymSize; j++)
                {
                    if (gym.Cells[i, j].State == CellState.Empty)
                    {
                        return false;
                    }
                }
            }

            Done(0.5f, 0.5f);

            _oScore += 0.5f;
            oScoreText.text = _oScore.ToString();
            _xScore += 0.5f;
            xScoreText.text = _xScore.ToString();

            return true;
        }

        private void UpdatePositionStats()
        {
            UpdateByteGymCells();
            _positionEvaluator.UpdatePositionStats(_byteGymCells);
        }

        private void UpdateByteGymCells()
        {
            for (int i = 0; i < gym.GymSize; i++)
            {
                for (int j = 0; j < gym.GymSize; j++)
                {
                    _byteGymCells[i, j] = GetCellStateByte(gym.Cells[i, j].State);
                }
            }
        }

        private static string GetWinner(int[,] positionStats)
        {
            if (positionStats[GetCellStateByte(Tags.PlayerO), 5] > 0 && positionStats[GetCellStateByte(Tags.PlayerX), 5] > 0)
            {
                throw new Exception("Both players reported as winners which should not occure.");
            }

            if (positionStats[GetCellStateByte(Tags.PlayerO), 5] > 0)
            {
                return Tags.PlayerO;
            }

            if (positionStats[GetCellStateByte(Tags.PlayerX), 5] > 0)
            {
                return Tags.PlayerX;
            }

            return String.Empty;
        }

        private TicTacPlayerAgent GetPlayer(string playerTag)
        {
            switch (playerTag)
            {
                case Tags.PlayerO:
                    return playerO;
                case Tags.PlayerX:
                    return playerX;
                default:
                    throw new ArgumentException($"Invalid player tag: {gym.CurrentPlayer}");
            }
        }

        private static byte GetCellStateByte(string cellState)
        {
            switch (cellState)
            {
                case CellState.Empty:
                    return 0;
                case CellState.PlayerO:
                    return 1;
                case CellState.PlayerX:
                    return 2;
                default:
                    throw new ArgumentException($"Invalid CellState: {cellState}");
            }
        }

        private void Done(float rewardO, float rewardX)
        {
            playerO.SetReward(rewardO);
            playerO.EndEpisode();
            playerX.SetReward(rewardX);
            playerX.EndEpisode();
            _isRestartRequested = true;
        }
    }
}
