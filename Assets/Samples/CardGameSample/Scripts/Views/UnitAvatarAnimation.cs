using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    [RequireComponent(typeof(Animator))]
    public class UnitAvatarAnimation : MonoBehaviour
    {
        private readonly int IDLE_HASH = Animator.StringToHash("Idle");
        private readonly int DAMAGED_HASH = Animator.StringToHash("Damaged");

        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Transform popupHook;

        public Transform PopupHook => popupHook != null ? popupHook : transform;

        public void PlayIdle()
        {
            animator.Play(IDLE_HASH);
        }

        public void PlayDamaged()
        {
            animator.Play(DAMAGED_HASH);
        }
    }
}
