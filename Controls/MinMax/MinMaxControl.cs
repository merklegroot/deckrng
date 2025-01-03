using Godot;

public partial class MinMaxControl : Control
{
	[Signal]
	public delegate void ValueChangedEventHandler(int delta);

	private bool _wasLeftTriggerActive = false;
	private bool _wasRightTriggerActive = false;

    private bool _isMax = false;
    
    [Export]
    public bool IsMax
    {
        get { return _isMax; }
        set
        { 
            _isMax = value;
            UpdateDisplay();
        }
    }

    private int _value = 1;

    [Export]
    public int Value
    {
        get { return _value; }
        set 
        { 
            if (_value != value)
            {
                _value = value;
                UpdateDisplay();
            }
        }
    }

    private Label _valueLabel => GetNode<Label>("valueLabel");
    private Label _headerLabel => GetNode<Label>("headerLabel");

    private Button _upButton;
    private Button _downButton;

    private Texture2D _steamDeckL1;
    private Texture2D _steamDeckL2;
    private Texture2D _steamDeckR1;
    private Texture2D _steamDeckR2;

    public override void _Ready()
    {
        _upButton = GetNode<Button>("ButtonContainer/upButton");
        _upButton.Pressed += OnUp;

        _downButton = GetNode<Button>("ButtonContainer/downButton");
        _downButton.Pressed += OnDown;

        // Load textures
        _steamDeckL1 = GD.Load<Texture2D>("res://addons/controller_icons/assets/steamdeck/l1.png");
        _steamDeckL2 = GD.Load<Texture2D>("res://addons/controller_icons/assets/steamdeck/l2.png");
        _steamDeckR1 = GD.Load<Texture2D>("res://addons/controller_icons/assets/steamdeck/r1.png");
        _steamDeckR2 = GD.Load<Texture2D>("res://addons/controller_icons/assets/steamdeck/r2.png");

        UpdateDisplay();
    }

    public override void _ExitTree()
    {
        if (_upButton != null)
        {
            _upButton.Pressed -= OnUp;
        }

        if (_downButton != null)
        {
            _downButton.Pressed -= OnDown;
        }

        base._ExitTree();
    }


    private void OnUp()
    {
        EmitSignal(SignalName.ValueChanged, 1);
    }

    private void OnDown()
    {
        EmitSignal(SignalName.ValueChanged, -1);
    }

    private void MinDelta(int delta)
    {
        if (IsMax) return;

        EmitSignal(SignalName.ValueChanged, delta);
    }

    private void MaxDelta(int delta)
    {
        if (!IsMax) return;

        EmitSignal(SignalName.ValueChanged, delta);
    }

    private void UpdateDisplay()
    {
        if (_valueLabel != null)
            _valueLabel.Text = Value.ToString();

        if (_headerLabel != null)
            _headerLabel.Text = IsMax ? "Max" : "Min";
        
        if (_upButton != null)
        {
            _upButton.Icon = IsMax ? _steamDeckR1 : _steamDeckL1;
        }

        if (_downButton != null)
        {
            _downButton.Icon = IsMax ? _steamDeckR2 : _steamDeckL2;
        }
    }

    public override void _Input(InputEvent ev)
	{
		if (ev is InputEventJoypadButton joypadButton && joypadButton.Pressed)
		{
			HandleJoypadButton(joypadButton);
			return;
		}
		
		if (ev is InputEventJoypadMotion joypadMotion)
		{
			HandleJoypadTriggers(joypadMotion);
		}
	}

    private void HandleJoypadButton(InputEventJoypadButton joypadButton)
	{
		if (joypadButton.ButtonIndex == JoyButton.LeftShoulder)
		{
            MinDelta(1);            
			return;
		}

		if (joypadButton.ButtonIndex == JoyButton.RightShoulder)
		{
            MaxDelta(1);
			return;
		}
	}

	private void HandleJoypadTriggers(InputEventJoypadMotion joypadMotion)
	{
		if (joypadMotion.Axis == JoyAxis.TriggerLeft)
		{
			HandleLeftTrigger(joypadMotion.AxisValue);
			return;
		}

		if (joypadMotion.Axis == JoyAxis.TriggerRight)
		{
			HandleRightTrigger(joypadMotion.AxisValue);
		}
	}

    private const float _triggerDeactivationLevel = 0.05f;
    private const float _triggerActivationLevel = 0.10f;

	private void HandleLeftTrigger(float axisValue)
	{
        var isHighEnoughToActivate = axisValue >= _triggerActivationLevel;
        var isLowEnoughToDeactivate = axisValue <= _triggerDeactivationLevel;

        if (_wasLeftTriggerActive) {
            if (isLowEnoughToDeactivate) {
                _wasLeftTriggerActive = false;
            }

            return;
        }

        if (!isHighEnoughToActivate)
            return;

        _wasLeftTriggerActive = true;
        MinDelta(-1);
	}

	private void HandleRightTrigger(float axisValue)
	{
        var isHighEnoughToActivate = axisValue >= _triggerActivationLevel;
        var isLowEnoughToDeactivate = axisValue <= _triggerDeactivationLevel;

        if (_wasRightTriggerActive) {
            if (isLowEnoughToDeactivate) {
                _wasRightTriggerActive = false;
            }

            return;
        }

        if (!isHighEnoughToActivate)
            return;

        _wasRightTriggerActive = true;
        MaxDelta(-1);
	}
} 