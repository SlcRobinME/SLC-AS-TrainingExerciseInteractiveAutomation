namespace Automation_1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class SelectElementModel
    {
        private readonly IEngine engine;

        public SelectElementModel(IEngine engine)
        {
            this.engine = engine;
        }

        public Element SelectedElement { get; set; }

        public IEnumerable<Element> GetAllElements()
        {
                var elements = engine.FindElementsByName("*");

                if (elements == null || elements.Length == 0)
                    return Enumerable.Empty<Element>();

                return elements.OrderBy(e => e.ElementName);
        }
    }

    public class SelectParameterModel
    {
        public Element SelectedElement { get; set; }

        public int ParametarId { get; set; }
    }

    public class SetValueModel
    {
        public Element SelectedElement { get; set; }

        public int ParameterId { get; set; }
    }
}
