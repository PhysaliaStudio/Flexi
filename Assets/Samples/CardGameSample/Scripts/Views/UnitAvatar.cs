using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public class UnitAvatar : MonoBehaviour
    {
        [SerializeField]
        private HealthBar healthBar;

        private new UnitAvatarAnimation animation;
        private Unit unit;

        public Unit Unit => unit;
        public Transform PopupHook => animation != null ? animation.PopupHook : transform;

        public void SetAnimation(UnitAvatarAnimation animation)
        {
            this.animation = animation;
        }

        public void SetUnit(Unit unit)
        {
            this.unit = unit;
            healthBar.SetMaxHealth(unit.HealthMax);
            healthBar.SetCurrentHealth(unit.Health);
        }

        public void Heal(int amount)
        {
            healthBar.ModifyCurrentHealth(amount);
        }

        public void Damage(int amount)
        {
            healthBar.ModifyCurrentHealth(-amount);
            animation.PlayDamaged();
        }
    }
}
