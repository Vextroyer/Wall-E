using Godot;
using System;
//using Frontend;

public partial class Clear : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void _pressed()
    {
        var nodocontrol = GetNode<NodoPadre>("..");
        nodocontrol.clear();
    }
}
