using Godot;
using System;

public partial class MainScene : Node2D
{
	private readonly Random _random = new Random();

	private Button _generateButton;
	private Label _resultLabel;

	private MinMaxControl _minControl;
	private MinMaxControl _maxControl;

	private const int minConstraint = 0;
	private const int maxConstraint = 100;
	private const int minDefault = 1;
	private const int maxDefault = 10;

	private Button _closeButton;

	private PackedScene _fireAnimationScene;

	public override void _Ready()
	{
		_resultLabel = GetNode<Label>("resultLabel");

		_generateButton = GetNode<Button>("generateButton");
		_generateButton.Connect("pressed", Callable.From(() => OnGenerateRandomClick()));
		_generateButton.GrabFocus();

		_minControl = GetNode<MinMaxControl>("minControl");
		_minControl.ValueChanged += MinDelta;
		_minControl.Value = minDefault;

		_maxControl = GetNode<MinMaxControl>("maxControl");
		_maxControl.ValueChanged += MaxDelta;
		_maxControl.Value = maxDefault;

		var buildLabel = GetNode<Label>("buildLabel");
		buildLabel.Text = $"Build: {BuildInfo.BuildTime}";

		_closeButton = GetNode<Button>("closeButton");
		_closeButton.Pressed += OnCloseButtonPressed;

		_fireAnimationScene = GD.Load<PackedScene>("res://fire_animation.tscn");
	}

	public override void _Process(double delta)
	{
	}

	private void MinDelta(int difference)
	{
		var updatedValue = Constrain(_minControl.Value + difference, minConstraint, maxConstraint);
		_minControl.Value = updatedValue;

		if (_maxControl.Value < updatedValue)
			_maxControl.Value = updatedValue;
	}

	private void MaxDelta(int difference)
	{
		var updatedValue = Constrain(_maxControl.Value + difference, minConstraint, maxConstraint);
		if (updatedValue != _maxControl.Value)
			_maxControl.Value = updatedValue;

		if (_minControl.Value > updatedValue)
			_minControl.Value = updatedValue;
	}

	private static int Constrain(int source, int min, int max) =>
		source < min ? min
		: source > max ? max
		: source;

	private int GetRandom()
	{
		var range = _maxControl.Value - _minControl.Value + 1;

		if (range < 0)
		{
			return _minControl.Value;
		}

		var randomValue = (_random.Next() % range) + _minControl.Value;

		return randomValue;
	}

	private void OnGenerateRandomClick()
	{
		const int totalExplosions = 10;

		var randomValue = GetRandom();
		_resultLabel.Text = randomValue.ToString();
		
		for (int i = 0; i < totalExplosions; i++)
		{
			var fireSprite = _fireAnimationScene.Instantiate<AnimatedSprite2D>();
			AddChild(fireSprite);
			
			var randomOffset = new Vector2(
				_random.Next(-50, 50),
				_random.Next(-50, 50)
			);

			fireSprite.Position = new Vector2(588, 178) + randomOffset;
			fireSprite.SpeedScale = 6.0f;
			
			fireSprite.AnimationFinished += () => 
			{
				fireSprite.QueueFree();
			};
			
			fireSprite.Play();
		}
	}

	private void OnCloseButtonPressed()
	{
		GetTree().Quit();
	}

	public override void _Input(InputEvent ev)
	{
		if (ev is not InputEventJoypadButton joypadButton || !joypadButton.Pressed)
			return;

		if (joypadButton.ButtonIndex == JoyButton.A)
		{
			OnGenerateRandomClick();
			return;
		}

		if (joypadButton.ButtonIndex == JoyButton.B)
		{
			OnCloseButtonPressed();
			return;
		}
	}

	public override void _ExitTree()
	{
		if (_minControl != null)
			_minControl.ValueChanged -= MinDelta;

		if (_maxControl != null)
			_maxControl.ValueChanged -= MaxDelta;

		if (_closeButton != null)
		{
			_closeButton.Pressed -= OnCloseButtonPressed;
		}

		base._ExitTree();
	}
}
