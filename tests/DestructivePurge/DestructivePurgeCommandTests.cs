namespace ScaddinsTestProject.DestructivePurge
{
    using System;
    using System.Collections.Generic;
    using Autodesk.Revit.ApplicationServices;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using NSubstitute;
    using NUnit.Framework;
    using SCaddins.DestructivePurge;
    using SCaddinsTestProject;

    [TestFixture]
    public class CommandTests : OneTimeOpenDocumentTest
    {
        private Command _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new Command();
        }

        //[Test]
        //public void CanCallExecute()
        //{
        //    // Arrange
        //    var commandData = new ExternalCommandData
        //    {
        //        JournalData = Substitute.For<IDictionary<string, string>>(),
        //        View = new View(),
        //        Application = new UIApplication(new Application())
        //    };
        //    var message = "TestValue1003394312";
        //    var elements = new ElementSet();

        //    // Act
        //    var result = _testClass.Execute(commandData, ref message, elements);

        //    // Assert
        //    Assert.Fail("Create or modify test");
        //}

        //[Test]
        //public void CannotCallExecuteWithNullCommandData()
        //{
        //    var message = "TestValue255100026";
        //    Assert.Throws<ArgumentNullException>(() => _testClass.Execute(default(ExternalCommandData), ref message, new ElementSet()));
        //}

        //[Test]
        //public void CannotCallExecuteWithNullElements()
        //{
        //    var message = "TestValue716388863";
        //    Assert.Throws<ArgumentNullException>(() => _testClass.Execute(new ExternalCommandData
        //    {
        //        JournalData = Substitute.For<IDictionary<string, string>>(),
        //        View = new View(),
        //        Application = new UIApplication(new Application())
        //    }, ref message, default(ElementSet)));
        //}

        //[TestCase(null)]
        //[TestCase("")]
        //[TestCase("   ")]
        //public void CannotCallExecuteWithInvalidMessage(string value)
        //{
        //    var message = default(string);
        //    Assert.Throws<ArgumentNullException>(() => _testClass.Execute(new ExternalCommandData
        //    {
        //        JournalData = Substitute.For<IDictionary<string, string>>(),
        //        View = new View(),
        //        Application = new UIApplication(new Application())
        //    }, ref message, new ElementSet()));
        //}
    }
}