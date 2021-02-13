namespace Bob
{
    public partial class Grid
    {
        private Bubble[][] grid;

        public Vector Size;

        public Grid(Vector GridSize)
        {
            Size = GridSize;

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

        /*
        public Bubble GetFromPosition (Vector position)
        {
            return grid[position.Y][position.X];
        }

        public void SetToPosition(Vector position, ref Bubble bubble)
        {
            grid[position.Y][position.X] = bubble;
        }*/

        public void SlideMap(int step = 1)
        {
            for (int y = Size.Y - step; y >= 0; y--)
            {
                for (int x = 0; x < Size.X; x++)
                {
                    grid[y + step][x] = grid[y][x];
                }
            }
        }
    }
}
