using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ShowIt
{
    public class ModManager : MonoBehaviour
    {
        private bool _initialized;

        private ZonedBuildingWorldInfoPanel _zonedBuildingWorldInfoPanel;
        private UIPanel _makeHistoricalPanel;
        private UICheckBox _indicatorsCheckBox;
        private UIPanel _indicatorsPanel;
        private UILabel _header;
        private Dictionary<int, UIRadialChart> _charts;
        private Dictionary<int, UILabel> _numbers;
        private Dictionary<int, UISprite> _icons;
        private Dictionary<int, UILabel> _labels;

        private ushort _cachedBuildingID;
        private Dictionary<int, float> _effectsOnZonedBuilding;
        private Dictionary<int, float> _maxEffectsOnZonedBuilding;

        private const int MaxNumberOfCharts = 16;

        public void Awake()
        {
            try
            {
                _charts = new Dictionary<int, UIRadialChart>();
                _numbers = new Dictionary<int, UILabel>();
                _icons = new Dictionary<int, UISprite>();
                _labels = new Dictionary<int, UILabel>();

                _effectsOnZonedBuilding = new Dictionary<int, float>();
                _maxEffectsOnZonedBuilding = new Dictionary<int, float>();
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ModManager:Awake -> Exception: " + e.Message);
            }
        }

        public void Start()
        {
            try
            {
                _zonedBuildingWorldInfoPanel = GameObject.Find("(Library) ZonedBuildingWorldInfoPanel").GetComponent<ZonedBuildingWorldInfoPanel>();
                _makeHistoricalPanel = _zonedBuildingWorldInfoPanel.Find("MakeHistoricalPanel").GetComponent<UIPanel>();

                CreateUI();
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ModManager:Start -> Exception: " + e.Message);
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

                if (!_zonedBuildingWorldInfoPanel.component.isVisible || !_indicatorsPanel.isVisible)
                {
                    _cachedBuildingID = 0;
                }
                else
                {
                    RefreshData();
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ModManager:Update -> Exception: " + e.Message);
            }
        }

        public void OnDestroy()
        {
            try
            {
                for (var i = 0; i < MaxNumberOfCharts; i++)
                {
                    Destroy(_charts[i]);
                    Destroy(_numbers[i]);
                    Destroy(_labels[i]);
                    Destroy(_icons[i]);
                }
                if (_header != null)
                {
                    Destroy(_header);
                }
                if (_indicatorsPanel != null)
                {
                    Destroy(_indicatorsPanel);
                }
                if (_indicatorsCheckBox != null)
                {
                    Destroy(_indicatorsCheckBox);
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ModManager:OnDestroy -> Exception: " + e.Message);
            }
        }

        private void CreateUI()
        {
            try
            {
                _indicatorsPanel = UIUtils.CreatePanel(_zonedBuildingWorldInfoPanel.component, "ShowItIndicatorsPanel");
                _indicatorsPanel.backgroundSprite = "SubcategoriesPanel";
                _indicatorsPanel.opacity = 0.90f;

                _indicatorsCheckBox = UIUtils.CreateCheckBox(_makeHistoricalPanel, "ShowItIndicatorsCheckBox", "Indicators", ModConfig.Instance.ShowIndicators);
                _indicatorsCheckBox.width = 110f;
                _indicatorsCheckBox.label.textColor = new Color32(185, 221, 254, 255);
                _indicatorsCheckBox.label.textScale = 0.8125f;
                _indicatorsCheckBox.tooltip = "Indicators will show how well serviced the building is and what problems might prevent the building from leveling up.";
                _indicatorsCheckBox.AlignTo(_makeHistoricalPanel, UIAlignAnchor.TopLeft);
                _indicatorsCheckBox.relativePosition = new Vector3(_makeHistoricalPanel.width - _indicatorsCheckBox.width, 6f);
                _indicatorsCheckBox.eventCheckChanged += (component, value) =>
                {
                    _indicatorsPanel.isVisible = value;
                    ModConfig.Instance.ShowIndicators = value;
                    ModConfig.Instance.Save();
                };

                _header = UIUtils.CreateLabel(_indicatorsPanel, "ShowItIndicatorsPanelHeader", "Indicators");
                _header.font = UIUtils.GetUIFont("OpenSans-Regular");
                _header.textAlignment = UIHorizontalAlignment.Center;

                UIRadialChart chart;
                UILabel number;
                UISprite icon;
                UILabel label;

                for (var i = 0; i < MaxNumberOfCharts; i++)
                {
                    chart = UIUtils.CreateTwoSlicedRadialChart(_indicatorsPanel, "ShowItZonedIndicatorsPanelChart" + i);
                    chart.eventClick += (component, eventParam) =>
                    {
                        InfoManager.InfoMode infoMode = InfoManager.InfoMode.LandValue;
                        InfoManager.SubInfoMode subInfoMode = InfoManager.SubInfoMode.Default;
                        GetIndicatorInfoModes((ImmaterialResourceManager.Resource)component.objectUserData, out infoMode, out subInfoMode);
                        Singleton<InfoManager>.instance.SetCurrentMode(infoMode, subInfoMode);
                    };
                    _charts.Add(i, chart);

                    number = UIUtils.CreateLabel(chart, "ShowItIndicatorsPanelNumber" + i, "");
                    number.textAlignment = UIHorizontalAlignment.Center;
                    _numbers.Add(i, number);

                    icon = UIUtils.CreateSprite(chart, "ShowItIndicatorsPanelIcon" + i, "");
                    _icons.Add(i, icon);

                    label = UIUtils.CreateLabel(chart, "ShowItIndicatorsPanelLabel" + i, "");
                    label.font = UIUtils.GetUIFont("OpenSans-Regular");
                    label.textAlignment = UIHorizontalAlignment.Center;
                    label.textColor = new Color32(206, 248, 0, 255);
                    _labels.Add(i, label);
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ModManager:CreateUI -> Exception: " + e.Message);
            }
        }

        private void UpdateUI()
        {
            try
            {
                int rows;
                int columns;
                float horizontalSpacing = ModConfig.Instance.IndicatorsPanelChartHorizontalSpacing;
                float verticalSpacing = ModConfig.Instance.IndicatorsPanelChartVerticalSpacing;
                float topSpacing = 40f;
                float bottomSpacing = 15f;
                float horizontalPadding = 0f;
                float verticalPadding = 0f;

                if (ModConfig.Instance.IndicatorsPanelAlignment is "Right")
                {
                    rows = Mathf.FloorToInt((_indicatorsPanel.parent.height - topSpacing - bottomSpacing - verticalSpacing) / (ModConfig.Instance.IndicatorsPanelChartSize + ModConfig.Instance.IndicatorsPanelChartVerticalSpacing));
                    columns = Mathf.CeilToInt((float)MaxNumberOfCharts / rows);

                    _indicatorsPanel.AlignTo(_indicatorsPanel.parent, UIAlignAnchor.TopRight);
                    _indicatorsPanel.width = columns * (ModConfig.Instance.IndicatorsPanelChartSize + ModConfig.Instance.IndicatorsPanelChartHorizontalSpacing);
                    _indicatorsPanel.height = _indicatorsPanel.parent.height - bottomSpacing;
                    _indicatorsPanel.relativePosition = new Vector3(_indicatorsPanel.parent.width + 1f, 0f);

                    horizontalPadding = ModConfig.Instance.IndicatorsPanelChartHorizontalSpacing / 2f;
                    verticalPadding = (_indicatorsPanel.parent.height - topSpacing - bottomSpacing - rows * (ModConfig.Instance.IndicatorsPanelChartSize + ModConfig.Instance.IndicatorsPanelChartVerticalSpacing)) / 2f;
                }
                else
                {
                    columns = Mathf.FloorToInt((_indicatorsPanel.parent.width - horizontalSpacing) / (ModConfig.Instance.IndicatorsPanelChartSize + ModConfig.Instance.IndicatorsPanelChartHorizontalSpacing));
                    rows = Mathf.CeilToInt((float)MaxNumberOfCharts / columns);

                    _indicatorsPanel.AlignTo(_indicatorsPanel.parent, UIAlignAnchor.BottomLeft);
                    _indicatorsPanel.width = _indicatorsPanel.parent.width;
                    _indicatorsPanel.height = rows * (ModConfig.Instance.IndicatorsPanelChartSize + ModConfig.Instance.IndicatorsPanelChartVerticalSpacing) + topSpacing + bottomSpacing;
                    _indicatorsPanel.relativePosition = new Vector3(0f, _indicatorsPanel.parent.height + 1f);

                    horizontalPadding = (_indicatorsPanel.parent.width - columns * (ModConfig.Instance.IndicatorsPanelChartSize + ModConfig.Instance.IndicatorsPanelChartHorizontalSpacing)) / 2f;
                    verticalPadding = ModConfig.Instance.IndicatorsPanelChartVerticalSpacing / 2f;
                }

                _header.relativePosition = new Vector3(_indicatorsPanel.width / 2f - _header.width / 2f, _header.height / 2f + 5f);

                UIRadialChart chart;

                for (var i = 0; i < MaxNumberOfCharts; i++)
                {
                    chart = _charts[i];
                    chart.AlignTo(_indicatorsPanel, UIAlignAnchor.TopRight);
                    chart.size = new Vector3(ModConfig.Instance.IndicatorsPanelChartSize, ModConfig.Instance.IndicatorsPanelChartSize);
                    chart.relativePosition = new Vector3(horizontalPadding + i % columns * (chart.width + horizontalSpacing), topSpacing + verticalPadding + i / columns * (chart.height + verticalSpacing));

                    _numbers[i].textScale = ModConfig.Instance.IndicatorsPanelNumberTextScale;
                    _icons[i].size = new Vector3(ModConfig.Instance.IndicatorsPanelIconSize, ModConfig.Instance.IndicatorsPanelIconSize);
                    _labels[i].textScale = ModConfig.Instance.IndicatorsPanelLabelTextScale;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ModManager:UpdateUI -> Exception: " + e.Message);
            }
        }

        private void SetChart(int index, ImmaterialResourceManager.Resource resource)
        {
            try
            {
                int resourceKey = (int)resource;
                float resourceEffect = _effectsOnZonedBuilding[resourceKey];
                float resourceMaxEffect = _maxEffectsOnZonedBuilding[resourceKey];
                float resourceEffectPercentage = resourceEffect / resourceMaxEffect;

                Color32 colorRed = new Color32(241, 136, 136, 255);
                Color32 colorYellow = new Color32(251, 212, 0, 255);
                Color32 colorDarkGreen = new Color32(141, 149, 55, 255);
                Color32 colorLightGreen = new Color32(131, 213, 141, 255);
                Color32 color;

                if (resourceEffectPercentage > 0.75f)
                {
                    color = IsIndicatorPositive(resource) ? colorLightGreen : colorRed;
                }
                else if (resourceEffectPercentage > 0.50f)
                {
                    color = IsIndicatorPositive(resource) ? colorDarkGreen : colorRed;
                }
                else if (resourceEffectPercentage > 0.25f)
                {
                    color = IsIndicatorPositive(resource) ? colorYellow : colorRed;
                }
                else
                {
                    color = IsIndicatorPositive(resource) ? colorRed : colorRed;
                }

                _charts[index].GetSlice(0).outterColor = color;
                _charts[index].GetSlice(0).innerColor = color;

                _charts[index].SetValues(resourceEffectPercentage, 1 - resourceEffectPercentage);
                _charts[index].tooltip = GetIndicatorName(resource);
                _charts[index].objectUserData = resource;

                _numbers[index].text = $"{Math.Round(resourceEffectPercentage * 100f),1}%";
                _numbers[index].relativePosition = new Vector3(_charts[index].width / 2f - _numbers[index].width / 2f, _charts[index].height / 2f - _numbers[index].height / 2f);

                _icons[index].spriteName = GetIndicatorSprite(resource);
                _icons[index].tooltip = GetIndicatorName(resource);

                _labels[index].text = GetIndicatorName(resource).Substring(0, 4) + ".";
                _labels[index].tooltip = GetIndicatorName(resource);

                if (ModConfig.Instance.IndicatorsPanelLegend is "Icons")
                {
                    _labels[index].isVisible = false;
                    _icons[index].position = new Vector3(_charts[index].width / 2f - _icons[index].width / 2f, 0f - (ModConfig.Instance.IndicatorsPanelChartSize / 1.25f));
                    _icons[index].isVisible = true;
                }
                else if (ModConfig.Instance.IndicatorsPanelLegend is "Labels")
                {
                    _icons[index].isVisible = false;
                    _labels[index].position = new Vector3(_charts[index].width / 2f - _labels[index].width / 2f, 0f - ModConfig.Instance.IndicatorsPanelChartSize);
                    _labels[index].isVisible = true;
                }
                else
                {
                    _icons[index].position = new Vector3(_charts[index].width / 2f - _icons[index].width / 2f, 0f - (ModConfig.Instance.IndicatorsPanelChartSize / 1.75f));
                    _labels[index].position = new Vector3(_charts[index].width / 2f - _labels[index].width / 2f, 0f - ModConfig.Instance.IndicatorsPanelChartSize);
                    _icons[index].isVisible = true;
                    _labels[index].isVisible = true;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ModManager:SetChart -> Exception: " + e.Message);
            }
        }

        private void ResetAllCharts()
        {
            try
            {
                foreach (UIRadialChart chart in _charts.Values)
                {
                    chart.isVisible = true;
                }

                foreach (UILabel label in _labels.Values)
                {
                    label.isVisible = true;
                }

                foreach (UILabel tooltip in _numbers.Values)
                {
                    tooltip.isVisible = true;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ModManager:ResetAllCharts -> Exception: " + e.Message);
            }
        }

        private void RefreshData()
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

                    ResetAllCharts();

                    switch (building.Info.m_class.GetZone())
                    {
                        case ItemClass.Zone.ResidentialHigh:
                        case ItemClass.Zone.ResidentialLow:
                            ShowResidentialCharts();
                            break;
                        case ItemClass.Zone.Industrial:
                            ShowIndustrialCharts();
                            break;
                        case ItemClass.Zone.CommercialHigh:
                        case ItemClass.Zone.CommercialLow:
                            ShowCommercialCharts();
                            break;
                        case ItemClass.Zone.Office:
                            ShowOfficeCharts();
                            break;
                        default:

                            break;
                    }

                    _cachedBuildingID = buildingId;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ModManager:RefreshData -> Exception: " + e.Message);
            }
        }

        private void ShowResidentialCharts()
        {
            try
            {
                SetChart(0, ImmaterialResourceManager.Resource.HealthCare);
                SetChart(1, ImmaterialResourceManager.Resource.DeathCare);
                SetChart(2, ImmaterialResourceManager.Resource.FireDepartment);
                SetChart(3, ImmaterialResourceManager.Resource.PoliceDepartment);
                SetChart(4, ImmaterialResourceManager.Resource.EducationElementary);
                SetChart(5, ImmaterialResourceManager.Resource.EducationHighSchool);
                SetChart(6, ImmaterialResourceManager.Resource.EducationUniversity);
                SetChart(7, ImmaterialResourceManager.Resource.PublicTransport);
                SetChart(8, ImmaterialResourceManager.Resource.PostService);
                SetChart(9, ImmaterialResourceManager.Resource.Entertainment);
                SetChart(10, ImmaterialResourceManager.Resource.FirewatchCoverage);
                SetChart(11, ImmaterialResourceManager.Resource.DisasterCoverage);
                SetChart(12, ImmaterialResourceManager.Resource.RadioCoverage);
                SetChart(13, ImmaterialResourceManager.Resource.NoisePollution);
                SetChart(14, ImmaterialResourceManager.Resource.Abandonment);

                _charts[15].isVisible = false;
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ModManager:ShowResidentialCharts -> Exception: " + e.Message);
            }
        }

        private void ShowIndustrialCharts()
        {
            try
            {
                SetChart(0, ImmaterialResourceManager.Resource.HealthCare);
                SetChart(1, ImmaterialResourceManager.Resource.DeathCare);
                SetChart(2, ImmaterialResourceManager.Resource.FireDepartment);
                SetChart(3, ImmaterialResourceManager.Resource.PoliceDepartment);
                SetChart(4, ImmaterialResourceManager.Resource.EducationElementary);
                SetChart(5, ImmaterialResourceManager.Resource.EducationHighSchool);
                SetChart(6, ImmaterialResourceManager.Resource.EducationUniversity);
                SetChart(7, ImmaterialResourceManager.Resource.PublicTransport);
                SetChart(8, ImmaterialResourceManager.Resource.PostService);
                SetChart(9, ImmaterialResourceManager.Resource.Entertainment);
                SetChart(10, ImmaterialResourceManager.Resource.FirewatchCoverage);
                SetChart(11, ImmaterialResourceManager.Resource.DisasterCoverage);
                SetChart(12, ImmaterialResourceManager.Resource.RadioCoverage);
                SetChart(13, ImmaterialResourceManager.Resource.CargoTransport);
                SetChart(14, ImmaterialResourceManager.Resource.NoisePollution);
                SetChart(15, ImmaterialResourceManager.Resource.Abandonment);

            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ModManager:ShowIndustrialCharts -> Exception: " + e.Message);
            }
        }

        private void ShowCommercialCharts()
        {
            try
            {
                SetChart(0, ImmaterialResourceManager.Resource.HealthCare);
                SetChart(1, ImmaterialResourceManager.Resource.DeathCare);
                SetChart(2, ImmaterialResourceManager.Resource.FireDepartment);
                SetChart(3, ImmaterialResourceManager.Resource.PoliceDepartment);
                SetChart(4, ImmaterialResourceManager.Resource.EducationElementary);
                SetChart(5, ImmaterialResourceManager.Resource.EducationHighSchool);
                SetChart(6, ImmaterialResourceManager.Resource.EducationUniversity);
                SetChart(7, ImmaterialResourceManager.Resource.PublicTransport);
                SetChart(8, ImmaterialResourceManager.Resource.PostService);
                SetChart(9, ImmaterialResourceManager.Resource.Entertainment);
                SetChart(10, ImmaterialResourceManager.Resource.FirewatchCoverage);
                SetChart(11, ImmaterialResourceManager.Resource.DisasterCoverage);
                SetChart(12, ImmaterialResourceManager.Resource.RadioCoverage);
                SetChart(13, ImmaterialResourceManager.Resource.CargoTransport);
                SetChart(14, ImmaterialResourceManager.Resource.NoisePollution);
                SetChart(15, ImmaterialResourceManager.Resource.Abandonment);
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ModManager:ShowCommercialCharts -> Exception: " + e.Message);
            }
        }

        private void ShowOfficeCharts()
        {
            try
            {
                SetChart(0, ImmaterialResourceManager.Resource.HealthCare);
                SetChart(1, ImmaterialResourceManager.Resource.DeathCare);
                SetChart(2, ImmaterialResourceManager.Resource.FireDepartment);
                SetChart(3, ImmaterialResourceManager.Resource.PoliceDepartment);
                SetChart(4, ImmaterialResourceManager.Resource.EducationElementary);
                SetChart(5, ImmaterialResourceManager.Resource.EducationHighSchool);
                SetChart(6, ImmaterialResourceManager.Resource.EducationUniversity);
                SetChart(7, ImmaterialResourceManager.Resource.PublicTransport);
                SetChart(8, ImmaterialResourceManager.Resource.PostService);
                SetChart(9, ImmaterialResourceManager.Resource.Entertainment);
                SetChart(10, ImmaterialResourceManager.Resource.FirewatchCoverage);
                SetChart(11, ImmaterialResourceManager.Resource.DisasterCoverage);
                SetChart(12, ImmaterialResourceManager.Resource.RadioCoverage);
                SetChart(13, ImmaterialResourceManager.Resource.NoisePollution);
                SetChart(14, ImmaterialResourceManager.Resource.Abandonment);

                _charts[15].isVisible = false;
            }
            catch (Exception e)
            {
                Debug.Log("[Show It!] ModManager:ShowOfficeCharts -> Exception: " + e.Message);
            }
        }

        private bool IsIndicatorPositive(ImmaterialResourceManager.Resource resource)
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

        private string GetIndicatorName(ImmaterialResourceManager.Resource resource)
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
                case ImmaterialResourceManager.Resource.EducationLibrary:
                    return "Library";
                case ImmaterialResourceManager.Resource.ChildCare:
                    return "Child Care";
                case ImmaterialResourceManager.Resource.ElderCare:
                    return "Elder Care";
                case ImmaterialResourceManager.Resource.None:
                    return "None";
                default:
                    return "Unknown";
            }
        }

        private string GetIndicatorSprite(ImmaterialResourceManager.Resource resource)
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
                    return "InfoIconEducation";
                case ImmaterialResourceManager.Resource.EducationHighSchool:
                    return "InfoIconEducation";
                case ImmaterialResourceManager.Resource.EducationUniversity:
                    return "InfoIconEducation";
                case ImmaterialResourceManager.Resource.DeathCare:
                    return "NotificationIconDead";
                case ImmaterialResourceManager.Resource.PublicTransport:
                    return "InfoIconPublicTransport";
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
                case ImmaterialResourceManager.Resource.EducationLibrary:
                    return "InfoIconEducation";
                case ImmaterialResourceManager.Resource.ChildCare:
                    return "InfoIconPopulation";
                case ImmaterialResourceManager.Resource.ElderCare:
                    return "InfoIconAge";
                case ImmaterialResourceManager.Resource.None:
                    return "ToolbarIconHelp";
                default:
                    return "ToolbarIconHelp";
            }
        }

        private void GetIndicatorInfoModes(ImmaterialResourceManager.Resource resource, out InfoManager.InfoMode infoMode, out InfoManager.SubInfoMode subInfoMode)
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
                case ImmaterialResourceManager.Resource.EducationLibrary:
                    infoMode = InfoManager.InfoMode.Education;
                    subInfoMode = InfoManager.SubInfoMode.LibraryEducation;
                    break;
                case ImmaterialResourceManager.Resource.ChildCare:
                    infoMode = InfoManager.InfoMode.Health;
                    subInfoMode = InfoManager.SubInfoMode.ChildCare;
                    break;
                case ImmaterialResourceManager.Resource.ElderCare:
                    infoMode = InfoManager.InfoMode.Health;
                    subInfoMode = InfoManager.SubInfoMode.ElderCare;
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
