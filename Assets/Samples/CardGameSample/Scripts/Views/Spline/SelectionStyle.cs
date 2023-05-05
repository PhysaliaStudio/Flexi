using System;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    [Serializable]
    public class SelectionStyle
    {
        public static readonly SelectionStyle DEFAULT = new(DEFAULT_COLOR, null);
        private static readonly Color DEFAULT_COLOR = new(1f, 1f, 1f, 0.9f);

        [SerializeField]
        private Color outlineColor;
        [SerializeField]
        private Material characterOutlineMaterial;
        [SerializeField]
        private GameUISpline.TargetType targetType;
        [SerializeField]
        private bool showPulseOnTransition;
        [SerializeField]
        [Tooltip("If the midpoints are crowding the end image, the number to not display")]
        private int numMidPointsToSkipAtEnd;

        public SelectionStyle(Color outlineColor, Material characterOutlineMaterial)
        {
            this.outlineColor = outlineColor;
            this.characterOutlineMaterial = characterOutlineMaterial;
        }

        public Color OutlineColor => outlineColor;
        public Material CharacterOutlineMaterial => characterOutlineMaterial;
        public GameUISpline.TargetType TargetType => targetType;
        public bool ShowPulseOnTransition => showPulseOnTransition;
        public int NumMidPointsToSkipAtEnd => numMidPointsToSkipAtEnd;
    }
}
