using Godot;
using System;

public partial class Node2d : Node2D
{
	private readonly Random _random = new Random();

	private Label _minEdit;
	private Label _maxEdit;
	private Button _minUpButton;
	private Button _minDownButton;
	private Button _maxUpButton;
	private Button _maxDownButton;
	private Button _generateButton;
	private Label _resultLabel;
	private Label _debugLabel;

	private const int minConstraint = 0;
	private const int maxConstraint = 100;
	private const int minDefault = 1;
	private const int maxDefault = 10;

	private bool _isLeftTriggerActive = false;
	private bool _isRightTriggerActive = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_minEdit = GetNode<Label>("minEdit");
		_minEdit.Text = minDefault.ToString();

		_maxEdit = GetNode<Label>("maxEdit");
		_maxEdit.Text = maxDefault.ToString();

		_minUpButton = GetNode<Button>("minUpButton");
		_minUpButton.Connect("pressed", Callable.From(() => MinDelta(1)));

		_minDownButton = GetNode<Button>("minDownButton");
		_minDownButton.Connect("pressed", Callable.From(() => MinDelta(-1)));

		_maxUpButton = GetNode<Button>("maxUpButton");
		_maxUpButton.Connect("pressed", Callable.From(() => MaxDelta(1)));

		_maxDownButton = GetNode<Button>("maxDownButton");
		_maxDownButton.Connect("pressed", Callable.From(() => MaxDelta(-1)));

		_resultLabel = GetNode<Label>("resultLabel");

		_generateButton = GetNode<Button>("generateButton");
		_generateButton.Connect("pressed", Callable.From(() => OnGenerateRandomClick()));
		_generateButton.GrabFocus();

		_debugLabel = GetNode<Label>("debugLabel");
	}

	public override void _Process(double delta)
	{
	}

	private void MinDelta(int difference)
	{
		var updatedValue = Constrain(GetMinValue() + difference, minConstraint, maxConstraint);
		_minEdit.Text = updatedValue.ToString();

		if(GetMaxValue() < updatedValue)
		{
			_maxEdit.Text = updatedValue.ToString();
		}
	}

	private void MaxDelta(int difference)
	{
		var updatedValue = Constrain(GetMaxValue() + difference, minConstraint, maxConstraint);
		_maxEdit.Text = updatedValue.ToString();

		if(GetMinValue() > updatedValue)
		{
			_minEdit.Text = updatedValue.ToString();
		}
	}

	private static int Constrain(int source, int min, int max) =>
		source < min ? min
		: source > max ? max
		: source;

	private int GetMinValue() => 
		int.TryParse(_minEdit.Text, out var value) && value >= minConstraint 
			? value 
			: minConstraint;

	private int GetMaxValue() => 
		int.TryParse(_maxEdit.Text, out var value) && value >= 0 
			? value 
			: 0;

	private int GetRandom()
	{
		var minValue = GetMinValue();
		var range = GetMaxValue() - minValue + 1;

		if (range < 0)
		{
			return minValue;
		}

		var randomValue = (_random.Next() % range) + minValue;

		return randomValue;
	}

	private void OnGenerateRandomClick()
	{
		var randomValue = GetRandom();
		_resultLabel.Text = randomValue.ToString();
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
		_debugLabel.Text = $"Button: {joypadButton.ButtonIndex}";
		GD.Print($"Pressed button: {joypadButton.ButtonIndex}");

		if (joypadButton.ButtonIndex == JoyButton.A)
		{
			OnGenerateRandomClick();
			return;
		}

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

	private void HandleLeftTrigger(float axisValue)
	{
		if (axisValue > 0.5f && !_isLeftTriggerActive)
		{
			_isLeftTriggerActive = true;
			MinDelta(-1);
			_debugLabel.Text = $"L2 Trigger: {axisValue:F2}";
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
			MaxDelta(-1);
			_debugLabel.Text = $"R2 Trigger: {axisValue:F2}";
			return;
		}

		if (axisValue <= 0.5f)
		{
			_isRightTriggerActive = false;
		}
	}
}
