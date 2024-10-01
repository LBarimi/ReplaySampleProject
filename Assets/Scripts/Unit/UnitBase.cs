public class UnitBase : IUpdater
{
    public bool IsPendingRemove { get; set; }

    protected virtual void Awake()
    {
    }
    
    protected virtual void Start()
    {
    }
    
    protected virtual void OnDestroy()
    {
    }

    public virtual void Init()
    {
    }
    
    public virtual void InitStat()
    {
    }
    
    public virtual void InitVisual()
    {   
    }
    
    protected virtual void Update()
    {
    }
    
    public virtual void OnFixedUpdate(float deltaTime)
    {
    }
}

public struct StructInputData
{
    private bool _isDown;
    private bool _isPress;
    private bool _isUp;

    public StructInputData(bool isDown, bool isPress, bool isUp)
    {
        _isDown = isDown;
        _isPress = isPress;
        _isUp = isUp;
    }
}