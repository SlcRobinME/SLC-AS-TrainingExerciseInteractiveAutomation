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
			engineMock.Setup(e => e.FindElementsByName("*")).Returns(new[] { element });

			var model = new SelectElementModel(engineMock.Object);
			var view = new SelectElementView(engineMock.Object);
			var presenter = new SelectElementPresenter(view, model);

			presenter.LoadView();
			view.ElementDropDown.Selected = element;

			bool eventRaised = false;
			presenter.ContinueRequested += (s, e) => eventRaised = true;

			// Act
			presenter.SimulateContinuePressed();

			// Assert
			eventRaised.Should().BeTrue();
			model.SelectedElement.ElementName.Should().Be("Alpha");
		}

		[TestMethod]
		public void SelectParameterPresenter_ShouldRaiseContinueRequested()
		{
			// Arrange
			var model = new SelectParameterModel();
			var view = new SelectParameterView(engineMock.Object);
			var presenter = new SelectParameterPresenter(view, model);

			bool eventRaised = false;
			presenter.ContinueRequested += (s, e) => eventRaised = true;

			view.ParameterIdNumeric.Value = 123;

			// Act
			presenter.SimulateContinuePressed();

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

			var model = new SetValueModel
			{
				SelectedElement = elementMock.Object,
				ParameterId = 5
			};
			var view = new SetValueView(engineMock.Object);
			var presenter = new SetValuePresenter(view, model, engineMock.Object);

			view.StringTextBox.Text = "Hello";

			// Act
			presenter.SimulateSetStringPressed();

			// Assert
			view.ResultTextBox.Text.Should().Be("Success");
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