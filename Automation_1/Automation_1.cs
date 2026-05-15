/*
****************************************************************************
*  Copyright (c),  Skyline Communications NV  All Rights Reserved.        *
****************************************************************************

Revision History:

DATE		VERSION		AUTHOR			COMMENTS

15/05/2024	1.0.0.1		AMO, Skyline	Initial version
****************************************************************************
*/

namespace Automation_1
{
    using System;
    using System.Linq;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Core.DataMinerSystem.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents a DataMiner Automation script.
    /// </summary>
    public class Script
    {
        private InteractiveController app;
        private IEngine engine;

        /// <summary>
        /// The script entry point.
        /// </summary>
        /// <param name="engine">Link with SLAutomation process.</param>
        public void Run(IEngine engine)
        {
            try
            {
                engine.FindInteractiveClient("SetParameter Script", 100, "user:" + engine.UserLoginName);
                this.engine = engine;
                app = new InteractiveController(engine);

                engine.SetFlag(RunTimeFlags.NoKeyCaching);
                engine.Timeout = TimeSpan.FromHours(10);

                RunSafe(engine);
            }
            catch (ScriptAbortException)
            {
                throw;
            }
            catch (ScriptForceAbortException)
            {
                throw;
            }
            catch (ScriptTimeoutException)
            {
                throw;
            }
            catch (InteractiveUserDetachedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                engine.ExitFail($"Run|Something went wrong: {ex}");
            }
        }

        private void RunSafe(IEngine engine)
        {
            var dms = engine.GetDms();
            var allElements = dms.GetElements().Select(x => x.Name).ToList();

            // --- Dialog 1: Select Element ---
            var selectElementView = new SelectElementView(engine, allElements);
            var selectElementPresenter = new SelectElementPresenter(selectElementView);

            // --- Dialog 2: Select Parameter ---
            var selectParameterView = new SelectParameterView(engine);
            var selectParameterPresenter = new SelectParameterPresenter(selectParameterView);

            // --- Dialog 3: Set Value ---
            var setValueView = new SetValueView(engine);
            var setValuePresenter = new SetValuePresenter(setValueView, engine);

            // Wire navigation: Dialog 1 -> Dialog 2
            selectElementPresenter.ContinueRequested += (s, e) =>
            {
                selectParameterPresenter.ElementName = selectElementPresenter.SelectedElement;
                app.ShowDialog(selectParameterView);
            };

            // Wire navigation: Dialog 2 -> Dialog 1 (Back)
            selectParameterPresenter.BackRequested += (s, e) =>
            {
                app.ShowDialog(selectElementView);
            };

            // Wire navigation: Dialog 2 -> Dialog 3
            selectParameterPresenter.ContinueRequested += (s, e) =>
            {
                setValuePresenter.ElementName = selectElementPresenter.SelectedElement;
                setValuePresenter.ParameterId = selectParameterPresenter.SelectedParameterId;
                setValueView.FeedbackTextBox.Text = string.Empty;
                app.ShowDialog(setValueView);
            };

            // Wire navigation: Dialog 3 -> Dialog 2 (Back)
            setValuePresenter.BackRequested += (s, e) =>
            {
                app.ShowDialog(selectParameterView);
            };

            // Wire Exit
            setValuePresenter.ExitRequested += (s, e) =>
            {
                engine.ExitSuccess("Script finished by user.");
            };

            app.ShowDialog(selectElementView);
        }
    }

    /// <summary>View for the "Select Element" dialog.</summary>
    public class SelectElementView : Dialog
    {
        public SelectElementView(IEngine engine, System.Collections.Generic.IEnumerable<string> elementNames) : base(engine)
        {
            Title = "Step 1 – Select Element";

            var instructionLabel = new Label("Please select an element on which you would like to set a parameter:");
            var elementLabel = new Label("Element");
            ElementDropDown = new DropDown(elementNames) { IsDisplayFilterShown = true, IsSorted = true };
            ContinueButton = new Button("Continue...");

            AddWidget(instructionLabel, 0, 0, 1, 2);
            AddWidget(elementLabel, 1, 0);
            AddWidget(ElementDropDown, 1, 1);
            AddWidget(ContinueButton, 2, 0, 1, 2);
        }

        public DropDown ElementDropDown { get; }

        public Button ContinueButton { get; }
    }

    /// <summary>Presenter for the "Select Element" dialog.</summary>
    public class SelectElementPresenter
    {
        private readonly SelectElementView view;

        public SelectElementPresenter(SelectElementView view)
        {
            this.view = view;

            view.ContinueButton.Pressed += OnContinuePressed;
        }

        public event EventHandler ContinueRequested;

        public string SelectedElement => view.ElementDropDown.Selected;

