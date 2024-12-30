using Godot;

public partial class MinMaxControl : Control
{
    private string _labelText => !IsMax ? "Min" : "Max";

    [Export]
    public bool IsMax { get; set;}

    private int _value = 1;

    private Label _valueLabel;

    public override void _Ready()
    {
        _valueLabel = GetNode<Label>("valueLabel");
        _valueLabel.Text = _value.ToString();

        // Use the exported label text
        GetNode<Label>("headerLabel").Text = _labelText;
    }
} 