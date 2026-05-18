namespace Automation_1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Automation;

    public class SelectElementModel
    {
        private readonly IEngine engine;

        public SelectElementModel(IEngine engine)
        {
            this.engine = engine;
        }

        public string SelectedElement { get; set; }

        public IEnumerable<string> GetAllElementNames()
        {
            try
            {
                var elements = engine.FindElementsByName("*");
                if (elements != null && elements.Length > 0)
                    return elements.Select(e => e.ElementName).OrderBy(n => n);

                var elementsByView = engine.FindElementsInView(0);
                if (elementsByView != null && elementsByView.Length > 0)
                    return elementsByView.Select(e => e.ElementName).OrderBy(n => n);

                return new List<string> { "No elements found" };
            }
            catch (Exception ex)
            {
                return new List<string> { $"Error loading elements {ex.Message}" };
            }
        }
    }

    public class SelectParameterModel
    {
        public string SelectedElement { get; set; }

        public int ParametarId { get; set; }
    }

    public class SetValueModel
    {
        public string SelectedElement { get; set; }

        public int ParameterId { get; set; }
    }
}
