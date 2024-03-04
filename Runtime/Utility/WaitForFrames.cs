using UnityEngine;

namespace VibePack.Utility
{
    /// <summary>
    /// Waits for a given number of frames.
    /// </summary>
    public class WaitForFrames : CustomYieldInstruction
    {
        int frames;

        public override bool keepWaiting => --frames > 0;

        public WaitForFrames(int frames) => this.frames = frames;
    }
}
