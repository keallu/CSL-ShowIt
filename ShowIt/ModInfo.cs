using ICities;
using System;
using System.Reflection;

namespace ShowIt
{
    public class ModInfo : IUserMod
    {
        public string Name => "Show It!";
        public string Description => "Shows vital indicators for zoned buildings.";

        private static readonly string[] IndicatorsPanelAlignmentLabels =
        {
            "Right",
            "Bottom"
        };

        private static readonly string[] IndicatorsPanelAlignmentValues =
        {
            "Right",
            "Bottom"
        };

        private static readonly string[] IndicatorsPanelLegendLabels =
         {
            "Icons",
            "Labels",
            "Both"
        };

        private static readonly string[] IndicatorsPanelLegendValues =
        {
            "Icons",
            "Labels",
            "Both"
        };

        public void OnSettingsUI(UIHelperBase helper)
        {
            UIHelperBase group;

            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();

            group = helper.AddGroup(Name + " - " + assemblyName.Version.Major + "." + assemblyName.Version.Minor);

            int selectedIndex;
            float selectedValue;

            selectedIndex = GetSelectedOptionIndex(IndicatorsPanelAlignmentValues, ModConfig.Instance.IndicatorsPanelAlignment);
            group.AddDropdown("Alignment", IndicatorsPanelAlignmentLabels, selectedIndex, sel =>
            {
                ModConfig.Instance.IndicatorsPanelAlignment = IndicatorsPanelAlignmentValues[sel];
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.IndicatorsPanelChartSize;
            group.AddSlider("Chart Size", 35f, 65f, 0.5f, selectedValue, sel =>
            {
                ModConfig.Instance.IndicatorsPanelChartSize = sel;
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.IndicatorsPanelChartHorizontalSpacing;
            group.AddSlider("Chart Horizontal Spacing", 5f, 25f, 1f, selectedValue, sel =>
            {
                ModConfig.Instance.IndicatorsPanelChartHorizontalSpacing = sel;
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.IndicatorsPanelChartVerticalSpacing;
            group.AddSlider("Chart Vertical Spacing", 5f, 25f, 1f, selectedValue, sel =>
            {
                ModConfig.Instance.IndicatorsPanelChartVerticalSpacing = sel;
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.IndicatorsPanelNumberTextScale;
            group.AddSlider("Number Text Scale", 0.5f, 0.9f, 0.05f, selectedValue, sel =>
            {
                ModConfig.Instance.IndicatorsPanelNumberTextScale = sel;
                ModConfig.Instance.Save();
            });

            selectedIndex = GetSelectedOptionIndex(IndicatorsPanelLegendValues, ModConfig.Instance.IndicatorsPanelLegend);
            group.AddDropdown("Legend", IndicatorsPanelLegendLabels, selectedIndex, sel =>
            {
                ModConfig.Instance.IndicatorsPanelLegend = IndicatorsPanelLegendValues[sel];
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.IndicatorsPanelIconSize;
            group.AddSlider("Icon Size", 15f, 25f, 0.5f, selectedValue, sel =>
            {
                ModConfig.Instance.IndicatorsPanelIconSize = sel;
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.IndicatorsPanelLabelTextScale;
            group.AddSlider("Label Text Scale", 0.3f, 0.9f, 0.05f, selectedValue, sel =>
            {
                ModConfig.Instance.IndicatorsPanelLabelTextScale = sel;
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
