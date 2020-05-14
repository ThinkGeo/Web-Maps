using System;
using System.Collections.ObjectModel;

namespace ThinkGeo.MapSuite.USDemographicMap
{
    public class Category
    {
        private Collection<CategoryItem> items;
        private string title;
        private string categoryImage;

        public Category()
        { }

        public Category(string title, string categoryImage)
        {
            this.Title = title;
            this.CategoryImage = categoryImage;
        }

        public Collection<CategoryItem> Items
        {
            get
            {
                if (items == null)
                {
                    items = new Collection<CategoryItem>();
                }
                return items;
            }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string CategoryImage
        {
            get { return categoryImage; }
            set { categoryImage = value; }
        }

        public CategoryItem GetCategoryItemByAlias(string alias)
        {
            CategoryItem item = null;
            foreach (var categoryItem in this.Items)
            {
                if (categoryItem.Alias.Equals(alias.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    item = categoryItem;
                    break;
                }
            }

            return item;
        }
    }
}