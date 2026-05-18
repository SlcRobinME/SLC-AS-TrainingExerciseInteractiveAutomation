namespace Automation_1
{
    using System.Collections.Generic;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    public class SelectElementView : Dialog
    {
        public SelectElementView(Engine engine) : base(engine)
        {
            Title = "Select Element";

            LabelSelect = new Label("Please select an element on which you would like to set a parameter:");
            DropdownElements = new DropDown();
            ButtonContinue = new Button("Continue");

            AddWidget(LabelSelect, 0, 0);
            AddWidget(DropdownElements, 1, 0);
            AddWidget(ButtonContinue, 2, 0);
        }

        public Label LabelSelect { get; }

        public DropDown DropdownElements { get; }

        public Button ButtonContinue { get; }

        public void SetElementOptions(IEnumerable<string> elements)
        {
            DropdownElements.SetOptions(elements);
        }
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

            ButtonBack = new Button("Back");
            ButtonContinue = new Button("Continue");

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
            TextBoxString = new TextBox { PlaceHolder = "Enter string value"};
            ButtonSetString = new Button("Set String Value");

            LabelDouble = new Label("Double Value");
            NumericDouble = new Numeric { Decimals = 2, StepSize = 0.01 };
            ButtonSetDouble = new Button("Set Double Value");

            TextBoxResult = new TextBox
            {
                IsMultiline = true,
                IsEnabled = false,
                Height = 80,
            };

            ButtonBack = new Button("Back");
            ButtonExit = new Button("Exit");

            AddWidget(LabelString, 0, 0);
            AddWidget(TextBoxString, 0, 1);
            AddWidget(ButtonSetString, 0, 2);

            AddWidget(LabelDouble, 1, 0);
            AddWidget(NumericDouble, 1, 1);
            AddWidget(ButtonSetDouble, 1, 2);

            AddWidget(TextBoxResult, 2, 0, 1, 3);

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
