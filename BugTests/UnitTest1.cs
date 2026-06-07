// Copyright 2025 UNN-CS
// Nazyrov A.A.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;

namespace BugTests;

[TestClass]
public class BugLifecycleTests
{
    private BugLifecycle _bugUnderTest;

    [TestInitialize]
    public void SetupTest()
    {
        _bugUnderTest = new BugLifecycle();
    }

    // ===== ????????? ????????? =====
    [TestMethod]
    public void NewBug_ShouldBeInNewIssueState()
    {
        Assert.AreEqual(IssueState.NewIssue, _bugUnderTest.GetCurrentStatus());
    }

    // ===== ???????? ?? NewIssue =====
    [TestMethod]
    public void AssignAction_FromNew_ShouldMoveToTriaged()
    {
        _bugUnderTest.DoAssign();
        Assert.AreEqual(IssueState.Triaged, _bugUnderTest.GetCurrentStatus());
    }
    
    [TestMethod]
    public void RejectAction_FromNew_ShouldMoveToDeclined()
    {
        _bugUnderTest.DoReject();
        Assert.AreEqual(IssueState.Declined, _bugUnderTest.GetCurrentStatus());
    }
    
    // ===== ???????? ?? Triaged =====
    [TestMethod]
    public void StartWorkAction_FromTriaged_ShouldMoveToInWork()
    {
        _bugUnderTest.DoAssign();
        _bugUnderTest.DoStartWork();
        Assert.AreEqual(IssueState.InWork, _bugUnderTest.GetCurrentStatus());
    }
    
    [TestMethod]
    public void RejectAction_FromTriaged_ShouldMoveToDeclined()
    {
        _bugUnderTest.DoAssign();
        _bugUnderTest.DoReject();
        Assert.AreEqual(IssueState.Declined, _bugUnderTest.GetCurrentStatus());
    }
    
    // ===== ???????? ?? InWork =====
    [TestMethod]
    public void ResolveAction_FromInWork_ShouldMoveToResolved()
    {
        _bugUnderTest.DoAssign();
        _bugUnderTest.DoStartWork();
        _bugUnderTest.DoResolve();
        Assert.AreEqual(IssueState.Resolved, _bugUnderTest.GetCurrentStatus());
    }
    
    // ===== ???????? ?? Resolved =====
    [TestMethod]
    public void VerifyAction_FromResolved_ShouldMoveToVerified()
    {
        _bugUnderTest.DoAssign();
        _bugUnderTest.DoStartWork();
        _bugUnderTest.DoResolve();
        _bugUnderTest.DoVerify();
        Assert.AreEqual(IssueState.Verified, _bugUnderTest.GetCurrentStatus());
    }
    
    [TestMethod]
    public void ReopenAction_FromResolved_ShouldMoveToReopenedBug()
    {
        _bugUnderTest.DoAssign();
        _bugUnderTest.DoStartWork();
        _bugUnderTest.DoResolve();
        _bugUnderTest.DoReopen();
        Assert.AreEqual(IssueState.ReopenedBug, _bugUnderTest.GetCurrentStatus());
    }
    
    // ===== ???????? ?? Verified =====
    [TestMethod]
    public void CloseAction_FromVerified_ShouldMoveToDone()
    {
        _bugUnderTest.DoAssign();
        _bugUnderTest.DoStartWork();
        _bugUnderTest.DoResolve();
        _bugUnderTest.DoVerify();
        _bugUnderTest.DoClose();
        Assert.AreEqual(IssueState.Done, _bugUnderTest.GetCurrentStatus());
    }
    
    [TestMethod]
    public void ReopenAction_FromVerified_ShouldMoveToReopenedBug()
    {
        _bugUnderTest.DoAssign();
        _bugUnderTest.DoStartWork();
        _bugUnderTest.DoResolve();
        _bugUnderTest.DoVerify();
        _bugUnderTest.DoReopen();
        Assert.AreEqual(IssueState.ReopenedBug, _bugUnderTest.GetCurrentStatus());
    }
    
    // ===== ???????? ?? ReopenedBug =====
    [TestMethod]
    public void AssignAction_FromReopened_ShouldMoveToTriaged()
    {
        _bugUnderTest.DoAssign();
        _bugUnderTest.DoStartWork();
        _bugUnderTest.DoResolve();
        _bugUnderTest.DoReopen();
        _bugUnderTest.DoAssign();
        Assert.AreEqual(IssueState.Triaged, _bugUnderTest.GetCurrentStatus());
    }
    
    // ===== ???????? ?? Declined =====
    [TestMethod]
    public void ReopenAction_FromDeclined_ShouldMoveToReopenedBug()
    {
        _bugUnderTest.DoReject();
        _bugUnderTest.DoReopen();
        Assert.AreEqual(IssueState.ReopenedBug, _bugUnderTest.GetCurrentStatus());
    }
    
    // ===== ?????? ????? =====
    [TestMethod]
    public void FullHappyPath_ShouldEndInDone()
    {
        _bugUnderTest.DoAssign();
        _bugUnderTest.DoStartWork();
        _bugUnderTest.DoResolve();
        _bugUnderTest.DoVerify();
        _bugUnderTest.DoClose();
        Assert.AreEqual(IssueState.Done, _bugUnderTest.GetCurrentStatus());
    }
    
    [TestMethod]
    public void RejectThenReopenThenAssign_ShouldWork()
    {
        _bugUnderTest.DoReject();
        _bugUnderTest.DoReopen();
        _bugUnderTest.DoAssign();
        Assert.AreEqual(IssueState.Triaged, _bugUnderTest.GetCurrentStatus());
    }
    
    // ===== ???????? CanFire =====
    [TestMethod]
    public void CanAssign_FromNew_ShouldBeTrue()
    {
        Assert.IsTrue(_bugUnderTest.CanAssign());
    }
    
    [TestMethod]
    public void CanAssign_FromTriaged_ShouldBeFalse()
    {
        _bugUnderTest.DoAssign();
        Assert.IsFalse(_bugUnderTest.CanAssign());
    }
    
    [TestMethod]
    public void CanReject_FromNew_ShouldBeTrue()
    {
        Assert.IsTrue(_bugUnderTest.CanReject());
    }
    
    [TestMethod]
    public void CanReopen_FromResolved_ShouldBeTrue()
    {
        _bugUnderTest.DoAssign();
        _bugUnderTest.DoStartWork();
        _bugUnderTest.DoResolve();
        Assert.IsTrue(_bugUnderTest.CanReopen());
    }
    
    // ===== ???????? ?????????? =====
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void StartWork_FromNew_ShouldThrowException()
    {
        _bugUnderTest.DoStartWork();
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Resolve_FromNew_ShouldThrowException()
    {
        _bugUnderTest.DoResolve();
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Verify_FromTriaged_ShouldThrowException()
    {
        _bugUnderTest.DoAssign();
        _bugUnderTest.DoVerify();
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Close_FromResolved_ShouldThrowException()
    {
        _bugUnderTest.DoAssign();
        _bugUnderTest.DoStartWork();
        _bugUnderTest.DoResolve();
        _bugUnderTest.DoClose();
    }
}
