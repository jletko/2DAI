using System;
using System.Collections.Generic;

namespace Examples.TicTac
{
    public class PositionEvaluator
    {
        private readonly int[,] _allKernelStats = new int[3, 6];
        private readonly int[,] _lastAllKernelStats = new int[3, 6];
        private readonly int[,] _allKernelStatsDelta = new int[3, 6];
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

        public int[,] PositionStats => _allKernelStats;

        public int[,] PositionStatsDelta => _allKernelStatsDelta;

        public void UpdatePositionStats(byte[,] cells)
        {
            for (int i = 0; i < _allKernelStats.GetLength(0); i++)
            {
                for (int j = 0; j < _allKernelStats.GetLength(1); j++)
                {
                    _lastAllKernelStats[i, j] = _allKernelStats[i, j];
                }
            }

            for (int i = 0; i < _allKernelStats.GetLength(0); i++)
            {
                for (int j = 0; j < _allKernelStats.GetLength(1); j++)
                {
                    _allKernelStats[i, j] = 0;
                }
            }

            foreach (byte[,] winKernel in _winKernels)
            {
                int[,] kernelStats = GetKernelStats(winKernel, cells);
                for (int i = 0; i < _allKernelStats.GetLength(0); i++)
                {
                    for (int j = 0; j < _allKernelStats.GetLength(1); j++)
                    {
                        _allKernelStats[i, j] = _allKernelStats[i, j] + kernelStats[i, j];
                    }
                }
            }

            for (int i = 0; i < _allKernelStats.GetLength(0); i++)
            {
                for (int j = 0; j < _allKernelStats.GetLength(1); j++)
                {
                    _allKernelStatsDelta[i, j] = _allKernelStats[i, j] - _lastAllKernelStats[i, j];
                }
            }
        }

        private static int[,] GetKernelStats(byte[,] kernel, byte[,] cells)
        {
            int boardRowsCount = cells.GetLength(0);
            int boardColumnsCount = cells.GetLength(1);

            int[,] kernelStats = new int[3, 6];
            for (int i = 0; i < boardRowsCount; i++)
            {
                for (int j = 0; j < boardColumnsCount; j++)
                {
                    int[] statsOnCoordinates = GetKernelStatsOnCoordinates(kernel, cells, i, j);
                    kernelStats[1, statsOnCoordinates[1]] = kernelStats[1, statsOnCoordinates[1]] + 1;
                    kernelStats[2, statsOnCoordinates[2]] = kernelStats[2, statsOnCoordinates[2]] + 1;
                }
            }

            return kernelStats;
        }

        private static int[] GetKernelStatsOnCoordinates(byte[,] kernel, byte[,] cells, int i, int j)
        {
            int boardRowsCount = cells.GetLength(0);
            int boardColumnsCount = cells.GetLength(1);

            int halfKernelRowsCount = kernel.GetLength(0) / 2;
            int halfKernelColumnsCount = kernel.GetLength(1) / 2;

            int max1 = 0;
            int max2 = 0;
            for (int k = -halfKernelRowsCount; k <= halfKernelRowsCount; k++)
            {
                for (int l = -halfKernelColumnsCount; l <= halfKernelColumnsCount; l++)
                {
                    if (kernel[k + halfKernelRowsCount, l + halfKernelColumnsCount] != 1)
                    {
                        continue;
                    }

                    if (i + k < 0 || j + l < 0 || i + k >= boardRowsCount || j + l >= boardColumnsCount)
                    {
                        return new int[3];
                    }

                    switch (cells[i + k, j + l])
                    {
                        case 0:
                            break;
                        case 1:
                            if (max2 > 0)
                            {
                                return new int[3];
                            }
                            max1++;
                            break;
                        case 2:
                            if (max1 > 0)
                            {
                                return new int[3];
                            }
                            max2++;
                            break;
                        default:
                            throw new ArgumentException($"Unknown cell value: {cells[i + k, j + l]}");
                    }
                }
            }

            return new[] { 0, max1, max2 };
        }
    }
}
