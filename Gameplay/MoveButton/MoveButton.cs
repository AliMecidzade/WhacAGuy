using Godot;

public partial class MoveButton : Area2D , IEventBusInjectable
{

    [Export] private Texture2D moveButtonTexture; 
    [Export] private Texture2D moveButtonTexturePressed; 
    [Export] private Sprite2D _sprite;
    
    
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
                _sprite.Texture = moveButtonTexturePressed;
                OnMouseClicked();
            }
            else
            {
                _sprite.Texture = moveButtonTexture;
            }
        }
    }
    private void OnMouseClicked()
    {
        _eventBus.EmitMoveButtonClicked(this);
        
    }
}
