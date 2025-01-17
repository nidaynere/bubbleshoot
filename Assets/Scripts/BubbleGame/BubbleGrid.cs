﻿using System.Collections.Generic;

namespace Bob
{
    public struct BubbleGrid
    {
        public const int DirectionCount = 8;

        private Bubble[][] grid;

        public Vector Size;

        private Vector[] seekDirections;

        public BubbleGrid(Vector GridSize)
        {
            Size = GridSize;

            seekDirections = new Vector[] {
                new Vector (-1, 1) ,
                new Vector (0, 1) ,
                new Vector (1, 1) ,
                new Vector (-1, 0) ,
                new Vector (1, 0),
                new Vector (-1, -1) ,
                new Vector (0, -1) ,
                new Vector (1, -1) 
            };

            // define grid.
            grid = new Bubble[Size.Y][];
            for (int y = 0; y < Size.Y; y++)
            {
                grid[y] = new Bubble[Size.X];
            }
            //
        }

        public void AddBubbles(Bubble[] bubbles)
        {
            SlideMap(1);

            for (int x = 0; x < Size.X; x++)
            {
                grid[0][x] = bubbles[x];
            }
        }

        public int GetAround (Vector position, Vector[] result)
        {
            int count = 0;

            if (GetFromPosition(position) != null)
                result[count++] = position;

            for (int i = 0; i < DirectionCount; i++)
            {
                var cPosition = position + seekDirections[i];
                
                var bubble = GetFromPosition(cPosition);
                if (bubble == null)
                {
                    continue;
                }

                result[count++] = position;
            }

            return count;
        }

        public List<Vector> SeekForCombine(Vector mustContain)
        {
            int sizeY = Size.Y;
            int sizeX = Size.X;

            int bestLength = 0;
            int lastY = sizeY+1;

            List<Vector> best = new List<Vector>();

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    Vector position = new Vector(x, y);

                    var bubble = GetFromPosition(position);
                    if (bubble == null)
                    {
                        continue;
                    }

                    var cType = bubble.Numberos;

                    var combines = GetCombinations(cType, position);
                    combines.Insert(0, position);

                    if ( ((bestLength == combines.Count && combines[combines.Count -1].Y < lastY) || bestLength < combines.Count) 
                        && combines.Contains (mustContain))
                    {
                        bestLength = combines.Count;
                        best = combines;
                        lastY = combines[combines.Count - 1].Y;
                        OutputLog.AddLog("better combine path found, count => " + bestLength + ", y= "+ lastY);
                    }
                }
            }

            return best;
        }

        public List<Vector> GetMixes (Bubble.BubbleType type, Vector pivotPosition, List<Vector> exceptThis)
        {
            List<Vector> mixes = new List<Vector>();

            for (int i = 0; i < DirectionCount; i++)
            {
                var cPosition = pivotPosition + seekDirections[i];

                if (exceptThis.Contains(cPosition))
                {
                    continue;
                }

                var bubble = GetFromPosition(cPosition);
                if (bubble == null)
                {
                    continue;
                }

                if (bubble.Numberos == type)
                {
                    mixes.Add(cPosition);
                    exceptThis.Add(cPosition);
                    mixes.AddRange (GetMixes(type, cPosition, exceptThis));
                }
            }

            return mixes;
        }

        private List<Vector> GetCombinations (Bubble.BubbleType type, Vector pivotPosition)
        {
            List<Vector> final = new List<Vector>();

            for (int i = 0; i < DirectionCount; i++)
            {
                List<Vector> points = new List<Vector>();

                var cPosition = pivotPosition + seekDirections[i];

                var bubble = GetFromPosition(cPosition);
                if (bubble == null)
                {
                    continue;
                }

                if (bubble.Numberos == type)
                {
                    var newType = (Bubble.BubbleType)(int)type + 1;
                    points.Add(cPosition);

                    var go = GetCombinations(newType, cPosition);
                    points.AddRange(go);
                }

                if (final.Count < points.Count)
                {
                    final = points;
                }

            }

            return final;
        }

        public Bubble GetFromPosition (Vector position)
        {
            if (position.Y < 0 || position.Y >= Size.Y)
                return null;
            if (position.X < 0 || position.X >= Size.X)
                return null;

            return grid[position.Y][position.X];
        }

        public bool RemoveFromPosition(Vector position)
        {
            if (position.Y < 0 || position.Y >= Size.Y)
                return false;
            if (position.X < 0 || position.X >= Size.X)
                return false;

            if (grid[position.Y][position.X] == null)
                return false;

            grid[position.Y][position.X] = null;

            return true;
        }

        public bool FindClosePosition(Vector position, ref Vector result)
        {
            for (int i = 0; i < DirectionCount; i++)
            {
                var pos = position + seekDirections[i];
                if (IsPositionAvailable(pos))
                {
                    result = pos;
                    return true;
                }
            }

            return false;
        }

        public bool IsPositionAvailable (Vector position)
        {
            if (position.Y < 0 || position.Y >= Size.Y)
            {
                return false;
            }

            if (position.X < 0 || position.X >= Size.X)
            {
                return false;
            }

            var result = GetFromPosition(position) == null;
            return result;
        }

        public void AddToPosition(Bubble bubble, Vector position)
        {
            if (grid[position.Y][position.X] != null)
            {
                OutputLog.AddError ("[Grid] Target position is not empty. You cannot put anything on this position.");
                return;
            }

            grid[position.Y][position.X] = bubble;

            OutputLog.AddLog("[Grid] Bubble added to position => " + position);
        }

        public void SlideMap (int step = 1)
        {
            for (int y = Size.Y - step -1 ; y >= 0; y--)
            {
                for (int x = 0; x < Size.X; x++)
                {
                    grid[y + step][x] = grid[y][x];
                }
            }
        }

        /// <summary>
        /// Returns all bubbles in the specific row index.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="bubbles"></param>
        /// <param name="positions"></param>
        /// <returns></returns>
        public int GetBubblesAtRow (int rowIndex, int xLength, ref Bubble[] bubbles, ref Vector[] positions)
        {
            if (rowIndex >= Size.Y)
                return 0;

            int counter = 0;

            for (int x = 0; x < xLength; x++)
            {
                if (grid[rowIndex][x] != null)
                {
                    bubbles[counter] = grid[rowIndex][x];
                    positions[counter] = new Vector(x, rowIndex);
                    counter++;
                }
            }

            return counter;
        }


        /// <summary>
        /// Returns the list of the unbound vectors.
        /// Unbound means, at least 1 non-empty grid point should be at the upper directions.
        /// X X B   X X X   B B X
        ///   O       O       O
        /// Bound  UnBound  Bound
        /// </summary>
        /// <returns></returns>
        public List<ushort> RemoveUnBounds ()
        {
            OutputLog.AddLog("Remove unbounds...");

            List<ushort> unbounds = new List<ushort>();

            int sizeX = Size.X;
            int sizeY = Size.Y;

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    Vector position = new Vector(x, y);

                    var point = GetFromPosition(position);
                    if (point != null)
                    {
                        bool found = false;

                        // check for point.
                        for (int i = 0; i < 3; i++)
                        {
                            var cPosition = position + seekDirections[DirectionCount - i - 1];

                            if (cPosition.Y < 0 || GetFromPosition(cPosition) != null)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            // gone.
                            unbounds.Add(point.Id);
                            RemoveFromPosition(position);
                        }
                    }
                }
            }

            return unbounds;
        }
    }
}
