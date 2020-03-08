using MLAgents;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Examples.TicTac
{
    public class TicTacPlayerAgent : Agent
    {
        [SerializeField] private TicTacGym _gym;

        private string PlayerTag => gameObject.tag;
        private readonly List<int> _myLastMaskedActions = new List<int>();

        //TODO: fix
        public bool IsEnabled => true;

        public override void CollectObservations()
        {
            _myLastMaskedActions.Clear();

            int index = 0;
            for (int i = 0; i < _gym.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < _gym.Cells.GetLength(1); j++)
                {
                    AddVectorObs(_gym.Cells[i, j].State.Equals(CellState.EMPTY));
                    if (PlayerTag.Equals(CellState.PLAYER_O))
                    {
                        AddVectorObs(_gym.Cells[i, j].State.Equals(CellState.PLAYER_O));
                        AddVectorObs(_gym.Cells[i, j].State.Equals(CellState.PLAYER_X));
                    }
                    else
                    {
                        AddVectorObs(_gym.Cells[i, j].State.Equals(CellState.PLAYER_X));
                        AddVectorObs(_gym.Cells[i, j].State.Equals(CellState.PLAYER_O));
                    }

                    if (!_gym.Cells[i, j].State.Equals(CellState.EMPTY))
                    {
                        _myLastMaskedActions.Add(index);
                    }

                    index++;
                }
            }

            SetActionMask(0, _myLastMaskedActions);
        }

        public override void AgentAction(float[] vectorAction)
        {
            if (vectorAction == null)
            {
                return;
            }

            if (vectorAction.Length != 1)
            {
                throw new Exception($"Incorrect actions count. Expected: 1, actual: {vectorAction.Length}");
            }

            int cellIndex = Mathf.FloorToInt(vectorAction[0]);
            if (cellIndex < 0)
            {
                return;
            }

            _gym.SetMarkForCurrentPlayer(cellIndex / _gym.Cells.GetLength(0), cellIndex % _gym.Cells.GetLength(0));
        }
    }
}