namespace ModelViewPresenter_Tests
{
	using System;
	using System.Linq;
	using Automation_1;
	using FluentAssertions;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	[TestClass]
	public sealed class ModelAndPresenterTests
	{
		private Mock<IEngine> engineMock;

		[TestInitialize]
		public void Setup()
		{
			engineMock = new Mock<IEngine>(MockBehavior.Strict);
		}

		[TestMethod]
		public void SelectElementModel_GetAllElements_ReturnsSortedElements()
		{
			// Arrange
			var elements = new[]
			{
				CreateFakeElement("Zeta"),
				CreateFakeElement("Alpha"),
				CreateFakeElement("Beta")
			};

			engineMock.Setup(e => e.FindElementsByName("*")).Returns(elements);

			var model = new SelectElementModel(engineMock.Object);

			// Act
			var result = model.GetAllElements().ToList();

			// Assert
			result.Should().HaveCount(3);
			result.Select(e => e.ElementName).Should().ContainInOrder("Alpha", "Beta", "Zeta");
		}

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

		[TestMethod]
		public void SelectParameterModel_ShouldStoreSelectedElementAndParameterId()
		{
			// Arrange
			var element = CreateFakeElement("TestElement");
			var model = new SelectParameterModel();

			// Act
			model.SelectedElement = element;
			model.ParameterId = 42;

			// Assert
			model.SelectedElement.ElementName.Should().Be("TestElement");
			model.ParameterId.Should().Be(42);
		}

		[TestMethod]
		public void SetValueModel_ShouldStoreSelectedElementAndParameterId()
		{
			// Arrange
			var element = CreateFakeElement("AnotherElement");
			var model = new SetValueModel();

			// Act
			model.SelectedElement = element;
			model.ParameterId = 99;

			// Assert
			model.SelectedElement.ElementName.Should().Be("AnotherElement");
			model.ParameterId.Should().Be(99);
		}

		[TestMethod]
		public void SelectElementPresenter_ShouldRaiseContinueRequested()
		{
			// Arrange
			var element = CreateFakeElement("Alpha");
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
				ParameterId = 5
			};
			var presenter = new SetValuePresenter(viewMock.Object, model, engineMock.Object);

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