using UnityEngine;

namespace Physalia.Flexi.Samples.ActionGame
{
    [RequireComponent(typeof(MontagePlayer))]
    public class UnitAvatar : MonoBehaviour, IUnitAvatar
    {
        [SerializeField]
        private MontagePlayer montagePlayer;

        public bool HasMontage(string name)
        {
            return montagePlayer.HasMontage(name);
        }

        public void PlayMontage(string name)
        {
            montagePlayer.Play(name);
        }

        public bool IsMontagePlayedAndFinished(string name)
        {
            return montagePlayer.IsPlayedAndFinished(name);
        }

        public void Move(float x, float z)
        {
            if (x == 0f && z == 0f)
            {
                montagePlayer.Play("Idle");
                return;
            }

            montagePlayer.Play("Move");

            float angle = Mathf.Atan2(z, x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0f, 90f - angle, 0f);
        }
    }
}
