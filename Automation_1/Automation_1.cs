/*
****************************************************************************
*  Copyright (c),  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

Revision History:

DATE		VERSION		AUTHOR			COMMENTS

22/01/2024	1.0.0.1		XXX, Skyline	Initial version
****************************************************************************
*/

namespace Automation_1
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Net.Http.Headers;
	using System.Text;
	using Skyline.DataMiner.Analytics.GenericInterface.QueryBuilder;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents a DataMiner Automation script.
    /// </summary>
	public class Script
    {
        /// <summary>
        /// The script entry point.
        /// </summary>
        /// <param name="engine">Link with SLAutomation process.</param>
        public void Run(IEngine engine)
        {
            Configuration configuration = new Configuration();
            AutomationModel model = new AutomationModel(engine);
            AutomationWindow window = new AutomationWindow(engine);

            AutomationPresenter presenter =
                new AutomationPresenter(engine, configuration, model, window);

            presenter.Run();
        }
    }

    internal sealed class Configuration
    {
        public Element SelectedElement { get; set; }

        public int? ParameterId { get; set; }

        public string StringValue { get; set; } = string.Empty;

        public double DoubleValue { get; set; }

        public string ResultMessage { get; set; } = string.Empty;

        public ParameterMeasurementType? SelectedParameterType { get; set; }
    }

    internal sealed class AutomationModel
    {
        private readonly IEngine engine;

        public AutomationModel(IEngine engine)
        {
            this.engine = engine;
        }

        public Element[] GetElements()
        {
            return engine.FindElementsByName("*") ?? new Element[0];
        }

        public ParameterInfo[] GetParameters(Element element)
        {
            if (element == null)
            {
                return new ParameterInfo[0];
            }

            var protocol = element.Protocol;
            return protocol == null || protocol.Parameters == null
                ? new ParameterInfo[0]
                : protocol.Parameters;
        }

        public void SetStringValue(Element element, int parameterId, string value)
        {
            element.SetParameter(parameterId, value);
        }

        public void SetDoubleValue(Element element, int parameterId, double value)
        {
            element.SetParameter(parameterId, value);
        }
    }

    internal sealed class AutomationPresenter
    {
        private readonly IEngine engine;
        private readonly Configuration configuration;
        private readonly AutomationModel model;
        private readonly AutomationWindow window;
        private readonly InteractiveController controller;
        private readonly Dictionary<string, Element> elementsByLabel = new Dictionary<string, Element>();

        private readonly Dictionary<string, ParameterInfo> parametersByLabel = new Dictionary<string, ParameterInfo>();

        public AutomationPresenter(
            IEngine engine,
            Configuration configuration,
            AutomationModel model,
            AutomationWindow window)
        {
            this.engine = engine;
            this.configuration = configuration;
            this.model = model;
            this.window = window;
            controller = new InteractiveController(engine);

            PopulateElements();

            window.ContinueToParameterButton.Pressed += (sender, args) =>
            {
                string selectedLabel = window.ElementDropDown.Selected;
                Element selectedElement;

                if (String.IsNullOrEmpty(selectedLabel) || !elementsByLabel.TryGetValue(selectedLabel, out selectedElement))
                {
                    return;
                }

                configuration.SelectedElement = selectedElement;
                PopulateParameters(selectedElement);
                controller.ShowDialog(window.ParameterDialog);
            };

            window.BackToElementButton.Pressed += (sender, args) =>
                controller.ShowDialog(window.ElementDialog);

            window.ContinueToValueButton.Pressed += (sender, args) =>
            {
                string selectedLabel = window.ParameterDropDown.Selected;
                ParameterInfo selectedParameter;

                if (String.IsNullOrEmpty(selectedLabel) || !parametersByLabel.TryGetValue(selectedLabel, out selectedParameter))
                {
                    return;
                }

                configuration.ParameterId = selectedParameter.ID;
                configuration.SelectedParameterType = selectedParameter.ParameterType;

                window.ConfigureValueWidgets(selectedParameter.ParameterType == ParameterMeasurementType.String);

                window.StringValueTextBox.Text = configuration.StringValue;
                window.DoubleValueNumeric.Value = configuration.DoubleValue;
                controller.ShowDialog(window.ValueDialog);
            };

            window.BackToParameterButton.Pressed += (sender, args) =>
            {
                SaveEnteredValues();
                controller.ShowDialog(window.ParameterDialog);
            };

            window.SetStringValueButton.Pressed += (sender, args) =>
            {
                SaveEnteredValues();
                SetValue(() => model.SetStringValue(
                    configuration.SelectedElement,
                    configuration.ParameterId.Value,
                    configuration.StringValue));
            };

            window.SetDoubleValueButton.Pressed += (sender, args) =>
            {
                SaveEnteredValues();
                SetValue(() => model.SetDoubleValue(
                    configuration.SelectedElement,
                    configuration.ParameterId.Value,
                    configuration.DoubleValue));
            };

            window.ExitButton.Pressed += (sender, args) =>
                engine.ExitSuccess("The user exited the interactive automation.");
        }

        public void Run()
        {
            controller.Run(window.ElementDialog);
        }

        private void PopulateElements()
        {
            List<string> labels = new List<string>();
            var elements = model.GetElements();

            foreach (Element element in elements)
            {
                string label = String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} (DMA {1}, Element {2})",
                    element.ElementName,
                    element.DmaId,
                    element.ElementId);

                elementsByLabel.Add(label, element);
                labels.Add(label);
            }

            window.ElementDropDown.SetOptions(labels);
        }

        private void PopulateParameters(Element element)
        {
            parametersByLabel.Clear();
            List<string> labels = new List<string>();

            foreach (ParameterInfo parameter in model.GetParameters(element))
            {
                if (parameter == null)
                {
                    continue;
                }

                string name = String.IsNullOrWhiteSpace(parameter.Name)
                    ? parameter.Description
                    : parameter.Name;

                if (!String.IsNullOrWhiteSpace(parameter.Description) &&
                    !String.Equals(parameter.Name, parameter.Description, StringComparison.Ordinal))
                {
                    name += " - " + parameter.Description;
                }

                string label = String.Format(
                    CultureInfo.InvariantCulture,
                    "{0} (ID {1})",
                    name,
                    parameter.ID);

                parametersByLabel.Add(label, parameter);
                labels.Add(label);
            }

            window.ParameterDropDown.SetOptions(labels);
        }
        private void SaveEnteredValues()
        {
            configuration.StringValue = window.StringValueTextBox.Text;
            configuration.DoubleValue = window.DoubleValueNumeric.Value;
        }

        private void SetValue(Action setValue)
        {
            try
            {
                setValue();
                configuration.ResultMessage = "Success";
                ResetValueInputs();
            }
            catch (Exception exception)
            {
                configuration.ResultMessage = exception.ToString();
            }

            window.ResultTextBox.Text = configuration.ResultMessage;
            controller.ShowDialog(window.ValueDialog);
        }

        private void ResetValueInputs()
        {
            configuration.StringValue = String.Empty;
            configuration.DoubleValue = 0.0;

            window.StringValueTextBox.Text = String.Empty;
            window.DoubleValueNumeric.Value = 0.0;
        }
    }

    internal sealed class AutomationDialog : Dialog
    {
        public AutomationDialog(IEngine engine)
            : base(engine)
        {
        }
    }

    internal sealed class AutomationWindow
    {
        public AutomationDialog ElementDialog { get; private set; }

        public DropDown ElementDropDown { get; private set; }

        public Button ContinueToParameterButton { get; private set; }

        public AutomationDialog ParameterDialog { get; private set; }

        public DropDown ParameterDropDown { get; private set; }

        public Label StringValueLabel { get; private set; }

        public Label DoubleValueLabel { get; private set; }

        public Button BackToElementButton { get; private set; }

        public Button ContinueToValueButton { get; private set; }

        public AutomationDialog ValueDialog { get; private set; }

        public TextBox StringValueTextBox { get; private set; }

        public Button SetStringValueButton { get; private set; }

        public Numeric DoubleValueNumeric { get; private set; }

        public Button SetDoubleValueButton { get; private set; }

        public TextBox ResultTextBox { get; private set; }

        public Button BackToParameterButton { get; private set; }

        public Button ExitButton { get; private set; }

        public AutomationWindow(IEngine engine)
        {
            ElementDialog = new AutomationDialog(engine) { Title = "Select element" };
            ElementDropDown = new DropDown();
            ContinueToParameterButton = new Button("Continue...");

            AddWidget(ElementDialog, new Label("Select an element on which to set a parameter:"), 0, 0);
            AddWidget(ElementDialog, ElementDropDown, 1, 0);
            AddWidget(ElementDialog, ContinueToParameterButton, 2, 0);

            ParameterDialog = new AutomationDialog(engine) { Title = "Specify parameter" };
            ParameterDropDown = new DropDown();
            BackToElementButton = new Button("Back...");
            ContinueToValueButton = new Button("Continue...");

            ParameterDialog.SetColumnWidthStretch(0);
            ParameterDialog.SetColumnWidthStretch(1);

            AddWidget(ParameterDialog, new Label("Select a parameter:"), 0, 0, 1, 2, HorizontalAlignment.Stretch);
            AddWidget(ParameterDialog, ParameterDropDown, 1, 0, 1, 2, HorizontalAlignment.Stretch);
            AddWidget(ParameterDialog, BackToElementButton, 2, 0, 1, 1, HorizontalAlignment.Stretch);
            AddWidget(ParameterDialog, ContinueToValueButton, 2, 1, 1, 1, HorizontalAlignment.Stretch);

            ValueDialog = new AutomationDialog(engine) { Title = "Set parameter value" };
            StringValueLabel = new Label("String Value");
            StringValueTextBox = new TextBox();
            SetStringValueButton = new Button("Set String Value");

            DoubleValueLabel = new Label("Double Value");
            DoubleValueNumeric = new Numeric { Decimals = 2, StepSize = 0.01 };
            SetDoubleValueButton = new Button("Set Double Value");

            ResultTextBox = new TextBox { IsMultiline = true, Height = 60 };
            BackToParameterButton = new Button("Back...");
            ExitButton = new Button("Exit");

            AddWidget(ValueDialog, StringValueLabel, 0, 0);
            AddWidget(ValueDialog, StringValueTextBox, 0, 1);
            AddWidget(ValueDialog, SetStringValueButton, 0, 2);

            AddWidget(ValueDialog, DoubleValueLabel, 1, 0);
            AddWidget(ValueDialog, DoubleValueNumeric, 1, 1);
            AddWidget(ValueDialog, SetDoubleValueButton, 1, 2);

            AddWidget(ValueDialog, ResultTextBox, 2, 0, 1, 3);
            AddWidget(ValueDialog, BackToParameterButton, 3, 0);
            AddWidget(ValueDialog, ExitButton, 3, 2);
        }

        private static void AddWidget(
            Dialog dialog,
            Widget widget,
            int row,
            int column,
            int rowSpan = 1,
            int columnSpan = 1,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
        {
            dialog.AddWidget(
                widget,
                row,
                column,
                rowSpan,
                columnSpan,
                horizontalAlignment,
                VerticalAlignment.Center);
        }

        public void ConfigureValueWidgets(bool showStringValue)
        {
            StringValueLabel.IsVisible = showStringValue;
            StringValueTextBox.IsVisible = showStringValue;
            SetStringValueButton.IsVisible = showStringValue;

            DoubleValueLabel.IsVisible = !showStringValue;
            DoubleValueNumeric.IsVisible = !showStringValue;
            SetDoubleValueButton.IsVisible = !showStringValue;
        }
    }

}