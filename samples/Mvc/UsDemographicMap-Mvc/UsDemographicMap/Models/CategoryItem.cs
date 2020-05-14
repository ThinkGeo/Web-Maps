namespace ThinkGeo.MapSuite.USDemographicMap
{
    public class CategoryItem
    {
        private string columnName;
        private string alias;

        public CategoryItem()
        { }

        public CategoryItem(string columnName, string alias)
        {
            this.columnName = columnName;
            this.alias = alias;
        }

        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

        public string Alias
        {
            get { return alias; }
            set { alias = value; }
        }
    }
}