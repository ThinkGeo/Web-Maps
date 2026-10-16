using System.Collections.Generic;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class MarkerPlaceModel
    {
        public string Id { get; set; }

        public PointShape Position { get; set; }

        public PointMarkerStyle Style { get; set; }

        public string Description { get; set; }


        public static List<MarkerPlaceModel> GetMarkerPlaces() 
        {
            var result = new List<MarkerPlaceModel>();

            result.Add(new MarkerPlaceModel
            {
                Id = "France",
                Position = new PointShape(259274.399943, 6247045.447691),
                Style = new PointMarkerStyle("images/country/france.png", 0.5, 1.0, 0.4),
                Description = ", in Western Europe, encompasses medieval cities, alpine villages and Mediterranean beaches. Paris, its capital, is famed for its fashion houses, classical art museums including the Louvre and monuments like the Eiffel Tower. The country is also renowned for its wines and sophisticated cuisine."
            });

            result.Add(new MarkerPlaceModel
            {
                Id = "Germany",
                Position = new PointShape(1163747.714729, 6683705.090302),
                Style = new PointMarkerStyle("images/country/germany.png", 0.5, 1.0, 0.4),
                Description = ", a Western European country with a landscape of forests, rivers, mountain ranges and North Sea beaches. It has over 2 millennia of history. Berlin, its capital, is home to art and nightlife scenes, the Brandenburg Gate and many sites relating to WWII."
            });

            result.Add(new MarkerPlaceModel
            {
                Id = "Ireland",
                Position = new PointShape(-697105.697961,7048717.000346),
                Style = new PointMarkerStyle("images/country/ireland.png", 0.5, 1.0, 0.4),
                Description = ", an island in the North Atlantic. It is separated from Great Britain to its east by the North Channel, the Irish Sea, and St George's Channel. Ireland is the second-largest island of the British Isles, the third-largest in Europe, and the twentieth-largest on Earth. "
            });

            result.Add(new MarkerPlaceModel
            {
                Id = "Italy",
                Position = new PointShape(1389319.426111, 5141460.270574),
                Style = new PointMarkerStyle("images/country/italy.png", 0.5, 1.0, 0.4),
                Description = ", a European country with a long Mediterranean coastline, has left a powerful mark on Western culture and cuisine. Its capital, Rome, is home to the Vatican as well as landmark art and ancient ruins. "
            });

            result.Add(new MarkerPlaceModel
            {
                Id = "Poland",
                Position = new PointShape(2130029.085362, 6821808.652036),
                Style = new PointMarkerStyle("images/country/poland.png", 0.5, 1.0, 0.4),
                Description = ", officially the Republic of Poland, is a country located in Central Europe. It is divided into 16 administrative subdivisions, covering an area of 312,696 square kilometres, and has a largely temperate seasonal climate."
            });

            result.Add(new MarkerPlaceModel
            {
                Id = "Portugal",
                Position = new PointShape(-1010191.765817, 4684061.093316),
                Style = new PointMarkerStyle("images/country/portugal.png", 0.5, 1.0, 0.4),
                Description = ", a southern European country on the Iberian Peninsula, bordering Spain. Its location on the Atlantic Ocean has influenced many aspects of its culture: salt cod and grilled sardines are national dishes, the Algarve's beaches are a major destination and much of the nation's architecture dates to the 1500s–1800s, when Portugal had a powerful maritime empire."
            });

            result.Add(new MarkerPlaceModel
            {
                Id = "Romania",
                Position = new PointShape(2905830.067289, 5535263.840299),
                Style = new PointMarkerStyle("images/country/romania.png", 0.5, 1.0, 0.4),
                Description = ", a southeastern European country known for the forested region of Transylvania, ringed by the Carpathian Mountains. Its preserved medieval towns include Sighişoara, and there are many fortified churches and castles, notably clifftop Bran Castle, long associated with the Dracula legend."
            });

            result.Add(new MarkerPlaceModel
            {
                Id = "Spain",
                Position = new PointShape(-410925.464061, 4928659.583828),
                Style = new PointMarkerStyle("images/country/spain.png", 0.5, 1.0, 0.4),
                Description = ", a country on Europe's Iberian Peninsula, includes 17 autonomous regions with diverse geography and cultures. Capital city Madrid is home to the Royal Palace and Prado museum, housing works by European masters. Segovia has a medieval castle (the Alcázar) and an intact Roman aqueduct. "
            });

            result.Add(new MarkerPlaceModel
            {
                Id = "UK",
                Position = new PointShape(-14064.413204,6711782.579665),
                Style = new PointMarkerStyle("images/country/uk.png", 0.5, 1.0, 0.4),
                Description = "(United Kingdom), made up of England, Scotland, Wales and Northern Ireland, is an island nation in northwestern Europe. England – birthplace of Shakespeare and The Beatles – is home to the capital, London, a globally influential centre of finance and culture. England is also site of Neolithic Stonehenge, Bath's Roman spa and centuries-old universities at Oxford and Cambridge."
            });

            return result;
        }
    }
}
