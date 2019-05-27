using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.TicTac
{
    public class TicTacReferee : MonoBehaviour
    {
        [SerializeField] private TicTacGym _gym;
        [SerializeField] private TicTacPlayerAgent _playerO;
        [SerializeField] private TicTacPlayerAgent _playerX;
        [SerializeField] private HeuristicPlayer _heuristicO;
        [SerializeField] private HeuristicPlayer _heuristicX;
        [SerializeField] private TicTacPlayerAgent _startingPlayer;
        [SerializeField] private Text _oScoreText;
        [SerializeField] private Text _xScoreText;

        private bool _isRestartRequested;
        private int _oScore;
        private int _xScore;

        private readonly List<byte[,]> _winKernels = new List<byte[,]>
        {
            new byte[,]
            {
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0},
                {1, 1, 1, 1, 1},
                {0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0}
            },
            new byte[,]
            {
                {0, 0, 1, 0, 0},
                {0, 0, 1, 0, 0},
                {0, 0, 1, 0, 0},
                {0, 0, 1, 0, 0},
                {0, 0, 1, 0, 0}
            },
            new byte[,]
            {
                {1, 0, 0, 0, 0},
                {0, 1, 0, 0, 0},
                {0, 0, 1, 0, 0},
                {0, 0, 0, 1, 0},
                {0, 0, 0, 0, 1}
            },
            new byte[,]
            {
                {0, 0, 0, 0, 1},
                {0, 0, 0, 1, 0},
                {0, 0, 1, 0, 0},
                {0, 1, 0, 0, 0},
                {1, 0, 0, 0, 0}
            }
        };

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

            CheckIfGameIsFinished();

            if (_gym.IsTurnCompleted)
            {
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
                    if (_playerO.IsEnabled)
                    {
                        _playerO.RequestDecision();
                        return;
                    }
                    if (_heuristicO.IsEnabled)
                    {
                        _heuristicO.RequestDecision();
                        return;
                    }

                    return;
                case Tags.PLAYER_X:
                    if (_playerX.IsEnabled)
                    {
                        _playerX.RequestDecision();
                        return;
                    }
                    if (_heuristicX.IsEnabled)
                    {
                        _heuristicX.RequestDecision();
                        return;
                    }

                    return;
                default:
                    throw new Exception($"Unknown current player tag: {_gym.CurrentPlayer}");
            }
        }

        private void CheckIfGameIsFinished()
        {
            string winningPlayerTag = GetWinningPlayerTag();

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

            for (int i = 0; i < _gym.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < _gym.Cells.GetLength(1); j++)
                {
                    if (_gym.Cells[i, j].State == CellState.EMPTY)
                    {
                        return;
                    }
                }
            }

            Done(0, 0);
        }

        private void Done(float rewardO, float rewardX)
        {
            _playerO.SetReward(rewardO);
            _playerO.Done();
            _playerX.SetReward(rewardX);
            _playerX.Done();
            _heuristicO.Done();
            _heuristicX.Done();
            _isRestartRequested = true;
        }

        private string GetWinningPlayerTag()
        {
            var winningPlayerTag = string.Empty;
            foreach (byte[,] winKernel in _winKernels)
            {
                winningPlayerTag = GetWinningPlayerTagByKernel(winKernel, _gym.Cells);
                if (!string.IsNullOrEmpty(winningPlayerTag))
                {
                    return winningPlayerTag;
                }
            }

            return winningPlayerTag;
        }

        private string GetWinningPlayerTagByKernel(byte[,] kernel, Cell[,] boardCells)
        {
            int boardColumnsCount = boardCells.GetLength(0);
            int boardRowsCount = boardCells.GetLength(1);

            int halfKernelColumnsCount = kernel.GetLength(0) / 2;
            int halfKernelRowsCount = kernel.GetLength(1) / 2;

            for (int i = 0; i < boardColumnsCount; i++)
            {
                for (int j = 0; j < boardRowsCount; j++)
                {
                    int sum = 0;
                    for (int k = -halfKernelColumnsCount; k <= halfKernelColumnsCount; k++)
                    {
                        for (int l = -halfKernelRowsCount; l <= halfKernelRowsCount; l++)
                        {
                            if (i + k < 0 || j + l < 0 || i + k >= boardColumnsCount || j + l >= boardRowsCount)
                            {
                                continue;
                            }

                            if (kernel[k + halfKernelColumnsCount, l + halfKernelRowsCount] != 1)
                            {
                                continue;
                            }

                            switch (boardCells[i + k, j + l].State)
                            {
                                case CellState.PLAYER_O:
                                    sum++;
                                    break;
                                case CellState.PLAYER_X:
                                    sum--;
                                    break;
                                default:
                                    sum = (int)Mathf.Sign(sum) * Mathf.Max(Mathf.Abs(sum) - 1, 0);
                                    break;
                            }
                        }
                    }

                    if (Mathf.Abs(sum) < kernel.GetLength(0))
                    {
                        continue;
                    }

                    return sum > 0 ? Tags.PLAYER_O : Tags.PLAYER_X;
                }
            }

            return string.Empty;
        }
    }
}