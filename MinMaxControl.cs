using Godot;

public partial class MinMaxControl : Control
{
	[Signal]
	public delegate void ValueChangedEventHandler(int delta);

	private bool _isLeftTriggerActive = false;
	private bool _isRightTriggerActive = false;

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
                // EmitSignal(SignalName.ValueChanged, _value);
            }
        }
    }

    private Label _valueLabel => GetNode<Label>("valueLabel");
    private Label _headerLabel => GetNode<Label>("headerLabel");

    private Button _upButton;
    private Button _downButton;

    public override void _Ready()
    {
        _upButton = GetNode<Button>("upButton");
        _upButton.Pressed += OnUp;

        _downButton = GetNode<Button>("downButton");
        _downButton.Pressed += OnDown;

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
        _valueLabel.Text = Value.ToString();
        _headerLabel.Text = IsMax ? "Max" : "Min";
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
            MinDelta(-1);
            
			return;
		}

		if (joypadButton.ButtonIndex == JoyButton.RightShoulder)
		{
            MaxDelta(-1);

			return;
		}
	}

	private void HandleJoypadTriggers(InputEventJoypadMotion joypadMotion)
	{
		if (joypadMotion.Axis == JoyAxis.TriggerLeft)
		{
			MinDelta(-1);
			return;
		}

		if (joypadMotion.Axis == JoyAxis.TriggerRight)
		{
			MaxDelta(-1);
		}
	}

	private void HandleLeftTrigger(float axisValue)
	{
		if (axisValue > 0.5f && !_isLeftTriggerActive)
		{
			_isLeftTriggerActive = true;
            MinDelta(1);
			return;
		}

		if (axisValue <= 0.5f)
		{
			_isLeftTriggerActive = false;
		}
	}

	private void HandleRightTrigger(float axisValue)
	{
		if (axisValue > 0.5f && !_isRightTriggerActive)
		{
			_isRightTriggerActive = true;
			MaxDelta(1);
			return;
		}

		if (axisValue <= 0.5f)
		{
			_isRightTriggerActive = false;
		}
	}
} 