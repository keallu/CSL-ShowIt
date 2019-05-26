using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ShowIt
{
    public class ZonedBuildingExtenderPanel : MonoBehaviour
    {
        private bool _initialized;
        private ushort _cachedBuildingID;

        private Dictionary<int, float> _effectsOnZonedBuilding;
        private Dictionary<int, float> _maxEffectsOnZonedBuilding;

        private ZonedBuildingWorldInfoPanel _zonedBuildingWorldInfoPanel;
        private UIPanel _extenderPanel;

        private UILabel _headingLabel;
        private UISprite _effectChartHover;
        private Dictionary<int, UIRadialChart> _effectCharts;
        private Dictionary<int, UILabel> _effectOverlays;
        private Dictionary<int, UISprite> _effectIcons;

        public void Awake()
        {
            try
            {
                _effectsOnZonedBuilding = new Dictionary<int, float>();
                _maxEffectsOnZonedBuilding = new Dictionary<int, float>();
                _effectCharts = new Dictionary<int, UIRadialChart>();
                _effectOverlays = new Dictionary<int, UILabel>();
                _effectIcons = new Dictionary<int, UISprite>();
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ZonedBuildingExtenderPanel:Awake -> Exception: " + e.Message);
            }
        }

        public void Start()
        {
            try
            {
                if (_zonedBuildingWorldInfoPanel == null)
                {
                    _zonedBuildingWorldInfoPanel = GameObject.Find("(Library) ZonedBuildingWorldInfoPanel").GetComponent<ZonedBuildingWorldInfoPanel>();
                }

                CreateUI();
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ZonedBuildingExtenderPanel:Start -> Exception: " + e.Message);
            }
        }

        public void Update()
        {
            try
            {
                if (!_initialized || ModConfig.Instance.ConfigUpdated)
                {
                    UpdateUI();

                    _initialized = true;
                    ModConfig.Instance.ConfigUpdated = false;
                }

                if (!_zonedBuildingWorldInfoPanel.component.isVisible)
                {
                    _cachedBuildingID = 0;
                }
                else
                {
                    UpdateBindings();
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ZonedBuildingExtenderPanel:Update -> Exception: " + e.Message);
            }
        }

        public void OnDestroy()
        {
            try
            {
                for (var i = 0; i < 20; i++)
                {
                    Destroy(_effectCharts[i]);
                    Destroy(_effectOverlays[i]);
                    Destroy(_effectIcons[i]);
                }
                if (_effectChartHover != null)
                {
                    Destroy(_effectChartHover);
                }
                if (_headingLabel != null)
                {
                    Destroy(_headingLabel);
                }
                if (_extenderPanel != null)
                {
                    Destroy(_extenderPanel);
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ZonedBuildingExtenderPanel:OnDestroy -> Exception: " + e.Message);
            }
        }

        private void CreateUI()
        {
            try
            {
                _extenderPanel = UIUtils.CreatePanel(_zonedBuildingWorldInfoPanel.component, "ShowItZonedBuildingExtenderPanel");
                _extenderPanel.backgroundSprite = "SubcategoriesPanel";
                _extenderPanel.eventMouseEnter += (component, eventParam) =>
                {
                    _extenderPanel.opacity = ModConfig.Instance.ExtendedPanelOpacityWhenHover;
                };
                _extenderPanel.eventMouseLeave += (component, eventParam) =>
                {
                    _extenderPanel.opacity = ModConfig.Instance.ExtendedPanelOpacity;
                };

                _headingLabel = UIUtils.CreateLabel(_extenderPanel, "ShowItZonedBuildingExtenderPanelHeading", "Indicators");
                _headingLabel.textAlignment = UIHorizontalAlignment.Center;

                _effectChartHover = UIUtils.CreateSprite(_extenderPanel, "ShowItZonedBuildingExtenderPanelEffectChartHover", "ToolbarIconGroup1Hovered");
                _effectChartHover.size = new Vector3(55f, 55f);
                _effectChartHover.isVisible = false;

                UIRadialChart effectChart;
                UILabel effectOverlay;
                UISprite effectIcon;

                for (var i = 0; i < 20; i++)
                {
                    effectChart = UIUtils.CreateTwoSlicedRadialChart(_extenderPanel, "ShowItZonedBuildingExtenderPanelEffectChart" + i);
                    effectChart.eventMouseEnter += (component, eventParam) =>
                    {
                        _effectChartHover.relativePosition = new Vector3(component.relativePosition.x - 2.5f, component.relativePosition.y - 2.5f);
                        _effectChartHover.isVisible = true;
                    };
                    effectChart.eventMouseLeave += (component, eventParam) =>
                    {
                        _effectChartHover.isVisible = false;
                    };
                    effectChart.eventClick += (component, eventParam) =>
                    {
                        InfoManager.InfoMode infoMode = InfoManager.InfoMode.LandValue;
                        InfoManager.SubInfoMode subInfoMode = InfoManager.SubInfoMode.Default;
                        GetEffectInfoModes((ImmaterialResourceManager.Resource)component.objectUserData, out infoMode, out subInfoMode);
                        Singleton<InfoManager>.instance.SetCurrentMode(infoMode, subInfoMode);
                    };
                    _effectCharts.Add(i, effectChart);

                    effectOverlay = UIUtils.CreateLabel(effectChart, "ShowItZonedBuildingExtenderPanelOverlayLabel" + i, "");
                    effectOverlay.textAlignment = UIHorizontalAlignment.Center;
                    _effectOverlays.Add(i, effectOverlay);

                    effectIcon = UIUtils.CreateSprite(effectChart, "ShowItZonedBuildingExtenderPanelIconSprite" + i, "");
                    _effectIcons.Add(i, effectIcon);
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ZonedBuildingExtenderPanel:CreateUI -> Exception: " + e.Message);
            }
        }

        private void UpdateUI()
        {
            try
            {
                int columns;
                float gap;
                float separator;

                if (ModConfig.Instance.ExtendedPanelAlignment is "Bottom")
                {
                    _extenderPanel.AlignTo(_extenderPanel.parent, UIAlignAnchor.BottomLeft);
                    _extenderPanel.width = _extenderPanel.parent.width;
                    _extenderPanel.height = 350f;
                    _extenderPanel.relativePosition = new Vector3(0f, _extenderPanel.parent.height - 15f);

                    columns = 6;
                    separator = 23;
                    gap = 20;
                }
                else
                {
                    _extenderPanel.AlignTo(_extenderPanel.parent, UIAlignAnchor.TopRight);
                    _extenderPanel.width = 340f;
                    _extenderPanel.height = _extenderPanel.parent.height - 15f;
                    _extenderPanel.relativePosition = new Vector3(_extenderPanel.parent.width + 1f, 0f);

                    columns = 5;
                    separator = 15;
                    gap = 10;
                }

                _extenderPanel.opacity = ModConfig.Instance.ExtendedPanelOpacity;

                _headingLabel.relativePosition = new Vector3(_extenderPanel.width / 2f - _headingLabel.width / 2f, _headingLabel.height / 2f + 5f);

                UIRadialChart effectChart;
                UILabel effectOverlay;
                UISprite effectIcon;

                for (var i = 0; i < 20; i++)
                {
                    effectChart = _effectCharts[i];
                    effectChart.AlignTo(_extenderPanel, UIAlignAnchor.TopRight);
                    effectChart.relativePosition = new Vector3((float)separator + i % columns * (effectChart.width + separator), (float)(separator + effectChart.width) + i / columns * (separator + effectChart.width) - gap);

                    effectOverlay = _effectOverlays[i];
                    effectOverlay.textAlignment = UIHorizontalAlignment.Center;
                    effectOverlay.textScale = ModConfig.Instance.ExtendedPanelChartOverlayTextScale;

                    effectIcon = _effectIcons[i];
                    effectIcon.size = new Vector3(ModConfig.Instance.ExtendedPanelChartIconSize, ModConfig.Instance.ExtendedPanelChartIconSize);
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ZonedBuildingExtenderPanel:UpdateUI -> Exception: " + e.Message);
            }
        }

        private void UpdateBindings()
        {
            try
            {
                ushort buildingId = ((InstanceID)_zonedBuildingWorldInfoPanel
                        .GetType()
                        .GetField("m_InstanceID", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(_zonedBuildingWorldInfoPanel))
                        .Building;

                if (_cachedBuildingID == 0 || _cachedBuildingID != buildingId)
                {
                    Building building = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingId];

                    _effectsOnZonedBuilding.Clear();
                    _maxEffectsOnZonedBuilding.Clear();

                    float effect;
                    float maxEffect;

                    for (var i = 0; i < ImmaterialResourceManager.RESOURCE_COUNT; i++)
                    {
                        switch (building.Info.m_class.GetZone())
                        {
                            case ItemClass.Zone.ResidentialHigh:
                            case ItemClass.Zone.ResidentialLow:
                                effect = ResidentialBuildingHelper.CalculateResourceEffect(buildingId, ref building, (ImmaterialResourceManager.Resource)i);
                                maxEffect = ResidentialBuildingHelper.GetMaxEffect((ImmaterialResourceManager.Resource)i);
                                break;
                            case ItemClass.Zone.Industrial:
                                effect = IndustrialBuildingHelper.CalculateResourceEffect(buildingId, ref building, (ImmaterialResourceManager.Resource)i);
                                maxEffect = IndustrialBuildingHelper.GetMaxEffect((ImmaterialResourceManager.Resource)i);
                                break;
                            case ItemClass.Zone.CommercialHigh:
                            case ItemClass.Zone.CommercialLow:
                                effect = CommercialBuildingHelper.CalculateResourceEffect(buildingId, ref building, (ImmaterialResourceManager.Resource)i);
                                maxEffect = CommercialBuildingHelper.GetMaxEffect((ImmaterialResourceManager.Resource)i);
                                break;
                            case ItemClass.Zone.Office:
                                effect = OfficeBuildingHelper.CalculateResourceEffect(buildingId, ref building, (ImmaterialResourceManager.Resource)i);
                                maxEffect = OfficeBuildingHelper.GetMaxEffect((ImmaterialResourceManager.Resource)i);
                                break;
                            default:
                                effect = 0;
                                maxEffect = 0;
                                break;
                        }

                        _effectsOnZonedBuilding.Add(i, effect);
                        _maxEffectsOnZonedBuilding.Add(i, maxEffect);
                    }

                    ResetAllEffectCharts();

                    switch (building.Info.m_class.GetZone())
                    {
                        case ItemClass.Zone.ResidentialHigh:
                        case ItemClass.Zone.ResidentialLow:
                            ShowResidentialEffectCharts();
                            break;
                        case ItemClass.Zone.Industrial:
                            ShowIndustrialEffectCharts();
                            break;
                        case ItemClass.Zone.CommercialHigh:
                        case ItemClass.Zone.CommercialLow:
                            ShowCommercialEffectCharts();
                            break;
                        case ItemClass.Zone.Office:
                            ShowOfficeEffectCharts();
                            break;
                        default:

                            break;
                    }

                    _cachedBuildingID = buildingId;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ZonedBuildingExtenderPanel:UpdateBindings -> Exception: " + e.Message);
            }
        }

        private void ShowResidentialEffectCharts()
        {
            try
            {
                SetEffectChart(0, ImmaterialResourceManager.Resource.HealthCare);
                SetEffectChart(1, ImmaterialResourceManager.Resource.DeathCare);
                SetEffectChart(2, ImmaterialResourceManager.Resource.FireDepartment);
                SetEffectChart(3, ImmaterialResourceManager.Resource.PoliceDepartment);
                SetEffectChart(4, ImmaterialResourceManager.Resource.EducationElementary);
                SetEffectChart(5, ImmaterialResourceManager.Resource.EducationHighSchool);
                SetEffectChart(6, ImmaterialResourceManager.Resource.EducationUniversity);
                SetEffectChart(7, ImmaterialResourceManager.Resource.PublicTransport);
                SetEffectChart(8, ImmaterialResourceManager.Resource.PostService);
                SetEffectChart(9, ImmaterialResourceManager.Resource.Entertainment);
                SetEffectChart(10, ImmaterialResourceManager.Resource.Health);
                SetEffectChart(11, ImmaterialResourceManager.Resource.Wellbeing);
                SetEffectChart(12, ImmaterialResourceManager.Resource.FirewatchCoverage);
                SetEffectChart(13, ImmaterialResourceManager.Resource.DisasterCoverage);
                SetEffectChart(14, ImmaterialResourceManager.Resource.RadioCoverage);
                SetEffectChart(15, ImmaterialResourceManager.Resource.FireHazard);
                SetEffectChart(16, ImmaterialResourceManager.Resource.CrimeRate);
                SetEffectChart(17, ImmaterialResourceManager.Resource.NoisePollution);
                SetEffectChart(18, ImmaterialResourceManager.Resource.Abandonment);

                _effectCharts[19].isVisible = false;
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ZonedBuildingExtenderPanel:ShowResidentialEffectCharts -> Exception: " + e.Message);
            }
        }

        private void ShowIndustrialEffectCharts()
        {
            try
            {
                SetEffectChart(0, ImmaterialResourceManager.Resource.HealthCare);
                SetEffectChart(1, ImmaterialResourceManager.Resource.DeathCare);
                SetEffectChart(2, ImmaterialResourceManager.Resource.FireDepartment);
                SetEffectChart(3, ImmaterialResourceManager.Resource.PoliceDepartment);
                SetEffectChart(4, ImmaterialResourceManager.Resource.EducationElementary);
                SetEffectChart(5, ImmaterialResourceManager.Resource.EducationHighSchool);
                SetEffectChart(6, ImmaterialResourceManager.Resource.EducationUniversity);
                SetEffectChart(7, ImmaterialResourceManager.Resource.PublicTransport);
                SetEffectChart(8, ImmaterialResourceManager.Resource.PostService);
                SetEffectChart(9, ImmaterialResourceManager.Resource.Entertainment);
                SetEffectChart(10, ImmaterialResourceManager.Resource.Health);
                SetEffectChart(11, ImmaterialResourceManager.Resource.Wellbeing);
                SetEffectChart(12, ImmaterialResourceManager.Resource.FirewatchCoverage);
                SetEffectChart(13, ImmaterialResourceManager.Resource.DisasterCoverage);
                SetEffectChart(14, ImmaterialResourceManager.Resource.RadioCoverage);
                SetEffectChart(15, ImmaterialResourceManager.Resource.CargoTransport);
                SetEffectChart(16, ImmaterialResourceManager.Resource.FireHazard);
                SetEffectChart(17, ImmaterialResourceManager.Resource.CrimeRate);
                SetEffectChart(18, ImmaterialResourceManager.Resource.NoisePollution);
                SetEffectChart(19, ImmaterialResourceManager.Resource.Abandonment);

            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ZonedBuildingExtenderPanel:ShowIndustrialEffectCharts -> Exception: " + e.Message);
            }
        }

        private void ShowCommercialEffectCharts()
        {
            try
            {
                SetEffectChart(0, ImmaterialResourceManager.Resource.HealthCare);
                SetEffectChart(1, ImmaterialResourceManager.Resource.DeathCare);
                SetEffectChart(2, ImmaterialResourceManager.Resource.FireDepartment);
                SetEffectChart(3, ImmaterialResourceManager.Resource.PoliceDepartment);
                SetEffectChart(4, ImmaterialResourceManager.Resource.EducationElementary);
                SetEffectChart(5, ImmaterialResourceManager.Resource.EducationHighSchool);
                SetEffectChart(6, ImmaterialResourceManager.Resource.EducationUniversity);
                SetEffectChart(7, ImmaterialResourceManager.Resource.PublicTransport);
                SetEffectChart(8, ImmaterialResourceManager.Resource.PostService);
                SetEffectChart(9, ImmaterialResourceManager.Resource.Entertainment);
                SetEffectChart(10, ImmaterialResourceManager.Resource.Health);
                SetEffectChart(11, ImmaterialResourceManager.Resource.Wellbeing);
                SetEffectChart(12, ImmaterialResourceManager.Resource.FirewatchCoverage);
                SetEffectChart(13, ImmaterialResourceManager.Resource.DisasterCoverage);
                SetEffectChart(14, ImmaterialResourceManager.Resource.RadioCoverage);
                SetEffectChart(15, ImmaterialResourceManager.Resource.CargoTransport);
                SetEffectChart(16, ImmaterialResourceManager.Resource.FireHazard);
                SetEffectChart(17, ImmaterialResourceManager.Resource.CrimeRate);
                SetEffectChart(18, ImmaterialResourceManager.Resource.NoisePollution);
                SetEffectChart(19, ImmaterialResourceManager.Resource.Abandonment);
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ZonedBuildingExtenderPanel:ShowCommercialEffectCharts -> Exception: " + e.Message);
            }
        }

        private void ShowOfficeEffectCharts()
        {
            try
            {
                SetEffectChart(0, ImmaterialResourceManager.Resource.HealthCare);
                SetEffectChart(1, ImmaterialResourceManager.Resource.DeathCare);
                SetEffectChart(2, ImmaterialResourceManager.Resource.FireDepartment);
                SetEffectChart(3, ImmaterialResourceManager.Resource.PoliceDepartment);
                SetEffectChart(4, ImmaterialResourceManager.Resource.EducationElementary);
                SetEffectChart(5, ImmaterialResourceManager.Resource.EducationHighSchool);
                SetEffectChart(6, ImmaterialResourceManager.Resource.EducationUniversity);
                SetEffectChart(7, ImmaterialResourceManager.Resource.PublicTransport);
                SetEffectChart(8, ImmaterialResourceManager.Resource.PostService);
                SetEffectChart(9, ImmaterialResourceManager.Resource.Entertainment);
                SetEffectChart(10, ImmaterialResourceManager.Resource.Health);
                SetEffectChart(11, ImmaterialResourceManager.Resource.Wellbeing);
                SetEffectChart(12, ImmaterialResourceManager.Resource.FirewatchCoverage);
                SetEffectChart(13, ImmaterialResourceManager.Resource.DisasterCoverage);
                SetEffectChart(14, ImmaterialResourceManager.Resource.RadioCoverage);
                SetEffectChart(15, ImmaterialResourceManager.Resource.FireHazard);
                SetEffectChart(16, ImmaterialResourceManager.Resource.CrimeRate);
                SetEffectChart(17, ImmaterialResourceManager.Resource.NoisePollution);
                SetEffectChart(18, ImmaterialResourceManager.Resource.Abandonment);

                _effectCharts[19].isVisible = false;
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ZonedBuildingExtenderPanel:ShowOfficeEffectCharts -> Exception: " + e.Message);
            }
        }

        private void ResetAllEffectCharts()
        {
            try
            {
                foreach (UIRadialChart chart in _effectCharts.Values)
                {
                    chart.isVisible = true;
                }

                foreach (UILabel overlay in _effectOverlays.Values)
                {
                    overlay.isVisible = true;
                }

                foreach (UISprite icon in _effectIcons.Values)
                {
                    icon.isVisible = true;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ZonedBuildingExtenderPanel:ResetAllEffectCharts -> Exception: " + e.Message);
            }
        }

        private void SetEffectChart(int index, ImmaterialResourceManager.Resource resource)
        {
            try
            {
                int resourceKey = (int)resource;
                float resourceEffect = _effectsOnZonedBuilding[resourceKey];
                float resourceMaxEffect = _maxEffectsOnZonedBuilding[resourceKey];
                float resourceEffectPercentage = resourceEffect / resourceMaxEffect;

                Color32 colorRed = new Color32(255, 0, 0, 128);
                Color32 colorYellow = new Color32(255, 255, 0, 128);
                Color32 colorGreen = new Color32(0, 255, 0, 128);
                Color32 color;

                if (resourceEffectPercentage > 0.66f)
                {
                    color = IsEffectPositive(resource) ? colorGreen : colorRed;
                }
                else if (resourceEffectPercentage > 0.33f)
                {
                    color = IsEffectPositive(resource) ? colorYellow : colorRed;
                }
                else
                {
                    color = IsEffectPositive(resource) ? colorRed : colorRed;
                }

                _effectCharts[index].GetSlice(0).outterColor = color;
                _effectCharts[index].GetSlice(0).innerColor = color;

                _effectCharts[index].SetValues(resourceEffectPercentage, 1 - resourceEffectPercentage);
                _effectCharts[index].tooltip = GetEffectLongName(resource);
                _effectCharts[index].objectUserData = resource;

                _effectOverlays[index].text = $"{Math.Round(resourceEffectPercentage * 100f),1}%";
                _effectOverlays[index].relativePosition = new Vector3(_effectCharts[index].width / 2f - _effectOverlays[index].width / 2f, _effectCharts[index].height / 2f - _effectOverlays[index].height / 2f);

                _effectIcons[index].spriteName = GetEffectSprite(resource);
                _effectIcons[index].relativePosition = new Vector3(_effectCharts[index].width / 2f - _effectIcons[index].width / 2f, _effectIcons[index].height / 2f + _effectCharts[index].height / 2);

            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ZonedBuildingExtenderPanel:SetEffectChart -> Exception: " + e.Message);
            }
        }

        private bool IsEffectPositive(ImmaterialResourceManager.Resource resource)
        {
            switch (resource)
            {
                case ImmaterialResourceManager.Resource.HealthCare:
                    return true;
                case ImmaterialResourceManager.Resource.FireDepartment:
                    return true;
                case ImmaterialResourceManager.Resource.PoliceDepartment:
                    return true;
                case ImmaterialResourceManager.Resource.EducationElementary:
                    return true;
                case ImmaterialResourceManager.Resource.EducationHighSchool:
                    return true;
                case ImmaterialResourceManager.Resource.EducationUniversity:
                    return true;
                case ImmaterialResourceManager.Resource.EducationLibrary:
                    return true;
                case ImmaterialResourceManager.Resource.DeathCare:
                    return true;
                case ImmaterialResourceManager.Resource.PublicTransport:
                    return true;
                case ImmaterialResourceManager.Resource.NoisePollution:
                    return false;
                case ImmaterialResourceManager.Resource.CrimeRate:
                    return false;
                case ImmaterialResourceManager.Resource.Health:
                    return true;
                case ImmaterialResourceManager.Resource.Wellbeing:
                    return true;
                case ImmaterialResourceManager.Resource.Density:
                    return true;
                case ImmaterialResourceManager.Resource.Entertainment:
                    return true;
                case ImmaterialResourceManager.Resource.LandValue:
                    return true;
                case ImmaterialResourceManager.Resource.Attractiveness:
                    return true;
                case ImmaterialResourceManager.Resource.Coverage:
                    return true;
                case ImmaterialResourceManager.Resource.FireHazard:
                    return false;
                case ImmaterialResourceManager.Resource.Abandonment:
                    return false;
                case ImmaterialResourceManager.Resource.CargoTransport:
                    return true;
                case ImmaterialResourceManager.Resource.RadioCoverage:
                    return true;
                case ImmaterialResourceManager.Resource.FirewatchCoverage:
                    return true;
                case ImmaterialResourceManager.Resource.EarthquakeCoverage:
                    return true;
                case ImmaterialResourceManager.Resource.DisasterCoverage:
                    return true;
                case ImmaterialResourceManager.Resource.TourCoverage:
                    return true;
                case ImmaterialResourceManager.Resource.PostService:
                    return true;
                case ImmaterialResourceManager.Resource.None:
                    return false;
                default:
                    return false;
            }
        }

        private string GetEffectLongName(ImmaterialResourceManager.Resource resource)
        {
            switch (resource)
            {
                case ImmaterialResourceManager.Resource.HealthCare:
                    return "Health Care";
                case ImmaterialResourceManager.Resource.FireDepartment:
                    return "Fire Department";
                case ImmaterialResourceManager.Resource.PoliceDepartment:
                    return "Police Department";
                case ImmaterialResourceManager.Resource.EducationElementary:
                    return "Elementary";
                case ImmaterialResourceManager.Resource.EducationHighSchool:
                    return "High School";
                case ImmaterialResourceManager.Resource.EducationUniversity:
                    return "University";
                case ImmaterialResourceManager.Resource.EducationLibrary:
                    return "Library";
                case ImmaterialResourceManager.Resource.DeathCare:
                    return "Death Care";
                case ImmaterialResourceManager.Resource.PublicTransport:
                    return "Public Transport";
                case ImmaterialResourceManager.Resource.NoisePollution:
                    return "Noise Pollution";
                case ImmaterialResourceManager.Resource.CrimeRate:
                    return "Crime Rate";
                case ImmaterialResourceManager.Resource.Health:
                    return "Health";
                case ImmaterialResourceManager.Resource.Wellbeing:
                    return "Wellbeing";
                case ImmaterialResourceManager.Resource.Density:
                    return "Density";
                case ImmaterialResourceManager.Resource.Entertainment:
                    return "Entertainment";
                case ImmaterialResourceManager.Resource.LandValue:
                    return "Land Value";
                case ImmaterialResourceManager.Resource.Attractiveness:
                    return "Attractiveness";
                case ImmaterialResourceManager.Resource.Coverage:
                    return "Coverage";
                case ImmaterialResourceManager.Resource.FireHazard:
                    return "Fire Hazard";
                case ImmaterialResourceManager.Resource.Abandonment:
                    return "Abandonment";
                case ImmaterialResourceManager.Resource.CargoTransport:
                    return "Cargo Transport";
                case ImmaterialResourceManager.Resource.RadioCoverage:
                    return "Radio Coverage";
                case ImmaterialResourceManager.Resource.FirewatchCoverage:
                    return "Firewatch Coverage";
                case ImmaterialResourceManager.Resource.EarthquakeCoverage:
                    return "Earthquake Coverage";
                case ImmaterialResourceManager.Resource.DisasterCoverage:
                    return "Disaster Coverage";
                case ImmaterialResourceManager.Resource.TourCoverage:
                    return "Tour Coverage";
                case ImmaterialResourceManager.Resource.PostService:
                    return "Post Service";
                case ImmaterialResourceManager.Resource.None:
                    return "None";
                default:
                    return "Unknown";
            }
        }

        private string GetEffectSprite(ImmaterialResourceManager.Resource resource)
        {
            switch (resource)
            {
                case ImmaterialResourceManager.Resource.HealthCare:
                    return "ToolbarIconHealthcare";
                case ImmaterialResourceManager.Resource.FireDepartment:
                    return "ToolbarIconFireDepartment";
                case ImmaterialResourceManager.Resource.PoliceDepartment:
                    return "ToolbarIconPolice";
                case ImmaterialResourceManager.Resource.EducationElementary:
                    return "ToolbarIconEducation";
                case ImmaterialResourceManager.Resource.EducationHighSchool:
                    return "ToolbarIconEducation";
                case ImmaterialResourceManager.Resource.EducationUniversity:
                    return "ToolbarIconEducation";
                case ImmaterialResourceManager.Resource.EducationLibrary:
                    return "ToolbarIconEducation";
                case ImmaterialResourceManager.Resource.DeathCare:
                    return "NotificationIconDead";
                case ImmaterialResourceManager.Resource.PublicTransport:
                    return "ToolbarIconPublicTransport";
                case ImmaterialResourceManager.Resource.NoisePollution:
                    return "InfoIconNoisePollution";
                case ImmaterialResourceManager.Resource.CrimeRate:
                    return "InfoIconCrime";
                case ImmaterialResourceManager.Resource.Health:
                    return "InfoIconHealth";
                case ImmaterialResourceManager.Resource.Wellbeing:
                    return "InfoIconHappiness";
                case ImmaterialResourceManager.Resource.Density:
                    return "InfoIconPopulation";
                case ImmaterialResourceManager.Resource.Entertainment:
                    return "InfoIconEntertainment";
                case ImmaterialResourceManager.Resource.LandValue:
                    return "InfoIconLandValue";
                case ImmaterialResourceManager.Resource.Attractiveness:
                    return "InfoIconLevel";
                case ImmaterialResourceManager.Resource.Coverage:
                    return "ToolbarIconZoomOutGlobe";
                case ImmaterialResourceManager.Resource.FireHazard:
                    return "IconPolicySmokeDetectors";
                case ImmaterialResourceManager.Resource.Abandonment:
                    return "InfoIconDestruction";
                case ImmaterialResourceManager.Resource.CargoTransport:
                    return "InfoIconOutsideConnections";
                case ImmaterialResourceManager.Resource.RadioCoverage:
                    return "InfoIconRadio";
                case ImmaterialResourceManager.Resource.FirewatchCoverage:
                    return "InfoIconForestFire";
                case ImmaterialResourceManager.Resource.EarthquakeCoverage:
                    return "InfoIconEarthquake";
                case ImmaterialResourceManager.Resource.DisasterCoverage:
                    return "InfoIconDisasterDetection";
                case ImmaterialResourceManager.Resource.TourCoverage:
                    return "InfoIconTours";
                case ImmaterialResourceManager.Resource.PostService:
                    return "InfoIconPost";
                case ImmaterialResourceManager.Resource.None:
                    return "ToolbarIconHelp";
                default:
                    return "ToolbarIconHelp";
            }
        }

        private void GetEffectInfoModes(ImmaterialResourceManager.Resource resource, out InfoManager.InfoMode infoMode, out InfoManager.SubInfoMode subInfoMode)
        {
            infoMode = InfoManager.InfoMode.LandValue;
            subInfoMode = InfoManager.SubInfoMode.Default;

            switch (resource)
            {
                case ImmaterialResourceManager.Resource.HealthCare:
                    infoMode = InfoManager.InfoMode.Health;
                    subInfoMode = InfoManager.SubInfoMode.HealthCare;
                    break;
                case ImmaterialResourceManager.Resource.FireDepartment:
                    infoMode = InfoManager.InfoMode.FireSafety;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
                case ImmaterialResourceManager.Resource.PoliceDepartment:
                    infoMode = InfoManager.InfoMode.CrimeRate;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
                case ImmaterialResourceManager.Resource.EducationElementary:
                    infoMode = InfoManager.InfoMode.Education;
                    subInfoMode = InfoManager.SubInfoMode.ElementarySchool;
                    break;
                case ImmaterialResourceManager.Resource.EducationHighSchool:
                    infoMode = InfoManager.InfoMode.Education;
                    subInfoMode = InfoManager.SubInfoMode.HighSchool;
                    break;
                case ImmaterialResourceManager.Resource.EducationUniversity:
                    infoMode = InfoManager.InfoMode.Education;
                    subInfoMode = InfoManager.SubInfoMode.University;
                    break;
                case ImmaterialResourceManager.Resource.EducationLibrary:
                    infoMode = InfoManager.InfoMode.Education;
                    subInfoMode = InfoManager.SubInfoMode.LibraryEducation;
                    break;
                case ImmaterialResourceManager.Resource.DeathCare:
                    infoMode = InfoManager.InfoMode.Health;
                    subInfoMode = InfoManager.SubInfoMode.DeathCare;
                    break;
                case ImmaterialResourceManager.Resource.PublicTransport:
                    infoMode = InfoManager.InfoMode.Transport;
                    subInfoMode = InfoManager.SubInfoMode.NormalTransport;
                    break;
                case ImmaterialResourceManager.Resource.NoisePollution:
                    infoMode = InfoManager.InfoMode.NoisePollution;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
                case ImmaterialResourceManager.Resource.CrimeRate:
                    infoMode = InfoManager.InfoMode.CrimeRate;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
                case ImmaterialResourceManager.Resource.Health:
                    infoMode = InfoManager.InfoMode.Health;
                    subInfoMode = InfoManager.SubInfoMode.HealthCare;
                    break;
                case ImmaterialResourceManager.Resource.Wellbeing:
                    infoMode = InfoManager.InfoMode.Happiness;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
                case ImmaterialResourceManager.Resource.Density:
                    infoMode = InfoManager.InfoMode.Density;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
                case ImmaterialResourceManager.Resource.Entertainment:
                    infoMode = InfoManager.InfoMode.Entertainment;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
                case ImmaterialResourceManager.Resource.LandValue:
                    infoMode = InfoManager.InfoMode.LandValue;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
                case ImmaterialResourceManager.Resource.Attractiveness:
                    infoMode = InfoManager.InfoMode.Tourism;
                    subInfoMode = InfoManager.SubInfoMode.Attractiveness;
                    break;
                case ImmaterialResourceManager.Resource.Coverage:
                    infoMode = InfoManager.InfoMode.LandValue;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
                case ImmaterialResourceManager.Resource.FireHazard:
                    infoMode = InfoManager.InfoMode.FireSafety;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
                case ImmaterialResourceManager.Resource.Abandonment:
                    infoMode = InfoManager.InfoMode.Destruction;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
                case ImmaterialResourceManager.Resource.CargoTransport:
                    infoMode = InfoManager.InfoMode.Connections;
                    subInfoMode = InfoManager.SubInfoMode.Import;
                    break;
                case ImmaterialResourceManager.Resource.RadioCoverage:
                    infoMode = InfoManager.InfoMode.Radio;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
                case ImmaterialResourceManager.Resource.FirewatchCoverage:
                    infoMode = InfoManager.InfoMode.FireSafety;
                    subInfoMode = InfoManager.SubInfoMode.ForestFireHazard;
                    break;
                case ImmaterialResourceManager.Resource.EarthquakeCoverage:
                    infoMode = InfoManager.InfoMode.DisasterDetection;
                    subInfoMode = InfoManager.SubInfoMode.EarthquakeDetection;
                    break;
                case ImmaterialResourceManager.Resource.DisasterCoverage:
                    infoMode = InfoManager.InfoMode.DisasterDetection;
                    subInfoMode = InfoManager.SubInfoMode.DisasterDetection;
                    break;
                case ImmaterialResourceManager.Resource.TourCoverage:
                    infoMode = InfoManager.InfoMode.Tours;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
                case ImmaterialResourceManager.Resource.PostService:
                    infoMode = InfoManager.InfoMode.Post;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
                case ImmaterialResourceManager.Resource.None:
                    infoMode = InfoManager.InfoMode.LandValue;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
                default:
                    infoMode = InfoManager.InfoMode.LandValue;
                    subInfoMode = InfoManager.SubInfoMode.Default;
                    break;
            }
        }
    }
}
