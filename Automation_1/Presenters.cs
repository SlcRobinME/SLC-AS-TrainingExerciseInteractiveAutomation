namespace Automation_1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Automation;

    public class SelectElementPresenter
    {
        private readonly SelectElementView view;
        private readonly SelectElementModel model;

        public SelectElementPresenter(SelectElementView view, SelectElementModel model)
        {
            this.view = view;
            this.model = model;

            view.ButtonContinue.Pressed += OnContinuePressed;
        }

        public event EventHandler OnContinue;

        public void LoadView()
        {
            var elements = model.GetAllElementNames().ToList();

            if (!elements.Any())
            {
                elements = new List<string> { "No elements found" };
            }

            view.SetElementOptions(elements);
        }

        private void OnContinuePressed(object sender, EventArgs e)
        {
            model.SelectedElement = view.DropdownElements.Selected;
            OnContinue?.Invoke(this, EventArgs.Empty);
        }
    }

    public class SelectParameterPresenter
    {
        private readonly SelectParameterView view;
        private readonly SelectParameterModel model;

        public SelectParameterPresenter(SelectParameterView view, SelectParameterModel model)
        {
            this.view = view;
            this.model = model;

            view.ButtonContinue.Pressed += OnContinuePressed;
            view.ButtonBack.Pressed += OnBackPressed;
        }

        public event EventHandler OnContinue;

        public event EventHandler OnBack;

        public void LoadView()
        {
            view.NumericParameterId.Value = model.ParametarId;
        }

        private void OnContinuePressed(object sender, EventArgs e)
        {
            model.ParametarId = (int)view.NumericParameterId.Value;
            OnContinue?.Invoke(this, EventArgs.Empty);
        }

        private void OnBackPressed(object sender, EventArgs e)
        {
            OnBack?.Invoke(this, EventArgs.Empty);
        }
    }

    public class SetValuePresenter
    {
        private readonly SetValueView view;
        private readonly SetValueModel model;
        private readonly Engine engine;

        public SetValuePresenter(SetValueView view, SetValueModel model, Engine engine)
        {
            this.view = view;
            this.model = model;
            this.engine = engine;

            view.ButtonSetString.Pressed += OnSetStringPressed;
            view.ButtonSetDouble.Pressed += OnSetDoublePressed;
            view.ButtonBack.Pressed += OnBackPressed;
            view.ButtonExit.Pressed += OnExitPressed;
        }

        public event EventHandler OnBack;

        public event EventHandler OnExit;

        public void LoadView()
        {
            view.TextBoxString.Text = string.Empty;
            view.NumericDouble.Value = 0;
            view.ShowResult(string.Empty);
        }

        private void OnSetStringPressed(object sender, EventArgs e)
        {
            try
            {
                var element = engine.FindElement(model.SelectedElement);
                if (element == null)
                    throw new Exception($"Element '{model.SelectedElement}' not found.");

                element.SetParameter(model.ParameterId, view.TextBoxString.Text);
                view.ShowResult("Success");
            }
            catch (Exception ex)
            {
                view.ShowResult($"Exception: {ex.Message}");
            }
        }

        private void OnSetDoublePressed(object sender, EventArgs e)
        {
            try
            {
                var element = engine.FindElement(model.SelectedElement);
                if (element == null)
                    throw new Exception($"Element '{model.SelectedElement}' not found.");

                element.SetParameter(model.ParameterId, view.NumericDouble.Value);
                view.ShowResult("Success");
            }
            catch (Exception ex)
            {
                view.ShowResult($"Exception: {ex.Message}");
            }
        }

        private void OnBackPressed(object sender, EventArgs e)
        {
            OnBack?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitPressed(object sender, EventArgs e)
        {
            OnExit?.Invoke(this, EventArgs.Empty);
        }
    }
}
