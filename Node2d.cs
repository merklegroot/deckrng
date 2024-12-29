using Godot;
using System;

public partial class Node2d : Node2D
{
	private readonly Random _random = new Random();

	private TextEdit _minEdit;
	private TextEdit _maxEdit;
	private Button _minUpButton;
	private Button _minDownButton;
	private Button _maxUpButton;
	private Button _maxDownButton;
	private Button _generateButton;
	private Label _resultLabel;

	private const int minConstraint = 0;
	private const int maxConstraint = 100;
	private const int minDefault = 1;
	private const int maxDefault = 10;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_minEdit = GetNode<TextEdit>("minEdit");
		_minEdit.Text = minDefault.ToString();

		_maxEdit = GetNode<TextEdit>("maxEdit");
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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void MinDelta(int difference)
	{
		var updatedValue = Constrain(GetMinValue() + difference, minConstraint, maxConstraint);

		_minEdit.Text = updatedValue.ToString();
	}

	private void MaxDelta(int difference)
	{
		var updatedValue = Constrain(GetMaxValue() + difference, minConstraint, maxConstraint);
		_maxEdit.Text = updatedValue.ToString();
	}

	private static int Constrain(int source, int min, int max) =>
		source < min ? min
		: source > max ? max
		: source;

	private int GetMinValue()
	{
		if (!int.TryParse(_minEdit.Text, out var parsedValue))
			return minConstraint;

		if (parsedValue < minConstraint)
			return minConstraint;

		return parsedValue;
	}

	private int GetMaxValue()
	{
		if (!int.TryParse(_maxEdit.Text, out var parsedValue))
			return 0;

		if (parsedValue < 0)
			return 0;

		return parsedValue;
	}

	private int GetRandom(){

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
}
