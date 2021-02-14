using System.Collections.Generic;

namespace Bob
{
    public struct BubbleGrid
    {
#if UNITY_EDITOR
        public Bubble[][] GetGrid => grid;
#endif

        const int dirCount = 8;

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

        public List<Bubble> SeekForCombine(Vector position, out Bubble[] mixes, out int mixCount)
        {
            mixCount = 0;
            mixes = new Bubble[dirCount];

            OutputLog.AddLog("[Grid] Seeking for combine at position => " + position);

            var bubble = GetFromPosition(position);
            if (bubble == null)
            {
                OutputLog.AddError("[Grid] No bubble at this position to start seeking => " + position);
                return null;
            }
            var cType = bubble.Numberos;

            OutputLog.AddLog("[Grid] Seeking started at position " + position + " with bubble type => " + cType);

            List<Vector> except = new List<Vector>();
            except.Add(position);

            var best = GetCombinations(cType, position, ref except);

            // gathered mixes.
            mixCount = GetMixes(cType, position, mixes, best);

            return best;
        }

        private int GetMixes (Bubble.BubbleType type, Vector pivotPosition, Bubble[] mixes, List<Bubble> except)
        {
            int counter = 0;
            for (int i = 0; i < dirCount; i++)
            {
                var cPosition = pivotPosition + seekDirections[i];

                var bubble = GetFromPosition(cPosition);
                if (bubble == null)
                {
                    OutputLog.AddLog("No bubble at this pos => " + cPosition);
                    continue;
                }

                if (bubble.Numberos == type)
                {
                    if (!except.Contains(bubble))
                    {
                        OutputLog.AddLog("Found a sibling at => " + cPosition);
                        mixes[counter++] = bubble;
                    }
                    else OutputLog.AddLog("Found a sibling at => " + cPosition + " but it was in the exceptions.");
                }
                else OutputLog.AddLog("No sibling at this position => " + cPosition);
            }

            return counter;
        }

        private List<Bubble> GetCombinations (Bubble.BubbleType type, Vector pivotPosition, ref List<Vector> except)
        {
            OutputLog.AddLog("Seeking at position => " + pivotPosition + ", type: " + type.ToString());

            List<Bubble> final = new List<Bubble>();

            for (int i = 0; i < dirCount; i++)
            {
                OutputLog.AddLog("Direction => " + seekDirections[i]);

                List<Bubble> points = new List<Bubble>();

                var cPosition = pivotPosition + seekDirections[i];

                if (except != null && except.Contains(cPosition))
                {
                    // except this.
                    OutputLog.AddLog("Already seeked that position before => " + cPosition);
                    continue;
                }

                var bubble = GetFromPosition(cPosition);
                if (bubble == null)
                {
                    OutputLog.AddLog("No bubble at this pos => " + cPosition);
                    continue;
                }

                except.Add(cPosition);

                if (bubble.Numberos == type)
                {
                    OutputLog.AddLog("Found a sibling at => " + cPosition);

                    var newType = (Bubble.BubbleType)(int)type + 1;
                    points.Add(bubble);

                    var go = GetCombinations(newType, cPosition, ref except);
                    points.AddRange(go);
                }
                else OutputLog.AddLog("No sibling at this position => " + cPosition);

                if (final.Count < points.Count)
                {
                    OutputLog.AddLog("New list found, total length => " + final.Count);
                    final = points;
                }

            }

            return final;
        }

        private Bubble GetFromPosition (Vector position)
        {
            if (position.Y < 0 || position.Y >= Size.Y)
                return null;
            if (position.X < 0 || position.X >= Size.X)
                return null;

            return grid[position.Y][position.X];
        }

        public bool FindClosePosition(Vector position, ref Vector result)
        {
            for (int i = 0; i < dirCount; i++)
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
            OutputLog.AddLog("Checking " + position.ToString() + " if its available.");
            if (position.Y < 0 || position.Y >= Size.Y)
            {
                OutputLog.AddLog("Y is out of map");
                return false;
            }

            if (position.X < 0 || position.X >= Size.X)
            {
                OutputLog.AddLog("X is out of map");
                return false;
            }

            var result = GetFromPosition(position) == null;
            OutputLog.AddLog("result => " + result);
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
    }
}
