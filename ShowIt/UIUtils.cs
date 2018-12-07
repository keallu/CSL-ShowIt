using ColossalFramework.UI;
using UnityEngine;

namespace ShowIt
{
    public class UIUtils
    {
        public static UIPanel CreatePanel(UIComponent parent, string name)
        {
            UIPanel panel = parent.AddUIComponent<UIPanel>();
            panel.name = name;

            return panel;
        }

        public static UISprite CreateSprite(UIComponent parent, string name, string spriteName)
        {
            UISprite sprite = parent.AddUIComponent<UISprite>();
            sprite.name = name;
            sprite.spriteName = spriteName;

            return sprite;
        }

        public static UILabel CreateLabel(UIComponent parent, string name, string text)
        {
            UILabel label = parent.AddUIComponent<UILabel>();
            label.name = name;
            label.text = text;

            return label;
        }

        public static UIRadialChart CreateTwoSlicedRadialChart(UIComponent parent, string name)
        {
            UIRadialChart radialChart = parent.AddUIComponent<UIRadialChart>();
            radialChart.name = name;

            radialChart.size = new Vector3(50f, 50f);
            radialChart.spriteName = "PieChartBg";

            radialChart.AddSlice();
            UIRadialChart.SliceSettings slice = radialChart.GetSlice(0);
            Color32 color = new Color32(229, 229, 229, 128);
            slice.outterColor = color;
            slice.innerColor = color;

            radialChart.AddSlice();
            UIRadialChart.SliceSettings slice1 = radialChart.GetSlice(1);
            Color32 color1 = new Color32(178, 178, 178, 128);
            slice1.outterColor = color1;
            slice1.innerColor = color1;

            return radialChart;
        }

        public static UIButton CreateButton(UIComponent parent, string name)
        {
            UIButton button = parent.AddUIComponent<UIButton>();
            button.name = name;

            button.size = new Vector3(90f, 30f);
            button.textScale = 0.9f;
            button.normalBgSprite = "ButtonMenu";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.pressedBgSprite = "ButtonMenuPressed";
            button.canFocus = false;

            return button;
        }
    }
}
