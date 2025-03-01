using Godot;
using System;

public partial class Volume : HBoxContainer
{
	public override void _Ready()
	{
		GetNode<HSlider>("V/Volume1").Value=(AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Master"))+80)/80*100;
		GetNode<ProgressBar>("V/VolumeBar").Value=GetNode<HSlider>("V/Volume1").Value;
	}

	public void _on_volume_1_value_changed(float value)
	{
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"),(float)(80*(value/100)-80));
		GetNode<ProgressBar>("V/VolumeBar").Value=value;
		GetNode<Label>("Label").Text = value.ToString()+"%";
	}
}
