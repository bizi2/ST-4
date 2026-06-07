// Copyright 2025 UNN-CS
// Nazyrov A.A.

using Stateless;

namespace BugPro;

// ????????? ???????
public enum IssueState
{
    NewIssue,
    Triaged,
    InWork,
    Resolved,
    Verified,
    Done,
    ReopenedBug,
    Declined
}

// ????????-????????
public enum ActionTrigger
{
    AssignToDev,
    StartFixing,
    MarkResolved,
    VerifyFix,
    CloseIssue,
    ReopenIssue,
    RejectIssue
}

// ????? ??? ?????????? ????????? ?????? ????
public class BugLifecycle
{
    private StateMachine<IssueState, ActionTrigger> _stateMachine;
    private IssueState _currentStatus;

    public BugLifecycle()
    {
        _stateMachine = new StateMachine<IssueState, ActionTrigger>(
            () => _currentStatus, 
            status => _currentStatus = status
        );
        
        SetupTransitions();
        _currentStatus = IssueState.NewIssue;
    }
    
    private void SetupTransitions()
    {
        // NewIssue -> Triaged
        _stateMachine.Configure(IssueState.NewIssue)
            .Permit(ActionTrigger.AssignToDev, IssueState.Triaged)
            .Permit(ActionTrigger.RejectIssue, IssueState.Declined);
        
        // Triaged -> InWork
        _stateMachine.Configure(IssueState.Triaged)
            .Permit(ActionTrigger.StartFixing, IssueState.InWork)
            .Permit(ActionTrigger.RejectIssue, IssueState.Declined);
        
        // InWork -> Resolved
        _stateMachine.Configure(IssueState.InWork)
            .Permit(ActionTrigger.MarkResolved, IssueState.Resolved);
        
        // Resolved -> Verified
        _stateMachine.Configure(IssueState.Resolved)
            .Permit(ActionTrigger.VerifyFix, IssueState.Verified)
            .Permit(ActionTrigger.ReopenIssue, IssueState.ReopenedBug);
        
        // Verified -> Done
        _stateMachine.Configure(IssueState.Verified)
            .Permit(ActionTrigger.CloseIssue, IssueState.Done)
            .Permit(ActionTrigger.ReopenIssue, IssueState.ReopenedBug);
        
        // ReopenedBug -> Triaged
        _stateMachine.Configure(IssueState.ReopenedBug)
            .Permit(ActionTrigger.AssignToDev, IssueState.Triaged);
        
        // Declined -> ReopenedBug
        _stateMachine.Configure(IssueState.Declined)
            .Permit(ActionTrigger.ReopenIssue, IssueState.ReopenedBug);
        
        // Done ? ????????? ?????????
        _stateMachine.Configure(IssueState.Done)
            .Ignore(ActionTrigger.CloseIssue);
    }
    
    public IssueState GetCurrentStatus() => _stateMachine.State;
    
    public void DoAssign() => _stateMachine.Fire(ActionTrigger.AssignToDev);
    public void DoStartWork() => _stateMachine.Fire(ActionTrigger.StartFixing);
    public void DoResolve() => _stateMachine.Fire(ActionTrigger.MarkResolved);
    public void DoVerify() => _stateMachine.Fire(ActionTrigger.VerifyFix);
    public void DoClose() => _stateMachine.Fire(ActionTrigger.CloseIssue);
    public void DoReopen() => _stateMachine.Fire(ActionTrigger.ReopenIssue);
    public void DoReject() => _stateMachine.Fire(ActionTrigger.RejectIssue);
    
    public bool CanAssign() => _stateMachine.CanFire(ActionTrigger.AssignToDev);
    public bool CanStartWork() => _stateMachine.CanFire(ActionTrigger.StartFixing);
    public bool CanResolve() => _stateMachine.CanFire(ActionTrigger.MarkResolved);
    public bool CanVerify() => _stateMachine.CanFire(ActionTrigger.VerifyFix);
    public bool CanClose() => _stateMachine.CanFire(ActionTrigger.CloseIssue);
    public bool CanReopen() => _stateMachine.CanFire(ActionTrigger.ReopenIssue);
    public bool CanReject() => _stateMachine.CanFire(ActionTrigger.RejectIssue);
}

// ???????????????? ?????????
class ProgramEntry
{
    static void Main()
    {
        Console.WriteLine("=== Bug Workflow Simulation ===\n");
        
        var bugInstance = new BugLifecycle();
        Console.WriteLine($"Initial status: {bugInstance.GetCurrentStatus()}");
        
        bugInstance.DoAssign();
        Console.WriteLine($"After Assign: {bugInstance.GetCurrentStatus()}");
        
        bugInstance.DoStartWork();
        Console.WriteLine($"After Start Work: {bugInstance.GetCurrentStatus()}");
        
        bugInstance.DoResolve();
        Console.WriteLine($"After Resolve: {bugInstance.GetCurrentStatus()}");
        
        bugInstance.DoVerify();
        Console.WriteLine($"After Verify: {bugInstance.GetCurrentStatus()}");
        
        bugInstance.DoClose();
        Console.WriteLine($"After Close: {bugInstance.GetCurrentStatus()}\n");
        
        var reopenedBug = new BugLifecycle();
        reopenedBug.DoAssign();
        reopenedBug.DoStartWork();
        reopenedBug.DoResolve();
        reopenedBug.DoReopen();
        Console.WriteLine($"Reopened bug status: {reopenedBug.GetCurrentStatus()}");
    }
}
