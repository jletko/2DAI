using System;
using UnityEngine;

namespace Examples.TicTac
{
    public class TicTacGym : MonoBehaviour
    {
        public GameObject CellPrefab;

        public Cell[,] Cells { get; private set; }

        public string CurrentPlayer { get; set; }
        public bool IsTurnCompleted { get; set; }
        public int GymSize { get; private set; }

        public void SetMarkForCurrentPlayer(int row, int column)
        {
            Cells[row, column].State = CurrentPlayer;

            IsTurnCompleted = true;
        }

        public void Restart(string startingPlayer)
        {
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    Cells[i, j].State = CellState.EMPTY;
                }
            }

            IsTurnCompleted = true;
            CurrentPlayer = startingPlayer;
        }

        private void Awake()
        {
            IsTurnCompleted = true;

            GymSize = (int)transform.localScale.x;
            Cells = new Cell[GymSize, GymSize];

            var cellsArray = GetComponentsInChildren<Cell>();

            int counter = 0;
            for (int i = 0; i < GymSize; i++)
            {
                for (int j = 0; j < GymSize; j++)
                {
                    Cell cell = cellsArray[counter];
                    cell.Clicked += OnCellClicked;
                    Cells[i, j] = cell;
                    counter++;
                }
            }
        }

        private void OnCellClicked(Cell cell, EventArgs e)
        {
            var currentPlayerGameObject = GameObject.FindGameObjectWithTag(CurrentPlayer);
            var currentPlayerAgent = currentPlayerGameObject.GetComponent<TicTacPlayerAgent>();

            if (!currentPlayerAgent.IsHeuristic || currentPlayerAgent.IsHeuristicEnabled)
            {
                return;
            }

            if (cell.State != CellState.EMPTY)
            {
                return;
            }

            cell.State = CurrentPlayer;
            IsTurnCompleted = true;
        }
    }
}
