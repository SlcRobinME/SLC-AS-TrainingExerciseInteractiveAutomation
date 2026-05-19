/*
****************************************************************************
*  Copyright (c),  Skyline Communications NV  All Rights Reserved.        *
****************************************************************************

Revision History:

DATE		VERSION		AUTHOR			COMMENTS

15/05/2026	1.0.0.1		AMO, Skyline	Initial version
18/05/2026	1.0.0.2		AMO, Skyline	Pretier formatting
****************************************************************************
*/

/*
****************************************************************************
*  SetParameter – Interactive Automation Script
*  MVP pattern: Model / View / Presenter
****************************************************************************
*/

namespace Automation_1
{
    using System;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>DataMiner Automation script entry point.</summary>
    public class Script
    {
        /// <summary>The script entry point.</summary>
        /// <param name="engine">Link with SLAutomation process.</param>
        public void Run(IEngine engine)
        {
            try
            {
                engine.FindInteractiveClient("SetParameter Script", 100, "user:" + engine.UserLoginName);
                engine.SetFlag(RunTimeFlags.NoKeyCaching);
                engine.Timeout = TimeSpan.FromHours(10);

                RunSafe(engine);
            }
            catch (ScriptAbortException) { throw; }
            catch (ScriptForceAbortException) { throw; }
            catch (ScriptTimeoutException) { throw; }
            catch (InteractiveUserDetachedException) { throw; }
            catch (Exception ex)
            {
                engine.ExitFail($"Run|Something went wrong: {ex}");
            }
        }

        private void RunSafe(IEngine engine)
        {
            var controller = new InteractiveController(engine);

            // --- Models ---
            var elementModel = new SelectElementModel(engine);
            var parameterModel = new SelectParameterModel();
            var valueModel = new SetValueModel();

            // --- Views ---
            var elementView = new SelectElementView(engine);
            var parameterView = new SelectParameterView(engine);
            var valueView = new SetValueView(engine);

            // --- Presenters ---
            var elementPresenter = new SelectElementPresenter((ISelectElementView)elementView, elementModel);
            var parameterPresenter = new SelectParameterPresenter((ISelectParameterView)parameterView, parameterModel);
            var valuePresenter = new SetValuePresenter((ISetValueView)valueView, valueModel, engine);

            // Step 1 → Step 2
            elementPresenter.ContinueRequested += (s, e) =>
            {
                parameterModel.SelectedElement = elementModel.SelectedElement;
                parameterPresenter.LoadView();
                controller.ShowDialog(parameterView);
            };

            // Step 2 → Step 1 (Back)
            parameterPresenter.BackRequested += (s, e) =>
            {
                elementPresenter.LoadView();
                controller.ShowDialog(elementView);
            };

            // Step 2 → Step 3
            parameterPresenter.ContinueRequested += (s, e) =>
            {
                valueModel.SelectedElement = parameterModel.SelectedElement;
                valueModel.ParameterId = parameterModel.ParameterId;
                valuePresenter.LoadView();
                controller.ShowDialog(valueView);
            };

            // Step 3 → Step 2 (Back)
            valuePresenter.BackRequested += (s, e) =>
            {
                parameterPresenter.LoadView();
                controller.ShowDialog(parameterView);
            };

            // Step 3 → Exit
            valuePresenter.ExitRequested += (s, e) =>
            {
                engine.ExitSuccess("Script finished.");
            };

            // --- Start ---
            elementPresenter.LoadView();
            controller.ShowDialog(elementView);
        }
    }
}