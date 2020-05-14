namespace ThinkGeo.MapSuite.USDemographicMap
{
    public static class MapSuiteExampleHelper
    {
        public static string GetAliasUnitValueString(string columnName, double value)
        {
            string result = GetAliasUnitString(columnName, value);
            string[] strResults = result.Split(new string[] { ": " }, System.StringSplitOptions.None);

            if (strResults[strResults.Length - 1].Contains("%"))
            {
                result = strResults[strResults.Length - 1];
            }
            else
            {
                result = value.ToString("N0");
            }

            return result;
        }

        public static string GetAliasUnitString(string columnName, double value)
        {
            string result = string.Empty;
            switch (columnName)
            {
                case "Population":
                    result = string.Format("Population : {0} People", value.ToString("N0"));
                    break;
                case "PopDensity":
                    result = string.Format("Population Density : {0} People / sq.mi.", value.ToString("N0"));
                    break;
                case "Female":
                    result = string.Format("Female : {0} %", value.ToString("N2"));
                    break;
                case "Male":
                    result = string.Format("Male : {0} %", value.ToString("N2"));
                    break;
                case "AREALAND":
                    result = string.Format("Land Area : {0} sq.mi.", (value * 0.386102159 * 10e-7).ToString("N2"));
                    break;
                case "AREAWATR":
                    result = string.Format("Water Area : {0} sq.mi.", (value * 0.386102159 * 10e-7).ToString("N2"));
                    break;
                case "White":
                    result = string.Format("White : {0} %", value.ToString("N2"));
                    break;
                case "Black":
                    result = string.Format("Black : {0} %", value.ToString("N2"));
                    break;
                case "Indian":
                    result = string.Format("American Indian : {0} %", value.ToString("N2"));
                    break;
                case "Islander":
                    result = string.Format("Native Pacific Islander : {0} %", value.ToString("N2"));
                    break;
                case "Asian":
                    result = string.Format("Asian : {0} %", value.ToString("N2"));
                    break;
                case "Other":
                    result = string.Format("Other : {0} %", value.ToString("N2"));
                    break;
                case "MultiRace":
                    result = string.Format("Multiracial : {0} %", value.ToString("N2"));
                    break;
                case "Under5":
                    result = string.Format("<= 5 : {0} %", value.ToString("N2"));
                    break;
                case "5to9":
                    result = string.Format("5 ~ 10 : {0} %", value.ToString("N2"));
                    break;
                case "10to14":
                    result = string.Format("10 ~ 15 : {0} %", value.ToString("N2"));
                    break;
                case "15to17":
                    result = string.Format("15 ~ 18 : {0} %", value.ToString("N2"));
                    break;
                case "18to24":
                    result = string.Format("18 ~ 25 : {0} %", value.ToString("N2"));
                    break;
                case "25to34":
                    result = string.Format("25 ~ 35 : {0} %", value.ToString("N2"));
                    break;
                case "35to44":
                    result = string.Format("35 ~ 45 : {0} %", value.ToString("N2"));
                    break;
                case "45to54":
                    result = string.Format("45 ~ 55 : {0} %", value.ToString("N2"));
                    break;
                case "55to64":
                    result = string.Format("55 ~ 65 : {0} %", value.ToString("N2"));
                    break;
                case "65to74":
                    result = string.Format("65 ~ 75 : {0} %", value.ToString("N2"));
                    break;
                case "75to84":
                    result = string.Format("75 ~ 85 : {0} %", value.ToString("N2"));
                    break;
                case "over85":
                    result = string.Format(">= 85 : {0} %", value.ToString("N2"));
                    break;
                default:
                    result = value.ToString();
                    break;
            }

            return result;
        }
    }
}
