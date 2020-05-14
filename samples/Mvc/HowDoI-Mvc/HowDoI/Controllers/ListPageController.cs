using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.Globalization;

namespace CSharp_HowDoISamples.Controllers
{
    public class ListPageController : Controller
    {
        private const int DefaultPageSize = 10;
        private const string allSamplesString = "AllSamples";
        private const string FeaturedMvcString = "FeaturedMvc";

        public ListPageController()
        {
        }

        public ActionResult ListPageView(string categoryId, int? page)
        {
            string filter = string.Empty;
            if (ControllerContext.HttpContext.Request.Form["filter"] != null)
            {
                filter = ControllerContext.HttpContext.Request.Form["filter"].ToString();
                categoryId = allSamplesString;
            }

            if (string.IsNullOrEmpty(categoryId))
            {
                categoryId = allSamplesString;
            }

            string file = Server.MapPath("~/SampleList.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

            ModelCategoriesSamples models = GetModel(categoryId, currentPageIndex, doc, filter);
            return View(models);
        }

        private static ModelCategoriesSamples GetModel(string categoryId, int currentPageIndex, XmlDocument doc, string filter)
        {
            XmlNodeList list = doc.SelectNodes("//Category");

            Collection<Category> categories = new Collection<Category>();
            Collection<Sample> samples = new Collection<Sample>();
            foreach (XmlNode item in list)
            {
                Category category = new Category();
                category.Id = item.Attributes["Id"].Value;
                category.Name = item.Attributes["Name"].Value;
                category.Description = item.Attributes["description"].Value;
                if (item.Attributes["Path"] != null)
                {
                    category.Path = item.Attributes["Path"].Value;
                }

                categories.Add(category);

                if (category.Id == FeaturedMvcString)
                {
                    if (categoryId == category.Id)
                    {
                        foreach (XmlNode sampleNode in item.ChildNodes)
                        {
                            string sampleId = sampleNode.Attributes["Id"].Value;
                            XmlNode realSampleNode = doc.SelectSingleNode("//Sample[@Id='" + sampleId + "'][@Name]");
                            SetSampleItem(samples, realSampleNode.ParentNode.Attributes["Id"].Value, realSampleNode, filter);
                        }
                    }
                }
                else if (categoryId == allSamplesString || categoryId == category.Id)
                {
                    foreach (XmlNode sampleNode in item.ChildNodes)
                    {
                        SetSampleItem(samples, category.Id, sampleNode, filter);
                    }
                }
            }

            int pageSize = DefaultPageSize;
            if (!string.IsNullOrWhiteSpace(filter))
            {
                pageSize = samples.Count;
            }
            IPagedList<Sample> sampleList = samples.ToPagedList(currentPageIndex, pageSize);
            ModelCategoriesSamples models = new ModelCategoriesSamples(categories, sampleList);
            models.SelectedCatrgory = categoryId;
            var selected = (categories.Where(c => c.Id == categoryId)).ElementAt(0);
            models.SelectedCatrgoryName = selected.Name;
            models.SelectedCategoryDescritpion = selected.Description;
            models.CurrentPage = currentPageIndex + 1;
            return models;
        }

        private static void SetSampleItem(Collection<Sample> samples, string categoryId, XmlNode sampleNode, string filter)
        {
            if (sampleNode.Attributes["Name"].Value.ToLowerInvariant().Contains(filter.ToLowerInvariant()))
            {
                Sample sample = new Sample();
                sample.Id = sampleNode.Attributes["Id"].Value;
                sample.Name = sampleNode.Attributes["Name"].Value;
                sample.VideoUrl = sampleNode.Attributes["VideoUrl"].Value;
                sample.Description = sampleNode.ChildNodes[0].InnerText;
                sample.ImageFileName = sample.Id + ".jpg";
                if (sampleNode.Attributes["ImageFileName"] != null)
                {
                    sample.ImageFileName = sampleNode.Attributes["ImageFileName"].Value;
                }
                sample.Category = categoryId;
                samples.Add(sample);
            }
        }
    }
}
