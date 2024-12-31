using Godot;
using System;

public partial class Node2d : Node2D
{
	private readonly Random _random = new Random();

	private Button _generateButton;
	private Label _resultLabel;
	private Label _debugLabel;

	private MinMaxControl _minControl;
	private MinMaxControl _maxControl;

	private const int minConstraint = 0;
	private const int maxConstraint = 100;
	private const int minDefault = 1;
	private const int maxDefault = 10;

	public override void _Ready()
	{
		_resultLabel = GetNode<Label>("resultLabel");

		_generateButton = GetNode<Button>("generateButton");
		_generateButton.Connect("pressed", Callable.From(() => OnGenerateRandomClick()));
		_generateButton.GrabFocus();

		_debugLabel = GetNode<Label>("debugLabel");

		_minControl = GetNode<MinMaxControl>("minControl");
		_minControl.ValueChanged += MinDelta;
		_minControl.Value = minDefault;

		_maxControl = GetNode<MinMaxControl>("maxControl");
		_maxControl.ValueChanged += MaxDelta;
		_maxControl.Value = maxDefault;
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
		var randomValue = GetRandom();
		_resultLabel.Text = randomValue.ToString();
	}


	public override void _ExitTree()
	{
		if (_minControl != null)
			_minControl.ValueChanged -= MinDelta;

		if (_maxControl != null)
			_maxControl.ValueChanged -= MaxDelta;

		base._ExitTree();
	}
}
