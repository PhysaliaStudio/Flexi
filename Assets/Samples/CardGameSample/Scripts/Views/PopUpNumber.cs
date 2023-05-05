using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public class PopUpNumber : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text number;
        [SerializeField]
        private Transform child;

        private Sequence sequence;
        private int value;

        public void Init()
        {
            child.localPosition = Vector3.zero;
            number.color = Color.white;
            number.text = "";
            value = 0;
        }

        public int GetValue()
        {
            return value;
        }

        public void SetNumber(int value)
        {
            this.value = value;
            if (value > 0)
            {
                number.text = $"+{value}";
            }
            else
            {
                number.text = value.ToString();
            }
        }

        public void SetColor(Color color)
        {
            number.color = color;
        }

        public void Play(GameObjectPool<PopUpNumber> pool, float fadeTime)
        {
            if (sequence != null)
            {
                sequence.Kill(false);
            }

            sequence = DOTween.Sequence();
            sequence.Insert(0f, child.GetRectTransform().DOAnchorPosY(1.5f, fadeTime));
            sequence.Insert(0f, number.DOFade(0f, fadeTime));
            sequence.OnComplete(() => { pool.Release(this); });
        }
    }
}
