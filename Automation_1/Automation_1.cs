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
	using System.Text;
	using Skyline.DataMiner.Automation;
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
		public void Run(Engine engine)
        {
            //// engine.ShowUI();

            var controller = new InteractiveController(engine);

            // Models
            var elementModel = new SelectElementModel(engine);
            var parameterModel = new SelectParameterModel();
            var valueModel = new SetValueModel();

            // Views
            var elementView = new SelectElementView(engine);
            var parameterView = new SelectParameterView(engine);
            var valueView = new SetValueView(engine);

            var elementPresenter = new SelectElementPresenter(elementView, elementModel);
            var parameterPresenter = new SelectParameterPresenter(parameterView, parameterModel);
            var valuePresenter = new SetValuePresenter(valueView, valueModel, engine);

            elementPresenter.OnContinue += (s, e) =>
            {
                parameterModel.SelectedElement = elementModel.SelectedElement;
                parameterPresenter.LoadView();
                controller.ShowDialog(parameterView);
            };

            parameterPresenter.OnContinue += (s, e) =>
            {
                valueModel.SelectedElement = parameterModel.SelectedElement;
                valueModel.ParameterId = parameterModel.ParametarId;
                valuePresenter.LoadView();
                controller.ShowDialog(valueView);
            };

            parameterPresenter.OnBack += (s, e) =>
            {
                elementPresenter.LoadView();
                controller.ShowDialog(elementView);
            };

            valuePresenter.OnBack += (s, e) =>
            {
                parameterPresenter.LoadView();
                controller.ShowDialog(parameterView);
            };

            valuePresenter.OnExit += (s, e) =>
            {
                engine.ExitSuccess("Script finished.");
            };

            elementPresenter.LoadView();
            controller.ShowDialog(elementView);
        }
    }
}