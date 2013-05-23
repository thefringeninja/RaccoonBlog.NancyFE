using System;
using RaccoonBlog.NancyFE.Model;

namespace RaccoonBlog.NancyFE.ViewModels
{
    public class TagViewModel
    {
        public string Name { get; private set; }
        public int Weight { get; private set; }

        public string Slug
        {
            get { return Name.Slugify(); }
        }

        public TagViewModel(Tag model)
        {
            if (model.Name == null) throw new ArgumentException();
            Name = model.Name;
            Weight = model.Count * 10;
        }
    }
}