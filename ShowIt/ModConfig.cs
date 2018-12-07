namespace ShowIt
{
    [ConfigurationPath("ShowItConfig.xml")]
    public class ModConfig
    {
        public bool ConfigUpdated { get; set; }
        public string ExtendedPanelAlignment { get; set; }
        public string ExtendedPanelBackgroundSprite { get; set; }
        public float ExtendedPanelOpacity { get; set; }
        public string ExtendedPanelChartHelp { get; set; }
        public float ExtendedPanelChartOverlayTextScale { get; set; }
        public float ExtendedPanelChartLegendTextScale { get; set; }
        public float ExtendedPanelChartIconSize { get; set; }

        private static ModConfig instance;

        public static ModConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Configuration<ModConfig>.Load();
                }

                return instance;
            }
        }

        public void Save()
        {
            Configuration<ModConfig>.Save();
            ConfigUpdated = true;
        }
    }
}