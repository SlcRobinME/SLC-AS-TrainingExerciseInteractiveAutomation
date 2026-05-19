namespace Automation_1
{
    using System.Collections.Generic;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public static class UIConstants
    {
        public const int ButtonWidth = 120;
        public const int ViewWidth = 550;
        public const int ViewHeight = 200;
        public const int TextBoxWidth = 273;
        public const int TextBoxResultHeight = 80;
    }

    public class SelectElementView : Dialog
    {
        public SelectElementView(Engine engine) : base(engine)
        {
            Title = "Select Element";

            LabelSelect = new Label("Please select an element on which you would like to set a parameter:");
            DropdownElements = new DropDown<Element>();
            ButtonContinue = new Button("Continue") { Width = UIConstants.ButtonWidth };

            AddWidget(LabelSelect, 0, 0);
            AddWidget(DropdownElements, 1, 0);
            AddWidget(ButtonContinue, 2, 0);
            Width = UIConstants.ViewWidth;
            Height = UIConstants.ViewHeight;
        }

        public Label LabelSelect { get; }

        public DropDown<Element> DropdownElements { get; }

        public Button ButtonContinue { get; }

    }

    public class SelectParameterView : Dialog
    {
        public SelectParameterView(Engine engine) : base(engine)
        {
            Title = "Select Parameter";

            LabelInstruction = new Label("Please specify the Parameter ID you would like to set:");
            NumericParameterId = new Numeric
            {
                Decimals = 0,
                StepSize = 1,
                Minimum = 0,
                Maximum = int.MaxValue,
                Value = 0,
            };

            ButtonBack = new Button("Back") { Width = UIConstants.ButtonWidth };
            ButtonContinue = new Button("Continue") { Width = UIConstants.ButtonWidth };

            AddWidget(LabelInstruction, 0, 0, 1, 2);
            AddWidget(NumericParameterId, 1, 0, 1, 2);
            AddWidget(ButtonBack, 2, 0);
            AddWidget(ButtonContinue, 2, 1);
        }

        public Label LabelInstruction { get; }

        public Numeric NumericParameterId { get; }

        public Button ButtonBack { get; }

        public Button ButtonContinue { get; }
    }

    public class SetValueView : Dialog
    {
        public SetValueView(Engine engine) : base(engine)
        {
            Title = "Select Parameter Value";

            LabelString = new Label("String Value");
            TextBoxString = new TextBox
            {
                PlaceHolder = "Enter string value",
                Width = UIConstants.TextBoxWidth,
            };
            ButtonSetString = new Button("Set String Value") { Width = UIConstants.ButtonWidth };

            LabelDouble = new Label("Double Value");
            NumericDouble = new Numeric { Decimals = 2, StepSize = 0.01 };
            ButtonSetDouble = new Button("Set Double Value") { Width = UIConstants.ButtonWidth };

            TextBoxResult = new TextBox
            {
                IsMultiline = true,
                IsEnabled = false,
                Height = UIConstants.TextBoxResultHeight,
            };

            ButtonBack = new Button("Back") { Width = UIConstants.ButtonWidth };
            ButtonExit = new Button("Exit") { Width = UIConstants.ButtonWidth };

            AddWidget(LabelString, 0, 0);
            AddWidget(TextBoxString, 0, 1);
            AddWidget(ButtonSetString, 0, 2);

            AddWidget(LabelDouble, 1, 0);
            AddWidget(NumericDouble, 1, 1);
            AddWidget(ButtonSetDouble, 1, 2);

            AddWidget(TextBoxResult, 2, 0, 1, 2);

            AddWidget(ButtonBack, 3, 0);
            AddWidget(ButtonExit, 3, 2);
        }

        public Label LabelString { get; }

        public TextBox TextBoxString { get; }

        public Button ButtonSetString { get; }

        public Label LabelDouble { get; }

        public Numeric NumericDouble { get; }

        public Button ButtonSetDouble { get; }

        public TextBox TextBoxResult { get; }

        public Button ButtonBack { get; }

        public Button ButtonExit { get; }

        public void ShowResult(string message)
        {
            TextBoxResult.Text = message;
        }
    }
}
