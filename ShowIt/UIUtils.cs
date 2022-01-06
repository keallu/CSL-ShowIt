using ColossalFramework.UI;
using UnityEngine;

namespace ShowIt
{
    public class UIUtils
    {
        public static UIFont GetUIFont(string name)
        {
            UIFont[] fonts = Resources.FindObjectsOfTypeAll<UIFont>();

            foreach (UIFont font in fonts)
            {
                if (font.name.CompareTo(name) == 0)
                {
                    return font;
                }
            }

            return null;
        }

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

        public static UICheckBox CreateCheckBox(UIComponent parent, string name, string text, bool state)
        {
            UICheckBox checkBox = parent.AddUIComponent<UICheckBox>();
            checkBox.name = name;

            checkBox.height = 16f;
            checkBox.width = parent.width - 10f;

            UISprite uncheckedSprite = checkBox.AddUIComponent<UISprite>();
            uncheckedSprite.spriteName = "check-unchecked";
            uncheckedSprite.size = new Vector2(16f, 16f);
            uncheckedSprite.relativePosition = Vector3.zero;

            UISprite checkedSprite = checkBox.AddUIComponent<UISprite>();
            checkedSprite.spriteName = "check-checked";
            checkedSprite.size = new Vector2(16f, 16f);
            checkedSprite.relativePosition = Vector3.zero;
            checkBox.checkedBoxObject = checkedSprite;

            checkBox.label = checkBox.AddUIComponent<UILabel>();
            checkBox.label.text = text;
            checkBox.label.font = GetUIFont("OpenSans-Regular");
            checkBox.label.autoSize = false;
            checkBox.label.height = 20f;
            checkBox.label.verticalAlignment = UIVerticalAlignment.Middle;
            checkBox.label.relativePosition = new Vector3(20f, 0f);

            checkBox.isChecked = state;

            return checkBox;
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
    }
}
