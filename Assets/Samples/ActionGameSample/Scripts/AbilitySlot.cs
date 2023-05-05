using System;

namespace Physalia.Flexi.Samples.ActionGame
{
    public class AbilitySlot
    {
        public enum State { OPEN, RECAST, DISABLED, COOLDOWN }

        private State state = State.OPEN;
        private DateTime nextTime;

        public bool IsCastable()
        {
            if (state == State.DISABLED || state == State.COOLDOWN)
            {
                return false;
            }

            return true;
        }

        public State GetState()
        {
            return state;
        }

        public void SetToOpenState()
        {
            state = State.OPEN;
            nextTime = DateTime.Now;
        }

        public void SetToRecastState()
        {
            state = State.RECAST;
            nextTime = DateTime.Now;
        }

        public void SetToDisabledState()
        {
            state = State.DISABLED;
            nextTime = DateTime.MaxValue;
        }

        public void SetToCooldownState(int milliseconds)
        {
            state = State.COOLDOWN;
            SetNextTime(milliseconds);
        }

        public DateTime GetNextTime()
        {
            return nextTime;
        }

        private void SetNextTime(int milliseconds)
        {
            if (milliseconds == 0)
            {
                nextTime = DateTime.Now;
            }
            else
            {
                nextTime = DateTime.Now + TimeSpan.FromMilliseconds(milliseconds);
            }
        }

        public void Tick()
        {
            if (state == State.COOLDOWN)
            {
                if (DateTime.Now >= nextTime)
                {
                    state = State.OPEN;
                }
            }
        }
    }
}
