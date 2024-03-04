using UnityEngine;

namespace VibePack.Utility
{
    /// <summary>
    /// Interface for easy use of Awaiter class.
    /// 
    public interface IAwaitable
    {
        public bool ShouldWait();

        public CustomYieldInstruction Await();
    }
}