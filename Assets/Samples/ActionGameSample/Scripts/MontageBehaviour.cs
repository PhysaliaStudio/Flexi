using UnityEngine;

namespace Physalia.Flexi.Samples.ActionGame
{
    public class MontageBehaviour : StateMachineBehaviour
    {
        private MontagePlayer montagePlayer;

        public void SetMontagePlayer(MontagePlayer montagePlayer)
        {
            this.montagePlayer = montagePlayer;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            if (montagePlayer != null)
            {
                montagePlayer.NotifyStateStart(stateInfo);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            if (montagePlayer != null)
            {
                montagePlayer.NotifiyStateEnd(stateInfo);
            }
        }
    }
}
