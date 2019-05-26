namespace ShowIt
{
    [ConfigurationPath("ShowItConfig.xml")]
    public class ModConfig
    {
        public bool ConfigUpdated { get; set; }
        public string ExtendedPanelAlignment { get; set; } = "Right";
        public float ExtendedPanelOpacity { get; set; } = 0.75f;
        public float ExtendedPanelOpacityWhenHover { get; set; } = 0.95f;
        public float ExtendedPanelChartOverlayTextScale { get; set; } = 0.75f;
        public float ExtendedPanelChartIconSize { get; set; } = 25f;

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