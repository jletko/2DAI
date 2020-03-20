using System.Collections.Generic;
using UnityEngine;

namespace Examples.TicTac
{
    public static class PositionEvaluator
    {
        private static readonly List<byte[,]> WinKernels = new List<byte[,]>
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

        public static string GetWinningPlayerTag(Cell[,] cells)
        {
            var winningPlayerTag = string.Empty;
            foreach (byte[,] winKernel in WinKernels)
            {
                winningPlayerTag = GetWinningPlayerTagByKernel(winKernel, cells);
                if (!string.IsNullOrEmpty(winningPlayerTag))
                {
                    return winningPlayerTag;
                }
            }

            return winningPlayerTag;
        }

        private static string GetWinningPlayerTagByKernel(byte[,] kernel, Cell[,] cells)
        {
            int boardRowsCount = cells.GetLength(0);
            int boardColumnsCount = cells.GetLength(1);

            int halfKernelRowsCount = kernel.GetLength(0) / 2;
            int halfKernelColumnsCount = kernel.GetLength(1) / 2;

            for (int i = 0; i < boardRowsCount; i++)
            {
                for (int j = 0; j < boardColumnsCount; j++)
                {
                    int sum = 0;
                    for (int k = -halfKernelRowsCount; k <= halfKernelRowsCount; k++)
                    {
                        for (int l = -halfKernelColumnsCount; l <= halfKernelColumnsCount; l++)
                        {
                            if (i + k < 0 || j + l < 0 || i + k >= boardRowsCount || j + l >= boardColumnsCount)
                            {
                                continue;
                            }

                            if (kernel[k + halfKernelRowsCount, l + halfKernelColumnsCount] != 1)
                            {
                                continue;
                            }

                            switch (cells[i + k, j + l].State)
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
