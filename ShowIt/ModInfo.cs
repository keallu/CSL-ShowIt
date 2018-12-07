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

        private static readonly string[] ExtendedPanelBackgroundSpriteLabels =
        {
            "Generic Panel",
            "Info Bubble",
            "Info Bubble Service",
            "Info Display",
            "Info Panel",
            "Menu Container",
            "Menu Panel 1",
            "Menu Panel 2",
            "Subcategories Panel",
            "UnlockingPanel 1",
            "UnlockingPanel 2"
        };

        private static readonly string[] ExtendedPanelBackgroundSpriteValues =
        {
            "GenericPanel",
            "InfoBubble",
            "InfoBubbleService",
            "InfoDisplay",
            "InfoPanel",
            "MenuContainer",
            "MenuPanel1",
            "MenuPanel2",
            "SubcategoriesPanel",
            "UnlockingPanel1",
            "UnlockingPanel2"
        };

        private static readonly string[] ExtendedPanelChartHelpLabels =
        {
            "Text",
            "Icon"
        };

        private static readonly string[] ExtendedPanelChartHelpValues =
        {
            "Text",
            "Icon"
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

            selectedIndex = GetSelectedOptionIndex(ExtendedPanelBackgroundSpriteValues, ModConfig.Instance.ExtendedPanelBackgroundSprite);

            group.AddDropdown("Background Sprite", ExtendedPanelBackgroundSpriteLabels, selectedIndex, sel =>
            {
                ModConfig.Instance.ExtendedPanelBackgroundSprite = ExtendedPanelBackgroundSpriteValues[sel];
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.ExtendedPanelOpacity;

            group.AddSlider("Opacity", 0f, 1f, 0.05f, selectedValue, sel =>
            {
                ModConfig.Instance.ExtendedPanelOpacity = sel;
                ModConfig.Instance.Save();
            });

            selectedIndex = GetSelectedOptionIndex(ExtendedPanelChartHelpValues, ModConfig.Instance.ExtendedPanelChartHelp);

            group.AddDropdown("Chart Help", ExtendedPanelChartHelpLabels, selectedIndex, sel =>
            {
                ModConfig.Instance.ExtendedPanelChartHelp = ExtendedPanelChartHelpValues[sel];
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.ExtendedPanelChartOverlayTextScale;

            group.AddSlider("Chart Overlay Text Scale", 0.5f, 1.5f, 0.1f, selectedValue, sel =>
            {
                ModConfig.Instance.ExtendedPanelChartOverlayTextScale = sel;
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.ExtendedPanelChartLegendTextScale;

            group.AddSlider("Chart Legend Text Scale", 0.1f, 0.6f, 0.05f, selectedValue, sel =>
            {
                ModConfig.Instance.ExtendedPanelChartLegendTextScale = sel;
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.ExtendedPanelChartIconSize;

            group.AddSlider("Chart Icon Size", 15f, 40f, 2.5f, selectedValue, sel =>
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
