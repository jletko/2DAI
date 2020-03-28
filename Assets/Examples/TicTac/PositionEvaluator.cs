using System;
using System.Collections.Generic;

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

        public static int[,] GetPositionStats(byte[,] cells)
        {
            int[,] allKernelStats = new int[3, 6];
            foreach (byte[,] winKernel in WinKernels)
            {
                int[,] kernelStats = GetKernelStats(winKernel, cells);
                for (int i = 0; i < allKernelStats.GetLength(0); i++)
                {
                    for (int j = 0; j < allKernelStats.GetLength(1); j++)
                    {
                        allKernelStats[i, j] = allKernelStats[i, j] + kernelStats[i, j];
                    }
                }
            }

            return allKernelStats;
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

            int max_1 = 0;
            int max_2 = 0;
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
                            if (max_2 > 0)
                            {
                                return new int[3];
                            }
                            max_1++;
                            break;
                        case 2:
                            if (max_1 > 0)
                            {
                                return new int[3];
                            }
                            max_2++;
                            break;
                        default:
                            throw new ArgumentException($"Unknown cell value: {cells[i + k, j + l]}");
                    }
                }
            }

            return new[] { 0, max_1, max_2 };
        }
    }
}
