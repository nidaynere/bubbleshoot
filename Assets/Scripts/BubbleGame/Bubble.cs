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

        public bool IncreaseNumberos()
        {
            int currentNumberos = (int)Numberos;
            currentNumberos++;
            int max = (int)BubbleType.Max;
            if (max <= currentNumberos)
            {
                return false;
            }

            Numberos = (BubbleType)currentNumberos;

            return true;
        }

        public Bubble(ushort _Id)
        {
            int m = Random.Range (0, (int)BubbleType.Max - 1);
            Numberos = (BubbleType)m;
            Id = _Id;
        }
    }
}
