//---------------------------------
// Presenters.cs  (izmijenjeno)
//---------------------------------
namespace Automation_1
{
    using System;
    using System.Linq;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class SelectElementPresenter
    {
        private readonly ISelectElementView view;
        private readonly SelectElementModel model;

        public SelectElementPresenter(ISelectElementView view, SelectElementModel model)
        {
            this.view = view;
            this.model = model;

            view.ContinueButton.Pressed += OnContinuePressed;
        }

        public event EventHandler ContinueRequested;

        public void LoadView()
        {
            var options = model.GetAllElements()
                .Select(e => new Option<Element>(e.ElementName, e))
                .ToList();

            view.ElementDropDown.SetOptions(options);
        }

        private void OnContinuePressed(object sender, EventArgs e)
        {
            model.SelectedElement = view.ElementDropDown.Selected;
            ContinueRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    public class SelectParameterPresenter
    {
        private readonly ISelectParameterView view;
        private readonly SelectParameterModel model;

        public SelectParameterPresenter(ISelectParameterView view, SelectParameterModel model)
        {
            this.view = view;
            this.model = model;

            view.BackButton.Pressed += OnBackPressed;
            view.ContinueButton.Pressed += OnContinuePressed;
        }

        public event EventHandler BackRequested;

        public event EventHandler ContinueRequested;

        public void LoadView()
        {
            view.ParameterIdNumeric.Value = model.ParameterId;
        }

        private void OnBackPressed(object sender, EventArgs e) =>
            BackRequested?.Invoke(this, EventArgs.Empty);

        private void OnContinuePressed(object sender, EventArgs e)
        {
            model.ParameterId = (int)view.ParameterIdNumeric.Value;
            ContinueRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    public class SetValuePresenter
    {
        private readonly ISetValueView view;
        private readonly SetValueModel model;
        private readonly IEngine engine;

        public SetValuePresenter(ISetValueView view, SetValueModel model, IEngine engine)
        {
            this.view = view;
            this.model = model;
            this.engine = engine;

            view.SetStringButton.Pressed += OnSetStringPressed;
            view.SetDoubleButton.Pressed += OnSetDoublePressed;
            view.BackButton.Pressed += OnBackPressed;
            view.ExitButton.Pressed += OnExitPressed;
        }

        public event EventHandler BackRequested;

        public event EventHandler ExitRequested;

        public void LoadView()
        {
            view.StringTextBox.Text = string.Empty;
            view.DoubleNumeric.Value = 0;
            view.ShowResult(string.Empty);
        }

        private void OnSetStringPressed(object sender, EventArgs e)
        {
            try
            {
                FindElement().SetParameter(model.ParameterId, view.StringTextBox.Text);
                view.ShowResult("Success");
            }
            catch (Exception ex) { view.ShowResult($"Error: {ex.Message}"); }
        }

        private void OnSetDoublePressed(object sender, EventArgs e)
        {
            try
            {
                FindElement().SetParameter(model.ParameterId, view.DoubleNumeric.Value);
                view.ShowResult("Success");
            }
            catch (Exception ex) { view.ShowResult($"Error: {ex.Message}"); }
        }

        private void OnBackPressed(object sender, EventArgs e) =>
            BackRequested?.Invoke(this, EventArgs.Empty);

        private void OnExitPressed(object sender, EventArgs e) =>
            ExitRequested?.Invoke(this, EventArgs.Empty);

        private Element FindElement()
        {
            if (model.SelectedElement == null)
            {
                throw new InvalidOperationException(
                    "No element is selected. Please go back and choose an element.");
            }

            var element = engine.FindElement(model.SelectedElement.ElementName);

            if (element == null)
            {
                throw new InvalidOperationException(
                    $"Element '{model.SelectedElement.ElementName}' could not be found.");
            }

            return element;
        }
    }
}