using Stateless;

// Демонстрация работы класса Bug
var bug = new Bug(1, "Critical bug in login");
Console.WriteLine($"Bug #{bug.Id}: {bug.Description}");
Console.WriteLine($"Current state: {bug.State}");

// Переходим между состояниями
bug.Assign("John Doe");
Console.WriteLine($"State after Assign: {bug.State}");

bug.StartWork();
Console.WriteLine($"State after StartWork: {bug.State}");

bug.PauseWork();
Console.WriteLine($"State after PauseWork: {bug.State}");

bug.ResumeWork();
Console.WriteLine($"State after ResumeWork: {bug.State}");

bug.FixBug();
Console.WriteLine($"State after FixBug: {bug.State}");

bug.VerifyFix();
Console.WriteLine($"State after VerifyFix: {bug.State}");

Console.WriteLine("\n=== Bug State Machine Demo ===\n");

/// <summary>
/// Класс Bug описывает workflow работы с багом
/// </summary>
public class Bug
{
    // Определяем возможные состояния
    public enum BugState
    {
        New,        // Новый баг
        Assigned,   // Назначен исполнителю
        InProgress, // В работе
        OnHold,     // На паузе
        Fixed,      // Исправлен
        Closed,     // Закрыт
        Rejected    // Отклонен
    }

    // Определяем возможные триггеры (события)
    public enum BugTrigger
    {
        Assign,        // Назначить
        StartWork,     // Начать работу
        PauseWork,     // Приостановить работу
        ResumeWork,    // Возобновить работу
        FixBug,        // Исправить баг
        VerifyFix,     // Проверить исправление
        Reject,        // Отклонить
        Close          // Закрыть
    }

    private readonly StateMachine<BugState, BugTrigger> _stateMachine;

    public int Id { get; }
    public string Description { get; }
    public string AssignedTo { get; private set; } = "Unassigned";
    
    public BugState State => _stateMachine.State;

    public Bug(int id, string description)
    {
        Id = id;
        Description = description;

        // Инициализируем автомат состояний
        _stateMachine = new StateMachine<BugState, BugTrigger>(BugState.New);

        // Определяем переходы из состояния New
        _stateMachine.Configure(BugState.New)
            .Permit(BugTrigger.Assign, BugState.Assigned)
            .Permit(BugTrigger.Reject, BugState.Rejected);

        // Определяем переходы из состояния Assigned
        _stateMachine.Configure(BugState.Assigned)
            .Permit(BugTrigger.StartWork, BugState.InProgress)
            .Permit(BugTrigger.Reject, BugState.Rejected);

        // Определяем переходы из состояния InProgress
        _stateMachine.Configure(BugState.InProgress)
            .Permit(BugTrigger.PauseWork, BugState.OnHold)
            .Permit(BugTrigger.FixBug, BugState.Fixed)
            .Permit(BugTrigger.Reject, BugState.Rejected);

        // Определяем переходы из состояния OnHold
        _stateMachine.Configure(BugState.OnHold)
            .Permit(BugTrigger.ResumeWork, BugState.InProgress)
            .Permit(BugTrigger.Reject, BugState.Rejected);

        // Определяем переходы из состояния Fixed
        _stateMachine.Configure(BugState.Fixed)
            .Permit(BugTrigger.VerifyFix, BugState.Closed)
            .Permit(BugTrigger.Reject, BugState.InProgress);

        // Состояние Closed и Rejected - финальные состояния
        _stateMachine.Configure(BugState.Closed);
        _stateMachine.Configure(BugState.Rejected);
    }

    public void Assign(string assignee)
    {
        AssignedTo = assignee;
        _stateMachine.Fire(BugTrigger.Assign);
    }

    public void StartWork()
    {
        _stateMachine.Fire(BugTrigger.StartWork);
    }

    public void PauseWork()
    {
        _stateMachine.Fire(BugTrigger.PauseWork);
    }

    public void ResumeWork()
    {
        _stateMachine.Fire(BugTrigger.ResumeWork);
    }

    public void FixBug()
    {
        _stateMachine.Fire(BugTrigger.FixBug);
    }

    public void VerifyFix()
    {
        _stateMachine.Fire(BugTrigger.VerifyFix);
    }

    public void Reject()
    {
        _stateMachine.Fire(BugTrigger.Reject);
    }

    public void Close()
    {
        _stateMachine.Fire(BugTrigger.Close);
    }

    /// <summary>
    /// Проверяет, может ли быть выполнен переход по заданному триггеру
    /// </summary>
    public bool CanTransition(BugTrigger trigger)
    {
        return _stateMachine.CanFire(trigger);
    }
}
