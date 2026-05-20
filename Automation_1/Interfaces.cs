namespace Automation_1
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Automation;

    public interface IButton
    {
        event EventHandler<EventArgs> Pressed;
    }

    public interface ISelectElementView
    {
        IButton ContinueButton { get; }

        IDropDown<Element> ElementDropDown { get; }
    }

    public interface ISelectParameterView
    {
        IButton BackButton { get; }

        IButton ContinueButton { get; }

        INumeric ParameterIdNumeric { get; }
    }

    public interface ISetValueView
    {
        IButton SetStringButton { get; }

        IButton SetDoubleButton { get; }

        IButton BackButton { get; }

        IButton ExitButton { get; }

        ITextBox StringTextBox { get; }

        INumeric DoubleNumeric { get; }

        void ShowResult(string message);
    }

    public interface IDropDown<T>
    {
        T Selected { get; set; }

        void SetOptions(List<Skyline.DataMiner.Utils.InteractiveAutomationScript.Option<Element>> options);
    }

    public interface INumeric
    {
        double Value { get; set; }
    }

    public interface ITextBox
    {
        string Text { get; set; }
    }
}