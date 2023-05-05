using TMPro;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private SlicedFilledImage healthImage;
        [SerializeField]
        private TMP_Text healthText;

        private int currentValue = 100;
        private int maxValue = 100;

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void SetCurrentHealth(int value)
        {
            if (value < 0)
            {
                currentValue = 0;
            }
            else if (value > maxValue)
            {
                currentValue = maxValue;
            }
            else
            {
                currentValue = value;
            }

            RefreshBar();
        }

        public void ModifyCurrentHealth(int amount)
        {
            currentValue += amount;
            if (currentValue < 0)
            {
                currentValue = 0;
            }
            else if (currentValue > maxValue)
            {
                currentValue = maxValue;
            }

            RefreshBar();
        }

        public void SetMaxHealth(int value)
        {
            if (value < 0)
            {
                maxValue = 0;
            }
            else
            {
                maxValue = value;
            }

            if (currentValue > maxValue)
            {
                currentValue = maxValue;
            }

            RefreshBar();
        }

        private void RefreshBar()
        {
            healthText.text = $"{currentValue}/{maxValue}";
            if (maxValue > 0)
            {
                float amount = currentValue / (float)maxValue;
                SetHealthFillAmount(amount);
            }
            else
            {
                SetHealthFillAmount(0f);
            }
        }

        private void SetHealthFillAmount(float amount)
        {
            amount = Mathf.Clamp01(amount);
            healthImage.fillAmount = amount;
        }
    }
}
