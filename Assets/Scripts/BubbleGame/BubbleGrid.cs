using System.Collections.Generic;

namespace Bob
{
    public struct BubbleGrid
    {
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

        public void SeekForCombine (Vector position)
        {
            OutputLog.AddLog("[Grid] Seeking for combine at position => " + position);

            var bubble = GetFromPosition(position);
            if (bubble == null)
            {
                OutputLog.AddError("[Grid] No bubble at this position to start seeking => " + position);
            }

            OutputLog.AddLog("[Grid] Seeking started at position " + position + " with bubble type => " + bubble.Numberos.ToString());

            var cType = bubble.Numberos;
            List<Vector> except = new List<Vector>();
            except.Add(position);

            var best = GetCombinations(cType, position, ref except);

            OutputLog.AddLog("[Grid] Combine result length => " + best.Count.ToString ());
        }

        private List<Vector> GetCombinations (Bubble.BubbleType type, Vector pivotPosition, ref List<Vector> except)
        {
            List<Vector> final = new List<Vector>();

            for (int i = 0; i < dirCount; i++)
            {
                List<Vector> points = new List<Vector>();

                var cPosition = pivotPosition + seekDirections[i];

                if (except != null && except.Contains(cPosition))
                {
                    // except this.
                    continue;
                }

                var bubble = GetFromPosition(cPosition);
                if (bubble == null)
                    continue;

                except.Add(cPosition);

                if (bubble.Numberos == type)
                {
                    var newType = (Bubble.BubbleType)(int)type + 1;
                    points.Add(cPosition);

                    var go = GetCombinations(newType, cPosition, ref except);
                    points.AddRange(go);
                }

                if (final.Count < points.Count)
                    final = points;
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

        public bool IsPositionAvailable (Vector position)
        {
            if (position.Y < 0 || position.Y >= Size.Y)
                return false;
            if (position.X < 0 || position.X >= Size.X)
                return false;

            return GetFromPosition(position) == null;
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
    }
}
