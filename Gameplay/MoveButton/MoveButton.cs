using Godot;

public partial class MoveButton : Area2D , IEventBusInjectable
{
    
    private EventBus _eventBus;

    private float _areaAngle; 


    public float Angle {
        get
        {
            return _areaAngle;
        }
    }

    public void Init(EventBus eventBus)
    {
        _eventBus = eventBus;
    }
    
    public override void _Ready()
    {
        _areaAngle = this.Rotation;
        


    }


    
    public override void _InputEvent(Viewport viewport, InputEvent inputEvent, int shapeIdx)
    {
        if (inputEvent is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                OnMouseClicked();
            }
        }
    }
    private void OnMouseClicked()
    {
        _eventBus.EmitMoveButtonClicked(this);
        
    }
}
