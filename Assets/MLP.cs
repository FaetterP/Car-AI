using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets
{
    class MLP
    {
        private static Random rnd = new Random();
        private double[][,] wmatrs;
        private double[][] layers;

        public MLP(int[] lengths)
        {
            layers = new double[lengths.Length][];
            for (int i = 0; i < lengths.Length; i++)
            {
                if (i < lengths.Length - 1)
                {
                    layers[i] = new double[lengths[i] + 1];
                    layers[i][lengths[i]] = 1;
                }
                else
                {
                    layers[i] = new double[lengths[i]];
                }
            }

            wmatrs = new double[lengths.Length - 1][,];
            for (int i = 0; i < lengths.Length - 1; i++)
            {
                wmatrs[i] = new double[lengths[i] + 1, lengths[i + 1]];
            }
        }

        private void mult(double[] N1, double[,] W12, double[] N2, bool IsLast)
        {
            int val = N2.Length;
            if (!IsLast) { val--; }
            for (int i = 0; i < val; i++)
            {
                N2[i] = 0;
                for (int j = 0; j < N1.Length; j++)
                {
                    N2[i] += N1[j] * W12[j, i];
                }
                N2[i] = sigm(N2[i]);
            }
        }

        private double sigm(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public void RandomW(float percent)
        {
            for (int i = 0; i < wmatrs.Length; i++)
            {
                for (int j = 0; j < wmatrs[i].GetLength(0); j++)
                {
                    for (int k = 0; k < wmatrs[i].GetLength(1); k++)
                    {
                        if (rnd.NextDouble() <= percent)
                        {
                            wmatrs[i][j, k] = rnd.NextDouble() * 2 - 1;
                        }
                    }
                }
            }
        }

        public double[] Predict(double[] input)
        {
            Array.Copy(input, layers[0], input.Length);

            for (int j = 0; j < layers.Length - 2; j++)
            {
                mult(layers[j], wmatrs[j], layers[j + 1], false);
            }
            mult(layers[layers.Length - 2], wmatrs[layers.Length - 2], layers[layers.Length - 1], true);

            return layers.Last();
        }

        public double[][,] GetW()
        {
            double[][,] ret = new double[wmatrs.Length][,];

            for (int i = 0; i < wmatrs.Length; i++)
            {
                ret[i] = new double[wmatrs[i].GetLength(0), wmatrs[i].GetLength(1)];

                for (int i0 = 0; i0 < wmatrs[i].GetLength(0); i0++)
                {
                    for (int i1 = 0; i1 < wmatrs[i].GetLength(1); i1++)
                    {
                        ret[i][i0, i1] = wmatrs[i][i0, i1];
                    }
                }
            }

            return ret;
        }

        public void SetW(double[][,] w)
        {
            wmatrs = w;
        }
    }
}
