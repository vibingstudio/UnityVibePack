using System;

namespace VibePack.Utility
{
    [Serializable]
    public struct ValueRange<T>
    {
        public T min;
        public T max;
    }
}
