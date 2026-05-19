namespace Automation_1
{
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Automation;

    /// <summary>Model for the "Select Element" step.</summary>
    public class SelectElementModel
    {
        private readonly IEngine engine;

        public SelectElementModel(IEngine engine)
        {
            this.engine = engine;
        }

        /// <summary>The element chosen by the user.</summary>
        public Element SelectedElement { get; set; }

        /// <summary>Returns all elements in the DMS, sorted by name.</summary>
        public IEnumerable<Element> GetAllElements()
        {
            var elements = engine.FindElementsByName("*");

            if (elements == null || elements.Length == 0)
                return Enumerable.Empty<Element>();

            return elements.OrderBy(e => e.ElementName);
        }
    }

    /// <summary>Model for the "Select Parameter" step.</summary>
    public class SelectParameterModel
    {
        /// <summary>The element carried forward from step 1.</summary>
        public Element SelectedElement { get; set; }

        /// <summary>The parameter ID chosen by the user.</summary>
        public int ParameterId { get; set; }
    }

    /// <summary>Model for the "Set Value" step.</summary>
    public class SetValueModel
    {
        /// <summary>The element on which the parameter will be set.</summary>
        public Element SelectedElement { get; set; }

        /// <summary>The parameter ID to set.</summary>
        public int ParameterId { get; set; }
    }
}