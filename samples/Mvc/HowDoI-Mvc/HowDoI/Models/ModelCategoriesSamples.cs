using System.Collections.ObjectModel;

namespace CSharp_HowDoISamples
{
    public class ModelCategoriesSamples
    {
        public ModelCategoriesSamples(Collection<Category> categories, IPagedList<Sample> samples)
        {
            this.Categories = categories;
            this.Samples = samples;
        }

        public Collection<Category> Categories { get; private set; }
        public IPagedList<Sample> Samples { get; private set; }
        public string SelectedCatrgory { get; set; }
        public string SelectedCatrgoryName { get; set; }
        public string SelectedCategoryDescritpion { get; set; }
        public int CurrentPage { get; set; }
    }
}