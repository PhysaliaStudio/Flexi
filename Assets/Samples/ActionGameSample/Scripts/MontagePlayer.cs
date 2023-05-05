using UnityEngine;

namespace Physalia.Flexi.Samples.ActionGame
{
    public class MontagePlayer : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;

        private AnimatorStateInfo lastEndStateInfo;
        private AnimatorStateInfo lastStartStateInfo;

        private void Awake()
        {
            MontageBehaviour[] behaviours = animator.GetBehaviours<MontageBehaviour>();
            for (var i = 0; i < behaviours.Length; i++)
            {
                behaviours[i].SetMontagePlayer(this);
            }
        }

        public void NotifyStateStart(AnimatorStateInfo stateInfo)
        {
            lastStartStateInfo = stateInfo;
        }

        public void NotifiyStateEnd(AnimatorStateInfo stateInfo)
        {
            lastEndStateInfo = stateInfo;
        }

        public bool HasMontage(string name)
        {
            int hash = Animator.StringToHash(name);
            bool result = animator.HasState(0, hash);
            return result;
        }

        public void Play(string name)
        {
            animator.Play(name);
        }

        public bool IsPlayedAndFinished(string name)
        {
            return lastEndStateInfo.IsName(name);
        }
    }
}
