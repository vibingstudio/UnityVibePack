using System.Collections;
using UnityEngine;

namespace VibePack.Utility
{
    /// <summary>
    /// Awaits for a given IAwaitable.
    /// </summary>
    public class Awaiter : CustomYieldInstruction
    {
        readonly IAwaitable awaitable;

        public override bool keepWaiting => awaitable.ShouldWait();

        public Awaiter(IAwaitable awaitable) => this.awaitable = awaitable;
    }
}
