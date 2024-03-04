using UnityEngine;
using System;

namespace VibePack.Utility
{
    [Serializable]
    public struct Optional<T>
    {
        [SerializeField] bool enabled;
        [SerializeField] T value;

        public bool Enabled => enabled;
        public T Value => value;

        public Optional(T initialValue)
        {
            enabled = true;
            value = initialValue;
        }

        public Optional(T initialValue, bool enabled)
        {
            this.enabled = enabled;
            value = initialValue;
        }

        public static implicit operator Optional<T>(T o) => new Optional<T>(o);

        public static implicit operator T(Optional<T> o) => o.Value;

        public static implicit operator bool(Optional<T> o) => o.enabled;

        public static bool operator ==(Optional<T> lhs, Optional<T> rhs)
        {
            if (lhs.value is null)
            {
                if (rhs.value is null)
                    return true;

                return false;
            }

            return lhs.value.Equals(rhs.value);
        }

        public static bool operator !=(Optional<T> lhs, Optional<T> rhs) => !(lhs == rhs);

        public override bool Equals(object obj) => value.Equals(obj);

        public override int GetHashCode() => value.GetHashCode();

        public override string ToString() => value.ToString();
    }
}
