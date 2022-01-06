using ColossalFramework;

namespace ShowIt
{
    public static class CommercialBuildingHelper
    {
        public static int CalculateResourceEffect(ushort buildingID, ref Building data, ImmaterialResourceManager.Resource resource)
        {
            int value;

            switch (resource)
            {
                case ImmaterialResourceManager.Resource.HealthCare:
                case ImmaterialResourceManager.Resource.FireDepartment:
                case ImmaterialResourceManager.Resource.PoliceDepartment:
                case ImmaterialResourceManager.Resource.EducationElementary:
                case ImmaterialResourceManager.Resource.EducationHighSchool:
                case ImmaterialResourceManager.Resource.EducationUniversity:
                case ImmaterialResourceManager.Resource.DeathCare:
                case ImmaterialResourceManager.Resource.PublicTransport:
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, 100, 500, 50, 100);
                case ImmaterialResourceManager.Resource.NoisePollution:
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, 10, 100, 0, 100);
                case ImmaterialResourceManager.Resource.CrimeRate:
                case ImmaterialResourceManager.Resource.Health:
                case ImmaterialResourceManager.Resource.Wellbeing:                    
                case ImmaterialResourceManager.Resource.Density:
                    return 0;
                case ImmaterialResourceManager.Resource.Entertainment:
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, 100, 500, 50, 100);
                case ImmaterialResourceManager.Resource.LandValue:
                case ImmaterialResourceManager.Resource.Attractiveness:
                case ImmaterialResourceManager.Resource.Coverage:                    
                case ImmaterialResourceManager.Resource.FireHazard:
                    return 0;
                case ImmaterialResourceManager.Resource.Abandonment:
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, 15, 50, 10, 20);
                case ImmaterialResourceManager.Resource.CargoTransport:
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, 100, 500, 50, 100);
                case ImmaterialResourceManager.Resource.RadioCoverage:
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, 50, 100, 20, 25);
                case ImmaterialResourceManager.Resource.FirewatchCoverage:
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, 100, 1000, 0, 25);
                case ImmaterialResourceManager.Resource.EarthquakeCoverage:
                    return 0;
                case ImmaterialResourceManager.Resource.DisasterCoverage:
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, 50, 100, 20, 25);
                case ImmaterialResourceManager.Resource.TourCoverage:
                    return 0;
                case ImmaterialResourceManager.Resource.PostService:
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, 100, 500, 50, 100);
                case ImmaterialResourceManager.Resource.EducationLibrary:
                case ImmaterialResourceManager.Resource.ChildCare:
                case ImmaterialResourceManager.Resource.ElderCare:
                case ImmaterialResourceManager.Resource.None:
                    return 0;
                default:
                    return 0;
            }
        }

        public static int GetMaxEffect(ImmaterialResourceManager.Resource resource)
        {
            switch (resource)
            {
                case ImmaterialResourceManager.Resource.HealthCare:
                case ImmaterialResourceManager.Resource.FireDepartment:
                case ImmaterialResourceManager.Resource.PoliceDepartment:
                case ImmaterialResourceManager.Resource.EducationElementary:
                case ImmaterialResourceManager.Resource.EducationHighSchool:
                case ImmaterialResourceManager.Resource.EducationUniversity:
                case ImmaterialResourceManager.Resource.DeathCare:
                case ImmaterialResourceManager.Resource.PublicTransport:
                    return 100;
                case ImmaterialResourceManager.Resource.NoisePollution:
                    return 100;
                case ImmaterialResourceManager.Resource.CrimeRate:
                case ImmaterialResourceManager.Resource.Health:
                case ImmaterialResourceManager.Resource.Wellbeing:
                case ImmaterialResourceManager.Resource.Density:
                    return 0;
                case ImmaterialResourceManager.Resource.Entertainment:
                    return 100;
                case ImmaterialResourceManager.Resource.LandValue:
                case ImmaterialResourceManager.Resource.Attractiveness:
                case ImmaterialResourceManager.Resource.Coverage:
                case ImmaterialResourceManager.Resource.FireHazard:
                    return 50;
                case ImmaterialResourceManager.Resource.Abandonment:
                    return 20;
                case ImmaterialResourceManager.Resource.CargoTransport:
                    return 100;
                case ImmaterialResourceManager.Resource.RadioCoverage:
                    return 25;
                case ImmaterialResourceManager.Resource.FirewatchCoverage:
                    return 25;
                case ImmaterialResourceManager.Resource.EarthquakeCoverage:
                    return 0;
                case ImmaterialResourceManager.Resource.DisasterCoverage:
                    return 25;
                case ImmaterialResourceManager.Resource.TourCoverage:
                    return 0;
                case ImmaterialResourceManager.Resource.PostService:
                    return 100;
                case ImmaterialResourceManager.Resource.EducationLibrary:
                case ImmaterialResourceManager.Resource.ChildCare:
                case ImmaterialResourceManager.Resource.ElderCare:
                case ImmaterialResourceManager.Resource.None:
                    return 0;
                default:
                    return 0;
            }
        }
    }
}
