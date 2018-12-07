using ColossalFramework;

namespace ShowIt
{
    public static class ResidentialBuildingHelper
    {
        public static int CalculateResourceEffect(ushort buildingID, ref Building data, ImmaterialResourceManager.Resource resource)
        {
            int averageResidentRequirement;
            int value;

            switch (resource)
            {
                case ImmaterialResourceManager.Resource.HealthCare:
                case ImmaterialResourceManager.Resource.FireDepartment:
                case ImmaterialResourceManager.Resource.PoliceDepartment:
                case ImmaterialResourceManager.Resource.EducationElementary:
                case ImmaterialResourceManager.Resource.EducationHighSchool:
                case ImmaterialResourceManager.Resource.EducationUniversity:
                    averageResidentRequirement = GetAverageResidentRequirement(buildingID, ref data, resource);
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, averageResidentRequirement, 500, 20, 40);
                case ImmaterialResourceManager.Resource.DeathCare:
                    averageResidentRequirement = GetAverageResidentRequirement(buildingID, ref data, resource);
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, averageResidentRequirement, 500, 10, 20);
                case ImmaterialResourceManager.Resource.PublicTransport:
                    averageResidentRequirement = GetAverageResidentRequirement(buildingID, ref data, resource);
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, averageResidentRequirement, 500, 20, 40);
                case ImmaterialResourceManager.Resource.NoisePollution:
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, 10, 100, 0, 100);
                case ImmaterialResourceManager.Resource.CrimeRate:
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, 10, 100, 0, 100);
                case ImmaterialResourceManager.Resource.Health:
                case ImmaterialResourceManager.Resource.Wellbeing:
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, 60, 100, 0, 50);
                case ImmaterialResourceManager.Resource.Density:
                    return 0;
                case ImmaterialResourceManager.Resource.Entertainment:
                    averageResidentRequirement = GetAverageResidentRequirement(buildingID, ref data, resource);
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, averageResidentRequirement, 500, 30, 60);
                case ImmaterialResourceManager.Resource.LandValue:
                case ImmaterialResourceManager.Resource.Attractiveness:
                case ImmaterialResourceManager.Resource.Coverage:
                    return 0;
                case ImmaterialResourceManager.Resource.FireHazard:
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, 50, 100, 10, 50);
                case ImmaterialResourceManager.Resource.Abandonment:
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, 15, 50, 10, 20);
                case ImmaterialResourceManager.Resource.CargoTransport:
                    return 0;
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
                    averageResidentRequirement = GetAverageResidentRequirement(buildingID, ref data, resource);
                    Singleton<ImmaterialResourceManager>.instance.CheckLocalResource(resource, data.m_position, out value);
                    return ImmaterialResourceManager.CalculateResourceEffect(value, averageResidentRequirement, 200, 5, 10);
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
                    return 40;
                case ImmaterialResourceManager.Resource.DeathCare:
                    return 20;
                case ImmaterialResourceManager.Resource.PublicTransport:
                    return 40;
                case ImmaterialResourceManager.Resource.NoisePollution:
                    return 100;
                case ImmaterialResourceManager.Resource.CrimeRate:
                    return 100;
                case ImmaterialResourceManager.Resource.Health:
                case ImmaterialResourceManager.Resource.Wellbeing:
                    return 50;
                case ImmaterialResourceManager.Resource.Density:
                    return 0;
                case ImmaterialResourceManager.Resource.Entertainment:
                    return 60;
                case ImmaterialResourceManager.Resource.LandValue:
                case ImmaterialResourceManager.Resource.Attractiveness:
                case ImmaterialResourceManager.Resource.Coverage:
                    return 0;
                case ImmaterialResourceManager.Resource.FireHazard:
                    return 50;
                case ImmaterialResourceManager.Resource.Abandonment:
                    return 20;
                case ImmaterialResourceManager.Resource.CargoTransport:
                    return 0;
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
                    return 10;
                case ImmaterialResourceManager.Resource.None:
                    return 0;
                default:
                    return 0;
            }
        }

        // Copied from ResidentialBuildingAI
        private static int GetAverageResidentRequirement(ushort buildingID, ref Building data, ImmaterialResourceManager.Resource resource)
        {
            CitizenManager instance = Singleton<CitizenManager>.instance;
            uint num = data.m_citizenUnits;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            while (num != 0)
            {
                uint nextUnit = instance.m_units.m_buffer[num].m_nextUnit;
                if ((instance.m_units.m_buffer[num].m_flags & CitizenUnit.Flags.Home) != 0)
                {
                    int num5 = 0;
                    int num6 = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        uint citizen = instance.m_units.m_buffer[num].GetCitizen(i);
                        if (citizen != 0 && !instance.m_citizens.m_buffer[citizen].Dead)
                        {
                            num5 += GetResidentRequirement(resource, ref instance.m_citizens.m_buffer[citizen]);
                            num6++;
                        }
                    }
                    if (num6 == 0)
                    {
                        num3 += 100;
                        num4++;
                    }
                    else
                    {
                        num3 += num5;
                        num4 += num6;
                    }
                }
                num = nextUnit;
                if (++num2 > 524288)
                {
                    break;
                }
            }
            if (num4 != 0)
            {
                return (num3 + (num4 >> 1)) / num4;
            }
            return 0;
        }

        // Copied from ResidentialBuildingAI
        private static int GetResidentRequirement(ImmaterialResourceManager.Resource resource, ref Citizen citizen)
        {
            switch (resource)
            {
                case ImmaterialResourceManager.Resource.HealthCare:
                    return Citizen.GetHealthCareRequirement(Citizen.GetAgePhase(citizen.EducationLevel, citizen.Age));
                case ImmaterialResourceManager.Resource.DeathCare:
                    return Citizen.GetDeathCareRequirement(Citizen.GetAgePhase(citizen.EducationLevel, citizen.Age));
                case ImmaterialResourceManager.Resource.PoliceDepartment:
                    return Citizen.GetPoliceDepartmentRequirement(Citizen.GetAgePhase(citizen.EducationLevel, citizen.Age));
                case ImmaterialResourceManager.Resource.FireDepartment:
                    return Citizen.GetFireDepartmentRequirement(Citizen.GetAgePhase(citizen.EducationLevel, citizen.Age));
                case ImmaterialResourceManager.Resource.Entertainment:
                    return Citizen.GetEntertainmentRequirement(Citizen.GetAgePhase(citizen.EducationLevel, citizen.Age));
                case ImmaterialResourceManager.Resource.PublicTransport:
                    return Citizen.GetTransportRequirement(Citizen.GetAgePhase(citizen.EducationLevel, citizen.Age));
                case ImmaterialResourceManager.Resource.PostService:
                    return Citizen.GetMailAccumulation(citizen.EducationLevel);
                case ImmaterialResourceManager.Resource.EducationElementary:
                    {
                        Citizen.AgePhase agePhase3 = Citizen.GetAgePhase(citizen.EducationLevel, citizen.Age);
                        if (agePhase3 < Citizen.AgePhase.Teen0)
                        {
                            return Citizen.GetEducationRequirement(agePhase3);
                        }
                        return 0;
                    }
                case ImmaterialResourceManager.Resource.EducationHighSchool:
                    {
                        Citizen.AgePhase agePhase2 = Citizen.GetAgePhase(citizen.EducationLevel, citizen.Age);
                        if (agePhase2 >= Citizen.AgePhase.Teen0 && agePhase2 < Citizen.AgePhase.Young0)
                        {
                            return Citizen.GetEducationRequirement(agePhase2);
                        }
                        return 0;
                    }
                case ImmaterialResourceManager.Resource.EducationUniversity:
                    {
                        Citizen.AgePhase agePhase = Citizen.GetAgePhase(citizen.EducationLevel, citizen.Age);
                        if (agePhase >= Citizen.AgePhase.Young0)
                        {
                            return Citizen.GetEducationRequirement(agePhase);
                        }
                        return 0;
                    }
                default:
                    return 100;
            }
        }
    }
}
