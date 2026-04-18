namespace BugTests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestBugInitialization()
    {
        // Arrange & Act
        var bug = new Bug(1, "Test bug");

        // Assert
        Assert.AreEqual(1, bug.Id);
        Assert.AreEqual("Test bug", bug.Description);
        Assert.AreEqual(Bug.BugState.New, bug.State);
        Assert.AreEqual("Unassigned", bug.AssignedTo);
    }

    [TestMethod]
    public void TestBugAssignment()
    {
        // Arrange
        var bug = new Bug(2, "Test bug");

        // Act
        bug.Assign("John Doe");

        // Assert
        Assert.AreEqual("John Doe", bug.AssignedTo);
        Assert.AreEqual(Bug.BugState.Assigned, bug.State);
    }

    [TestMethod]
    public void TestTransitionFromNewToAssigned()
    {
        // Arrange
        var bug = new Bug(3, "Test bug");

        // Act
        bug.Assign("Jane Smith");

        // Assert
        Assert.AreEqual(Bug.BugState.Assigned, bug.State);
    }

    [TestMethod]
    public void TestTransitionFromAssignedToInProgress()
    {
        // Arrange
        var bug = new Bug(4, "Test bug");
        bug.Assign("Team lead");

        // Act
        bug.StartWork();

        // Assert
        Assert.AreEqual(Bug.BugState.InProgress, bug.State);
    }

    [TestMethod]
    public void TestTransitionFromInProgressToOnHold()
    {
        // Arrange
        var bug = new Bug(5, "Test bug");
        bug.Assign("Developer");
        bug.StartWork();

        // Act
        bug.PauseWork();

        // Assert
        Assert.AreEqual(Bug.BugState.OnHold, bug.State);
    }

    [TestMethod]
    public void TestTransitionFromOnHoldBackToInProgress()
    {
        // Arrange
        var bug = new Bug(6, "Test bug");
        bug.Assign("Developer");
        bug.StartWork();
        bug.PauseWork();

        // Act
        bug.ResumeWork();

        // Assert
        Assert.AreEqual(Bug.BugState.InProgress, bug.State);
    }

    [TestMethod]
    public void TestTransitionFromInProgressToFixed()
    {
        // Arrange
        var bug = new Bug(7, "Test bug");
        bug.Assign("Developer");
        bug.StartWork();

        // Act
        bug.FixBug();

        // Assert
        Assert.AreEqual(Bug.BugState.Fixed, bug.State);
    }

    [TestMethod]
    public void TestTransitionFromFixedToClosed()
    {
        // Arrange
        var bug = new Bug(8, "Test bug");
        bug.Assign("Developer");
        bug.StartWork();
        bug.FixBug();

        // Act
        bug.VerifyFix();

        // Assert
        Assert.AreEqual(Bug.BugState.Closed, bug.State);
    }

    [TestMethod]
    public void TestRejectBugFromNew()
    {
        // Arrange
        var bug = new Bug(9, "Test bug");

        // Act
        bug.Reject();

        // Assert
        Assert.AreEqual(Bug.BugState.Rejected, bug.State);
    }

    [TestMethod]
    public void TestRejectBugFromAssigned()
    {
        // Arrange
        var bug = new Bug(10, "Test bug");
        bug.Assign("Developer");

        // Act
        bug.Reject();

        // Assert
        Assert.AreEqual(Bug.BugState.Rejected, bug.State);
    }

    [TestMethod]
    public void TestRejectBugFromInProgress()
    {
        // Arrange
        var bug = new Bug(11, "Test bug");
        bug.Assign("Developer");
        bug.StartWork();

        // Act
        bug.Reject();

        // Assert
        Assert.AreEqual(Bug.BugState.Rejected, bug.State);
    }

    [TestMethod]
    public void TestRejectBugFromOnHold()
    {
        // Arrange
        var bug = new Bug(12, "Test bug");
        bug.Assign("Developer");
        bug.StartWork();
        bug.PauseWork();

        // Act
        bug.Reject();

        // Assert
        Assert.AreEqual(Bug.BugState.Rejected, bug.State);
    }

    [TestMethod]
    public void TestRejectBugFromFixed()
    {
        // Arrange
        var bug = new Bug(13, "Test bug");
        bug.Assign("Developer");
        bug.StartWork();
        bug.FixBug();

        // Act
        bug.Reject();

        // Assert
        // После отклонения из Fixed возвращаемся в InProgress
        Assert.AreEqual(Bug.BugState.InProgress, bug.State);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestInvalidTransitionStartWorkFromNew()
    {
        // Arrange
        var bug = new Bug(14, "Test bug");

        // Act - должно вызвать исключение
        bug.StartWork();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestInvalidTransitionFixBugFromAssigned()
    {
        // Arrange
        var bug = new Bug(15, "Test bug");
        bug.Assign("Developer");

        // Act - должно вызвать исключение
        bug.FixBug();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestInvalidTransitionPauseWorkFromNew()
    {
        // Arrange
        var bug = new Bug(16, "Test bug");

        // Act - должно вызвать исключение
        bug.PauseWork();
    }

    [TestMethod]
    public void TestCanTransitionTrue()
    {
        // Arrange
        var bug = new Bug(17, "Test bug");

        // Act & Assert
        Assert.IsTrue(bug.CanTransition(Bug.BugTrigger.Assign));
        Assert.IsTrue(bug.CanTransition(Bug.BugTrigger.Reject));
    }

    [TestMethod]
    public void TestCanTransitionFalse()
    {
        // Arrange
        var bug = new Bug(18, "Test bug");

        // Act & Assert
        Assert.IsFalse(bug.CanTransition(Bug.BugTrigger.StartWork));
        Assert.IsFalse(bug.CanTransition(Bug.BugTrigger.FixBug));
        Assert.IsFalse(bug.CanTransition(Bug.BugTrigger.VerifyFix));
    }

    [TestMethod]
    public void TestCompleteWorkflow()
    {
        // Arrange
        var bug = new Bug(19, "Critical bug");

        // Act
        bug.Assign("Alice");
        Assert.AreEqual(Bug.BugState.Assigned, bug.State);

        bug.StartWork();
        Assert.AreEqual(Bug.BugState.InProgress, bug.State);

        bug.PauseWork();
        Assert.AreEqual(Bug.BugState.OnHold, bug.State);

        bug.ResumeWork();
        Assert.AreEqual(Bug.BugState.InProgress, bug.State);

        bug.FixBug();
        Assert.AreEqual(Bug.BugState.Fixed, bug.State);

        bug.VerifyFix();
        Assert.AreEqual(Bug.BugState.Closed, bug.State);

        // Assert
        Assert.AreEqual("Alice", bug.AssignedTo);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestInvalidTransitionResumeWorkFromNew()
    {
        // Arrange
        var bug = new Bug(20, "Test bug");

        // Act - должно вызвать исключение
        bug.ResumeWork();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestInvalidTransitionVerifyFixFromInProgress()
    {
        // Arrange
        var bug = new Bug(21, "Test bug");
        bug.Assign("Developer");
        bug.StartWork();

        // Act - должно вызвать исключение
        bug.VerifyFix();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestInvalidTransitionAssignTwice()
    {
        // Arrange
        var bug = new Bug(22, "Test bug");
        bug.Assign("Developer1");

        // Act - должно вызвать исключение (уже назначено)
        bug.Assign("Developer2");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestInvalidTransitionPauseWorkFromFixed()
    {
        // Arrange
        var bug = new Bug(23, "Test bug");
        bug.Assign("Developer");
        bug.StartWork();
        bug.FixBug();

        // Act - должно вызвать исключение
        bug.PauseWork();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestInvalidTransitionCloseFromNew()
    {
        // Arrange
        var bug = new Bug(24, "Test bug");

        // Act - Close только из Closed
        bug.Close();
    }

    [TestMethod]
    public void TestMultiplePauseResumeSequence()
    {
        // Arrange
        var bug = new Bug(25, "Test bug");
        bug.Assign("Developer");
        bug.StartWork();

        // Act & Assert - первая пауза-возобновление
        bug.PauseWork();
        Assert.AreEqual(Bug.BugState.OnHold, bug.State);
        bug.ResumeWork();
        Assert.AreEqual(Bug.BugState.InProgress, bug.State);

        // Вторая пауза-возобновление
        bug.PauseWork();
        Assert.AreEqual(Bug.BugState.OnHold, bug.State);
        bug.ResumeWork();
        Assert.AreEqual(Bug.BugState.InProgress, bug.State);
    }

    [TestMethod]
    public void TestRejectFromAllValidStates()
    {
        // Arrange & Act & Assert
        // Из New
        var bug1 = new Bug(26, "Test");
        bug1.Reject();
        Assert.AreEqual(Bug.BugState.Rejected, bug1.State);

        // Из Assigned
        var bug2 = new Bug(27, "Test");
        bug2.Assign("Dev");
        bug2.Reject();
        Assert.AreEqual(Bug.BugState.Rejected, bug2.State);

        // Из InProgress
        var bug3 = new Bug(28, "Test");
        bug3.Assign("Dev");
        bug3.StartWork();
        bug3.Reject();
        Assert.AreEqual(Bug.BugState.Rejected, bug3.State);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestInvalidTransitionFixBugFromOnHold()
    {
        // Arrange
        var bug = new Bug(29, "Test bug");
        bug.Assign("Developer");
        bug.StartWork();
        bug.PauseWork();

        // Act - должно вызвать исключение
        bug.FixBug();
    }

    [TestMethod]
    public void TestCanTransitionAfterStateChange()
    {
        // Arrange & Act
        var bug = new Bug(30, "Test bug");

        // Initially in New state
        Assert.IsTrue(bug.CanTransition(Bug.BugTrigger.Assign));
        Assert.IsFalse(bug.CanTransition(Bug.BugTrigger.StartWork));

        // Move to Assigned
        bug.Assign("Dev");
        Assert.IsTrue(bug.CanTransition(Bug.BugTrigger.StartWork));
        Assert.IsFalse(bug.CanTransition(Bug.BugTrigger.PauseWork));

        // Move to InProgress
        bug.StartWork();
        Assert.IsTrue(bug.CanTransition(Bug.BugTrigger.PauseWork));
        Assert.IsTrue(bug.CanTransition(Bug.BugTrigger.FixBug));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestInvalidTransitionStartWorkFromOnHold()
    {
        // Arrange
        var bug = new Bug(31, "Test bug");
        bug.Assign("Developer");
        bug.StartWork();
        bug.PauseWork();

        // Act - должно вызвать исключение (из OnHold нельзя прямо в InProgress через StartWork)
        // Должны использовать ResumeWork
        bug.StartWork();
    }

    [TestMethod]
    public void TestAssigneeUpdate()
    {
        // Arrange
        var bug = new Bug(32, "Test bug");
        Assert.AreEqual("Unassigned", bug.AssignedTo);

        // Act
        bug.Assign("Alice");
        Assert.AreEqual("Alice", bug.AssignedTo);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestInvalidTransitionRejectFromClosed()
    {
        // Arrange
        var bug = new Bug(33, "Test bug");
        bug.Assign("Developer");
        bug.StartWork();
        bug.FixBug();
        bug.VerifyFix();

        // Act - Closed - финальное состояние
        bug.Reject();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestInvalidTransitionVerifyFixFromOnHold()
    {
        // Arrange
        var bug = new Bug(34, "Test bug");
        bug.Assign("Developer");
        bug.StartWork();
        bug.PauseWork();

        // Act - должно вызвать исключение
        bug.VerifyFix();
    }
}