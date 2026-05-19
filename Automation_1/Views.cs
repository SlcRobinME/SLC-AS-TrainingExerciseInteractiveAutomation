namespace Automation_1
{
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>Shared UI dimension constants.</summary>
    public static class UIConstants
    {
        public const int ButtonWidth = 120;
        public const int ViewWidth = 550;
        public const int ViewHeight = 200;
        public const int TextBoxWidth = 273;
        public const int ResultBoxHeight = 80;
    }

    /// <summary>View for the "Select Element" dialog.</summary>
    public class SelectElementView : Dialog
    {
        public SelectElementView(IEngine engine) : base(engine)
        {
            Title = "Step 1 – Select Element";

            InstructionLabel = new Label("Please select an element on which you would like to set a parameter:");
            ElementDropDown = new DropDown<Element> { IsDisplayFilterShown = true };
            ContinueButton = new Button("Continue") { Width = UIConstants.ButtonWidth };

            AddWidget(InstructionLabel, 0, 0, 1, 2);
            AddWidget(ElementDropDown, 1, 0, 1, 2);
            AddWidget(ContinueButton, 2, 0, 1, 2);

            Width = UIConstants.ViewWidth;
            Height = UIConstants.ViewHeight;
        }

        public Label InstructionLabel { get; }

        public DropDown<Element> ElementDropDown { get; }

        public Button ContinueButton { get; }
    }

    /// <summary>View for the "Select Parameter" dialog.</summary>
    public class SelectParameterView : Dialog
    {
        public SelectParameterView(IEngine engine) : base(engine)
        {
            Title = "Step 2 – Specify Parameter ID";

            InstructionLabel = new Label("Please specify the ID of the parameter you would like to set:");
            ParameterIdNumeric = new Numeric
            {
                Decimals = 0,
                StepSize = 1,
                Minimum = 0,
                Maximum = int.MaxValue,
                Value = 0,
            };
            BackButton = new Button("Back") { Width = UIConstants.ButtonWidth };
            ContinueButton = new Button("Continue") { Width = UIConstants.ButtonWidth };

            AddWidget(InstructionLabel, 0, 0, 1, 2);
            AddWidget(ParameterIdNumeric, 1, 0, 1, 2);
            AddWidget(BackButton, 2, 0);
            AddWidget(ContinueButton, 2, 1);

            Width = UIConstants.ViewWidth;
            Height = UIConstants.ViewHeight;
        }

        public Label InstructionLabel { get; }

        public Numeric ParameterIdNumeric { get; }

        public Button BackButton { get; }

        public Button ContinueButton { get; }
    }

    /// <summary>View for the "Set Value" dialog.</summary>
    public class SetValueView : Dialog
    {
        public SetValueView(IEngine engine) : base(engine)
        {
            Title = "Step 3 – Set Parameter Value";

            // Row 0: String input
            StringLabel = new Label("String Value");
            StringTextBox = new TextBox { PlaceHolder = "Enter string value...", Width = UIConstants.TextBoxWidth };
            SetStringButton = new Button("Set String") { Width = UIConstants.ButtonWidth };

            // Row 1: Double input
            DoubleLabel = new Label("Double Value");
            DoubleNumeric = new Numeric { Decimals = 2, StepSize = 0.01 };
            SetDoubleButton = new Button("Set Double") { Width = UIConstants.ButtonWidth };

            // Row 2: Result feedback (read-only, multi-line)
            ResultTextBox = new TextBox
            {
                IsMultiline = true,
                IsEnabled = false,
                Height = UIConstants.ResultBoxHeight,
                PlaceHolder = "Result will appear here...",
            };

            // Row 3: Navigation
            BackButton = new Button("Back") { Width = UIConstants.ButtonWidth };
            ExitButton = new Button("Exit") { Width = UIConstants.ButtonWidth };

            AddWidget(StringLabel, 0, 0);
            AddWidget(StringTextBox, 0, 1);
            AddWidget(SetStringButton, 0, 2);

            AddWidget(DoubleLabel, 1, 0);
            AddWidget(DoubleNumeric, 1, 1);
            AddWidget(SetDoubleButton, 1, 2);

            AddWidget(ResultTextBox, 2, 0, 1, 3);

            AddWidget(BackButton, 3, 0);
            AddWidget(ExitButton, 3, 2);

            Width = UIConstants.ViewWidth;
        }

        public Label StringLabel { get; }

        public TextBox StringTextBox { get; }

        public Button SetStringButton { get; }

        public Label DoubleLabel { get; }

        public Numeric DoubleNumeric { get; }

        public Button SetDoubleButton { get; }

        public TextBox ResultTextBox { get; }

        public Button BackButton { get; }

        public Button ExitButton { get; }

        /// <summary>Displays a result message in the feedback box.</summary>
        public void ShowResult(string message) => ResultTextBox.Text = message;
    }
}