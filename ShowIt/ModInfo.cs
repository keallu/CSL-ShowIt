using ICities;
using System;

namespace ShowIt
{
    public class ModInfo : IUserMod
    {
        public string Name => "Show It!";
        public string Description => "Shows vital indicators for zoned buildings.";

        private static readonly string[] ExtendedPanelAlignmentLabels =
        {
            "Right",
            "Bottom"
        };

        private static readonly string[] ExtendedPanelAlignmentValues =
        {
            "Right",
            "Bottom"
        };

        public void OnSettingsUI(UIHelperBase helper)
        {
            UIHelperBase group = helper.AddGroup(Name);

            int selectedIndex;
            float selectedValue;

            selectedIndex = GetSelectedOptionIndex(ExtendedPanelAlignmentValues, ModConfig.Instance.ExtendedPanelAlignment);
            group.AddDropdown("Alignment", ExtendedPanelAlignmentLabels, selectedIndex, sel =>
            {
                ModConfig.Instance.ExtendedPanelAlignment = ExtendedPanelAlignmentValues[sel];
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.ExtendedPanelOpacity;
            group.AddSlider("Opacity", 0.5f, 1f, 0.05f, selectedValue, sel =>
            {
                ModConfig.Instance.ExtendedPanelOpacity = sel;
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.ExtendedPanelOpacityWhenHover;
            group.AddSlider("Opacity When Hover", 0.5f, 1f, 0.05f, selectedValue, sel =>
            {
                ModConfig.Instance.ExtendedPanelOpacityWhenHover = sel;
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.ExtendedPanelChartOverlayTextScale;
            group.AddSlider("Chart Overlay Text Scale", 0.5f, 1.0f, 0.05f, selectedValue, sel =>
            {
                ModConfig.Instance.ExtendedPanelChartOverlayTextScale = sel;
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.ExtendedPanelChartIconSize;
            group.AddSlider("Chart Icon Size", 20f, 30f, 1.0f, selectedValue, sel =>
            {
                ModConfig.Instance.ExtendedPanelChartIconSize = sel;
                ModConfig.Instance.Save();
            });
        }

        private int GetSelectedOptionIndex(string[] option, string value)
        {
            int index = Array.IndexOf(option, value);
            if (index < 0) index = 0;

            return index;
        }
    }
}
