using System;
using TMPro;
using UnityEngine;

namespace Physalia.Flexi.Samples.ActionGame
{
    public class AbilitySlotView : MonoBehaviour
    {
        [SerializeField]
        private GameObject cooldownObject;
        [SerializeField]
        private TextMeshProUGUI timeText;

        private Unit unit;

        public void SetUnit(Unit unit)
        {
            this.unit = unit;
            RefreshCooldownTime();
        }

        private void Update()
        {
            RefreshCooldownTime();
        }

        private void RefreshCooldownTime()
        {
            AbilitySlot slot = unit.AbilitySlot;
            AbilitySlot.State state = slot.GetState();
            cooldownObject.SetActive(!slot.IsCastable());

            if (state != AbilitySlot.State.COOLDOWN)
            {
                timeText.text = "";
                return;
            }

            DateTime nextTime = unit.AbilitySlot.GetNextTime();
            if (nextTime < DateTime.Now)
            {
                timeText.text = "";
                return;
            }

            float seconds = (float)(nextTime - DateTime.Now).TotalSeconds;
            timeText.text = $"{seconds:0.0}s";
        }
    }
}
