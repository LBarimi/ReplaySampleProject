// 매니저에서 업데이트를 관리하도록 작업된 인터페이스.
public interface IUpdater
{
    public bool IsMarkedForRemoval { get; set; }
    
    public void OnFixedUpdate(float deltaTime);
    
    public void OnFixedAfterUpdate(float deltaTime);
}