using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSharp_HowDoISamples
{
    public class Category
    {
        public Category()
            : this(string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }

        public Category(string id, string name, string path, string description)
        {
            Id = id;
            Name = name;
            Path = path;
            Description = description;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
    }
}