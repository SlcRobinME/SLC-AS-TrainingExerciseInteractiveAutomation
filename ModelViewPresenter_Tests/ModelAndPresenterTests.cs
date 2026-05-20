namespace ModelViewPresenter_Tests
{
	using System;
	using System.Linq;
	using Automation_1;
	using FluentAssertions;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;
	using Skyline.DataMiner.Automation;

	/// <summary>
	/// Should contain unit tests for the models and presenters of the MVP pattern, ensuring that the logic for retrieving elements, storing selected values, and handling user interactions is working correctly and can be maintained with confidence as the code evolves.
	/// </summary>
	[TestClass]
	public sealed class ModelAndPresenterTests
	{
		private Mock<IEngine>? engineMock;

		/// <summary>
		/// Should initialize a new instance of the SelectElementModel with a mocked IEngine, allowing us to control the behavior of the engine and test the model's logic in isolation without relying on an actual DataMiner environment.
		/// </summary>
		[TestInitialize]
		public void Setup()
		{
			this.engineMock = new Mock<IEngine>(MockBehavior.Strict);
		}

		/// <summary>
		/// Should return all elements sorted by name, ensuring that the user sees a well-organized list of elements to choose from, improving usability and efficiency when selecting an element from a potentially large list.
		/// </summary>
		[TestMethod]
		public void SelectElementModel_GetAllElements_ReturnsSortedElements()
		{
			// Arrange
			var elements = new[]
			{
				this.CreateFakeElement("Zeta"),
				this.CreateFakeElement("Alpha"),
				this.CreateFakeElement("Beta"),
			};

			engineMock.Setup(e => e.FindElementsByName("*")).Returns(elements);

			var model = new SelectElementModel(this.engineMock.Object);

			// Act
			var result = model.GetAllElements().ToList();

			// Assert
			result.Should().HaveCount(3);
			result.Select(e => e.ElementName).Should().ContainInOrder("Alpha", "Beta", "Zeta");
		}

		/// <summary>
		/// Should return an empty collection if no elements are found, ensuring that the application can gracefully handle cases where there are no elements to display without throwing exceptions.
		/// </summary>
		[TestMethod]
		public void SelectElementModel_GetAllElements_WhenNoElements_ReturnsEmpty()
		{
			// Arrange
			engineMock.Setup(e => e.FindElementsByName("*")).Returns((Element[])null);

			var model = new SelectElementModel(engineMock.Object);

			// Act
			var result = model.GetAllElements();

			// Assert
			result.Should().BeEmpty();
		}

		/// <summary>
		/// Should store the selected element and parameter ID in SelectParameterModel, which will be used in the final step to set the parameter value on the correct element and parameter.
		/// </summary>
		[TestMethod]
		public void SelectParameterModel_ShouldStoreSelectedElementAndParameterId()
		{
			// Arrange
			var element = this.CreateFakeElement("TestElement");
			var model = new SelectParameterModel();

			// Act
			model.SelectedElement = element;
			model.ParameterId = 42;

			// Assert
			model.SelectedElement.ElementName.Should().Be("TestElement");
			model.ParameterId.Should().Be(42);
		}

		/// <summary>
		/// Should store the selected element and parameter ID in SetValueModel, which will be used to set the parameter value in the final step.
		/// </summary>
		[TestMethod]
		public void SetValueModel_ShouldStoreSelectedElementAndParameterId()
		{
			// Arrange
			var element = this.CreateFakeElement("AnotherElement");
			var model = new SetValueModel();

			// Act
			model.SelectedElement = element;
			model.ParameterId = 99;

			// Assert
			model.SelectedElement.ElementName.Should().Be("AnotherElement");
			model.ParameterId.Should().Be(99);
		}

		/// <summary>
		/// Should raise ContinueRequested and update model with selected element when Continue button is pressed in SelectElementPresenter.
		/// </summary>
		[TestMethod]
		public void SelectElementPresenter_ShouldRaiseContinueRequested()
		{
			// Arrange
			var element = this.CreateFakeElement("Alpha");
			var buttonMock = new Mock<IButton>();
			var dropDown = new Mock<Automation_1.IDropDown<Element>>();
			var viewMock = new Mock<ISelectElementView>();

			viewMock.Setup(v => v.ContinueButton).Returns(buttonMock.Object);
			viewMock.Setup(v => v.ElementDropDown).Returns(dropDown.Object);
			dropDown.Setup(d => d.Selected).Returns(element);

			engineMock.Setup(e => e.FindElementsByName("*")).Returns(new[] { element });

			var model = new SelectElementModel(engineMock.Object);
			var presenter = new SelectElementPresenter((ISelectElementView)viewMock.Object, model);
			presenter.LoadView();

			bool eventRaised = false;
			presenter.ContinueRequested += (s, e) => eventRaised = true;

			// Act — direktno firamo event, nema Simulate metoda
			buttonMock.Raise(b => b.Pressed += null, EventArgs.Empty);

			// Assert
			eventRaised.Should().BeTrue();
			model.SelectedElement.ElementName.Should().Be("Alpha");
		}

		/// <summary>
		/// Should raise ContinueRequested and update model with parameter ID when Continue button is pressed in SelectParameterPresenter.
		/// </summary>
		[TestMethod]
		public void SelectParameterPresenter_ShouldRaiseContinueRequested()
		{
			// Arrange
			var continueButtonMock = new Mock<IButton>();
			var backButtonMock = new Mock<IButton>();
			var numericMock = new Mock<INumeric>();
			var viewMock = new Mock<ISelectParameterView>();

			viewMock.Setup(v => v.ContinueButton).Returns(continueButtonMock.Object);
			viewMock.Setup(v => v.BackButton).Returns(backButtonMock.Object);
			viewMock.Setup(v => v.ParameterIdNumeric).Returns(numericMock.Object);
			numericMock.Setup(n => n.Value).Returns(123);

			var model = new SelectParameterModel();
			var presenter = new SelectParameterPresenter(viewMock.Object, model);

			bool eventRaised = false;
			presenter.ContinueRequested += (s, e) => eventRaised = true;

			// Act
			continueButtonMock.Raise(b => b.Pressed += null, EventArgs.Empty);

			// Assert
			eventRaised.Should().BeTrue();
			model.ParameterId.Should().Be(123);
		}

		/// <summary>
		/// Should update the parameter value on the element and show success message when SetStringButton is pressed in SetValuePresenter.
		/// </summary>
		[TestMethod]
		public void SetValuePresenter_OnSetStringPressed_ShouldUpdateParameter()
		{
			// Arrange
			var elementMock = new Mock<Element>();
			elementMock.SetupGet(e => e.ElementName).Returns("Alpha");
			elementMock.Setup(e => e.SetParameter(It.IsAny<int>(), It.IsAny<object>()));

			engineMock.Setup(e => e.FindElement("Alpha")).Returns(elementMock.Object);

			var setStringButtonMock = new Mock<IButton>();
			var setDoubleButtonMock = new Mock<IButton>();
			var backButtonMock = new Mock<IButton>();
			var exitButtonMock = new Mock<IButton>();
			var stringTextBoxMock = new Mock<ITextBox>();
			var doubleNumericMock = new Mock<INumeric>();
			var resultTextBoxMock = new Mock<ITextBox>();
			var viewMock = new Mock<ISetValueView>();

			viewMock.Setup(v => v.SetStringButton).Returns(setStringButtonMock.Object);
			viewMock.Setup(v => v.SetDoubleButton).Returns(setDoubleButtonMock.Object);
			viewMock.Setup(v => v.BackButton).Returns(backButtonMock.Object);
			viewMock.Setup(v => v.ExitButton).Returns(exitButtonMock.Object);
			viewMock.Setup(v => v.StringTextBox).Returns(stringTextBoxMock.Object);
			viewMock.Setup(v => v.DoubleNumeric).Returns(doubleNumericMock.Object);
			stringTextBoxMock.Setup(t => t.Text).Returns("Hello");

			var model = new SetValueModel
			{
				SelectedElement = elementMock.Object,
				ParameterId = 5,
			};
			var presenter = new SetValuePresenter(viewMock.Object, model, this.engineMock.Object);

			// Act
			setStringButtonMock.Raise(b => b.Pressed += null, EventArgs.Empty);

			// Assert
			viewMock.Verify(v => v.ShowResult("Success"), Times.Once);
			elementMock.Verify(e => e.SetParameter(5, "Hello"), Times.Once);
		}

		// Helper method to create fake element with read-only ElementName
		private Element CreateFakeElement(string name)
		{
			var elementMock = new Mock<Element>();
			elementMock.SetupGet(e => e.ElementName).Returns(name);
			return elementMock.Object;
		}
	}
}