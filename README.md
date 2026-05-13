# Exercise Interactive Automation

## Description

Create a new interactive automation script that contains three dialogs.

Upon launching the script an initial dialog is show. This dialog contains a label asking the user to select an element on which he/she would like to set a parameter, together with a dropdown containing all elements on the DMA. A user should be able to select an element and proceed to the next dialog by clicking a "Continue..." button.

The second dialog contains a label asking the user to specify which parameter he/she would like to set. Specifying the parameter will be done based on the parameter ID. Add a numeric widget that the user can use to specify this ID. This dialog should also contain a "Continue..." button, together with a "Back..." button in case the user wants to edit another element.

On the third dialog, the user can specify what value he/she would like to set on the parameter. As we support two types of parameters (either text or a number) we will need to add widgets for both types. The first row will contain a label specifying "String Value", a TextBox and a "Set String Value" button. The second row will contain a label specifying "Double Value", a Numeric widget (displaying two decimals and a stepsize of 0.01) and a "Set Double Value" button. This dialog should also contain a multiline textbox that will display an exception if the parameter could not be set or "Success" if the set worked. The multiline textbox should be spanned across three columns. THis dialog should contain a "Back..." button that allows the user to navigate to the previous dialog together with an "Exit" button that stops the script.

## Pointers

* Add titles to every dialog indicating what the purpose of the dialog is.
* Use the OnChange event on the buttons to handle user input.
* Use [Skyline.DataMiner.Utils.InteractiveAutomationScriptToolkit](https://github.com/SkylineCommunications/Skyline.DataMiner.Utils.InteractiveAutomationScriptToolkit) NuGet.
* Use the Model-View-Presenter architectural pattern ([MVP Video](https://community.dataminer.services/courses/dataminer-automation/lessons/model-view-presenter/))
