using System;

namespace Bob
{
    public class Bubble
    {
        public enum BubbleType
        {
            _2,
            _4,
            _8,
            _16,
            _32,
            _64,
            _128,
            _256,
            Max
        }

        public readonly ushort Id;
        public BubbleType Numberos;

        public Bubble(ushort _Id)
        {
            int m = new Random().Next(0, (int)BubbleType.Max - 1);
            Numberos = (BubbleType)m;
            Id = _Id;
        }
    }
}
