﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace Labeling
{
    [Serializable]
    public class CustomTextStyle : TextStyle
    {
        private float minFontSize;
        private float maxFontSize;

        public CustomTextStyle(string textColumnName, GeoFont textFont, GeoSolidBrush textSolidBrush)
            : base(textColumnName, textFont, textSolidBrush)
        {
            this.minFontSize = 8;
            this.MaxFontSize = 25;
        }

        public float MinFontSize
        {
            get { return minFontSize; }
            set { minFontSize = value; }
        }

        public float MaxFontSize
        {
            get { return maxFontSize; }
            set { maxFontSize = value; }
        }

        protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas, Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
        {
            TextStyle clonedStyle = new TextStyle(this.TextColumnName, this.Font, this.TextBrush);
            clonedStyle.HaloPen = this.HaloPen;
            clonedStyle.Mask = this.Mask;
            clonedStyle.GridSize = this.GridSize;
            clonedStyle.OverlappingRule = this.OverlappingRule;
            clonedStyle.DuplicateRule = this.DuplicateRule;
            clonedStyle.TextPlacement = TextPlacement.Right;

            float fontSize = Convert.ToInt32(50000 / canvas.CurrentScale);

            if (fontSize < minFontSize)
            {
                fontSize = minFontSize;
            }
            else if (fontSize > maxFontSize)
            {
                fontSize = maxFontSize;
            }

            clonedStyle.Font = new GeoFont(clonedStyle.Font.FontName, fontSize, clonedStyle.Font.Style);
            clonedStyle.Draw(features, canvas, labelsInThisLayer, labelsInAllLayers);
        }
    }
}