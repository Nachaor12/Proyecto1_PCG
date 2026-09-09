using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InterpolationMode
{
    Bilinear,
    Bicubic
}

public static class ValueNoise
{
    public static float[,] GenerateValueNoiseMap(int resolution, int latticeSpacing, int seed, InterpolationMode interpolationMode)
    {
        if (latticeSpacing <= 0)
            throw new ArgumentException("Lattice spacing debe ser mayor que 0.");

        int numberOfCells = (resolution - 1) / latticeSpacing;
        int controlPointResolution = numberOfCells + 1;

        float[,] controlPoints = GenerateControlPoints(controlPointResolution, seed);
        float[,] heights = new float[resolution, resolution];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int cellX = Mathf.Min(x / latticeSpacing, numberOfCells - 1);
                int cellY = Mathf.Min(y / latticeSpacing, numberOfCells - 1);

                float tx = (x - cellX * latticeSpacing) / (float)latticeSpacing;
                float ty = (y - cellY * latticeSpacing) / (float)latticeSpacing;

                float topLeft = controlPoints[cellY, cellX];
                float topRight = controlPoints[cellY, cellX + 1];
                float bottomLeft = controlPoints[cellY + 1, cellX];
                float bottomRight = controlPoints[cellY + 1, cellX + 1];

                float weightX = GetInterpolationWeight(tx, interpolationMode);
                float weightY = GetInterpolationWeight(ty, interpolationMode);

                float top = LinearInterpolation(topLeft, topRight, weightX);
                float bottom = LinearInterpolation(bottomLeft, bottomRight, weightX);

                heights[y, x] = LinearInterpolation(top, bottom, weightY);
            }
        }

        return heights;
    }

    public static float LinearInterpolation(float a, float b, float t)
    {
        return (a * (1f - t)) + (b * t);
    }

    public static float BicubicWeight(float t)
    {
        return (-2f * t * t * t) + (3f * t * t);
    }

    public static float GetInterpolationWeight(float t, InterpolationMode mode)
    {
        return mode == InterpolationMode.Bicubic ? BicubicWeight(t) : t;
    }

    private static float[,] GenerateControlPoints(int resolution, int seed)
    {
        System.Random rand = new System.Random(seed);
        float[,] points = new float[resolution, resolution];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                points[y, x] = (float)rand.NextDouble();
            }
        }

        return points;
    }
}