        private void OnContinuePressed(object sender, EventArgs e)
        {
            ContinueRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>View for the "Select Parameter" dialog.</summary>
    public class SelectParameterView : Dialog
    {
        public SelectParameterView(IEngine engine) : base(engine)
        {
            Title = "Step 2 – Specify Parameter ID";

            var instructionLabel = new Label("Please specify the ID of the parameter you would like to set:");
            var paramLabel = new Label("Parameter ID");
            ParameterIdNumeric = new Numeric { Decimals = 0, StepSize = 1, Minimum = 0, Maximum = int.MaxValue, Value = 0 };
            BackButton = new Button("Back...");
            ContinueButton = new Button("Continue...");

            AddWidget(instructionLabel, 0, 0, 1, 2);
            AddWidget(paramLabel, 1, 0);
            AddWidget(ParameterIdNumeric, 1, 1);
            AddWidget(BackButton, 2, 0);
            AddWidget(ContinueButton, 2, 1);
        }

        public Numeric ParameterIdNumeric { get; }

        public Button BackButton { get; }

        public Button ContinueButton { get; }
    }

    /// <summary>Presenter for the "Select Parameter" dialog.</summary>
    public class SelectParameterPresenter
    {
        private readonly SelectParameterView view;

        public SelectParameterPresenter(SelectParameterView view)
        {
            this.view = view;

            view.BackButton.Pressed += OnBackPressed;
            view.ContinueButton.Pressed += OnContinuePressed;
        }

        public event EventHandler BackRequested;

        public event EventHandler ContinueRequested;

        public string ElementName { get; set; }

        public int SelectedParameterId => (int)view.ParameterIdNumeric.Value;

        private void OnBackPressed(object sender, EventArgs e)
        {
            BackRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnContinuePressed(object sender, EventArgs e)
        {
            ContinueRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>View for the "Set Value" dialog.</summary>
    public class SetValueView : Dialog
    {
        public SetValueView(IEngine engine) : base(engine)
        {
            Title = "Step 3 – Set Parameter Value";

            // Row 0: String value
            var stringLabel = new Label("String Value");
            StringTextBox = new TextBox { PlaceHolder = "Enter string value..." };
            SetStringButton = new Button("Set String Value");

            // Row 1: Double value
            var doubleLabel = new Label("Double Value");
            DoubleNumeric = new Numeric { Decimals = 2, StepSize = 0.01, Minimum = double.MinValue, Maximum = double.MaxValue, Value = 0 };
            SetDoubleButton = new Button("Set Double Value");

            // Row 2: Feedback (spans 3 columns)
            FeedbackTextBox = new TextBox { IsMultiline = true, Height = 80, PlaceHolder = "Result will appear here..." };

            // Row 3: Navigation
            BackButton = new Button("Back...");
            ExitButton = new Button("Exit");

            AddWidget(stringLabel, 0, 0);
            AddWidget(StringTextBox, 0, 1);
            AddWidget(SetStringButton, 0, 2);

            AddWidget(doubleLabel, 1, 0);
            AddWidget(DoubleNumeric, 1, 1);
            AddWidget(SetDoubleButton, 1, 2);

            AddWidget(FeedbackTextBox, 2, 0, 1, 3);

            AddWidget(BackButton, 3, 0);
            AddWidget(ExitButton, 3, 2);
        }

        public TextBox StringTextBox { get; }

        public Button SetStringButton { get; }

        public Numeric DoubleNumeric { get; }

        public Button SetDoubleButton { get; }

        public TextBox FeedbackTextBox { get; }

        public Button BackButton { get; }

        public Button ExitButton { get; }
    }

    /// <summary>Presenter for the "Set Value" dialog.</summary>
    public class SetValuePresenter
    {
        private readonly SetValueView view;
        private readonly IEngine engine;

        public SetValuePresenter(SetValueView view, IEngine engine)
        {
            this.view = view;
            this.engine = engine;

            view.SetStringButton.Pressed += OnSetStringPressed;
            view.SetDoubleButton.Pressed += OnSetDoublePressed;
            view.BackButton.Pressed += OnBackPressed;
            view.ExitButton.Pressed += OnExitPressed;
        }

        public event EventHandler BackRequested;

        public event EventHandler ExitRequested;

        public string ElementName { get; set; }

        public int ParameterId { get; set; }

        private void OnSetStringPressed(object sender, EventArgs e)
        {
            try
            {
                var element = engine.FindElement(ElementName);
                element.SetParameter(ParameterId, view.StringTextBox.Text);
                view.FeedbackTextBox.Text = "Success";
            }
            catch (Exception ex)
            {
                view.FeedbackTextBox.Text = $"Error: {ex.Message}";
            }
        }

        private void OnSetDoublePressed(object sender, EventArgs e)
        {
            try
            {
                var element = engine.FindElement(ElementName);
                element.SetParameter(ParameterId, view.DoubleNumeric.Value);
                view.FeedbackTextBox.Text = "Success";
            }
            catch (Exception ex)
            {
                view.FeedbackTextBox.Text = $"Error: {ex.Message}";
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
    }
}