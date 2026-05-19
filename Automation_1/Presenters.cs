namespace Automation_1
{
    using System;
    using System.Linq;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>Presenter for the "Select Element" dialog.</summary>
    public class SelectElementPresenter
    {
        private readonly SelectElementView view;
        private readonly SelectElementModel model;

        public SelectElementPresenter(SelectElementView view, SelectElementModel model)
        {
            this.view = view;
            this.model = model;

            view.ContinueButton.Pressed += OnContinuePressed;
        }

        /// <summary>Raised when the user clicks Continue.</summary>
        public event EventHandler ContinueRequested;

        /// <summary>Populates the element dropdown from the model.</summary>
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

    /// <summary>Presenter for the "Select Parameter" dialog.</summary>
    public class SelectParameterPresenter
    {
        private readonly SelectParameterView view;
        private readonly SelectParameterModel model;

        public SelectParameterPresenter(SelectParameterView view, SelectParameterModel model)
        {
            this.view = view;
            this.model = model;

            view.BackButton.Pressed += OnBackPressed;
            view.ContinueButton.Pressed += OnContinuePressed;
        }

        /// <summary>Raised when the user clicks Back.</summary>
        public event EventHandler BackRequested;

        /// <summary>Raised when the user clicks Continue.</summary>
        public event EventHandler ContinueRequested;

        /// <summary>Syncs the view with current model state.</summary>
        public void LoadView()
        {
            view.ParameterIdNumeric.Value = model.ParameterId;
        }

        private void OnBackPressed(object sender, EventArgs e)
        {
            BackRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnContinuePressed(object sender, EventArgs e)
        {
            model.ParameterId = (int)view.ParameterIdNumeric.Value;
            ContinueRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>Presenter for the "Set Value" dialog.</summary>
    public class SetValuePresenter
    {
        private readonly SetValueView view;
        private readonly SetValueModel model;

        public SetValuePresenter(SetValueView view, SetValueModel model)
        {
            this.view = view;
            this.model = model;

            view.SetStringButton.Pressed += OnSetStringPressed;
            view.SetDoubleButton.Pressed += OnSetDoublePressed;
            view.BackButton.Pressed += OnBackPressed;
            view.ExitButton.Pressed += OnExitPressed;
        }

        /// <summary>Raised when the user clicks Back.</summary>
        public event EventHandler BackRequested;

        /// <summary>Raised when the user clicks Exit.</summary>
        public event EventHandler ExitRequested;

        /// <summary>Resets the view to a clean state before showing.</summary>
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
                EnsureElement();
                model.SelectedElement.SetParameter(model.ParameterId, view.StringTextBox.Text);
                view.ShowResult("Success");
            }
            catch (Exception ex)
            {
                view.ShowResult($"Error: {ex.Message}");
            }
        }

        private void OnSetDoublePressed(object sender, EventArgs e)
        {
            try
            {
                EnsureElement();
                model.SelectedElement.SetParameter(model.ParameterId, view.DoubleNumeric.Value);
                view.ShowResult("Success");
            }
            catch (Exception ex)
            {
                view.ShowResult($"Error: {ex.Message}");
            }
        }

        private void OnBackPressed(object sender, EventArgs e)
        {
            BackRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitPressed(object sender, EventArgs e)
        {
            ExitRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Throws a descriptive exception when the element reference is missing.</summary>
        private void EnsureElement()
        {
            if (model.SelectedElement == null)
            {
                throw new InvalidOperationException(
                    "No element is selected. Please go back and choose an element.");
            }
        }
    }
}