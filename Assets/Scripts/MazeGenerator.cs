using System;
using System.Collections.Generic;

public class MazeCellData
{
    // Orden: 0 = WEST, 1 = NORTH, 2 = EAST, 3 = SOUTH
    public bool[] Walls = { true, true, true, true };
    public bool IsVisited = false;
}

public class MazeGenerator
{
    public int Width { get; }
    public int Height { get; }

    public MazeCellData[,] Cells { get; }

    private readonly Random random;

    private struct Neighbor
    {
        public int X;
        public int Y;
        public WallOrientation FromCurrent;
        public WallOrientation FromNeighbor;

        public Neighbor(int x, int y, WallOrientation fromCurrent, WallOrientation fromNeighbor)
        {
            X = x;
            Y = y;
            FromCurrent = fromCurrent;
            FromNeighbor = fromNeighbor;
        }
    }

    public MazeGenerator(int width, int height, int? seed = null)
    {
        Width = width;
        Height = height;

        Cells = new MazeCellData[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Cells[x, y] = new MazeCellData();
            }
        }

        random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public void Generate()
    {
        var stack = new Stack<(int x, int y)>();

        int startX = 0;
        int startY = 0;

        Cells[startX, startY].IsVisited = true;
        stack.Push((startX, startY));

        while (stack.Count > 0)
        {
            var current = stack.Peek();
            var neighbors = GetUnvisitedNeighbors(current.x, current.y);

            if (neighbors.Count == 0)
            {
                stack.Pop();
                continue;
            }

            Neighbor chosen = neighbors[random.Next(neighbors.Count)];

            Cells[current.x, current.y].Walls[(int)chosen.FromCurrent] = false;
            Cells[chosen.X, chosen.Y].Walls[(int)chosen.FromNeighbor] = false;

            Cells[chosen.X, chosen.Y].IsVisited = true;
            stack.Push((chosen.X, chosen.Y));
        }
    }

    private List<Neighbor> GetUnvisitedNeighbors(int x, int y)
    {
        var result = new List<Neighbor>();

        // Oeste
        if (x > 0 && !Cells[x - 1, y].IsVisited)
        {
            result.Add(new Neighbor(
                x - 1, y,
                WallOrientation.WEST,
                WallOrientation.EAST
            ));
        }

        // Norte
        if (y < Height - 1 && !Cells[x, y + 1].IsVisited)
        {
            result.Add(new Neighbor(
                x, y + 1,
                WallOrientation.NORTH,
                WallOrientation.SOUTH
            ));
        }

        // Este
        if (x < Width - 1 && !Cells[x + 1, y].IsVisited)
        {
            result.Add(new Neighbor(
                x + 1, y,
                WallOrientation.EAST,
                WallOrientation.WEST
            ));
        }

        // Sur
        if (y > 0 && !Cells[x, y - 1].IsVisited)
        {
            result.Add(new Neighbor(
                x, y - 1,
                WallOrientation.SOUTH,
                WallOrientation.NORTH
            ));
        }

        return result;
    }
}
