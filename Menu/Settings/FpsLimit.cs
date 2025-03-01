using Godot;
using System;

public partial class FpsLimit : HBoxContainer
{
	public override void _Ready()
	{
		var file=new ConfigFile();
		Error err = file.Load("user://GodotTemplate/Settings/settings.ini");
		if (err == Error.Ok)
		{
			GetNode<OptionButton>("FpsLimit1").Selected=file.GetValue("Settings","fps_limit",8).AsInt32();
		}
		else
		{
			GD.PushError($"Settings file load failed, error code: {(int)err}("+err.ToString()+")");
		}
	}
	public void _on_fps_limit_1_item_selected(int selected)
	{
		GetNode<Game>("/root/Global").max_fps_sel=GetNode<OptionButton>("FpsLimit1").Selected;
		if (selected==0)
		{
			Engine.MaxFps=24;
		}
		if (selected==1)
		{
			Engine.MaxFps=30;
		}
		if (selected==2)
		{
			Engine.MaxFps=60;
		}
		if (selected==3)
		{
			Engine.MaxFps=90;
		}
		if (selected==4)
		{
			Engine.MaxFps=120;
		}
		if (selected==5)
		{
			Engine.MaxFps=144;
		}
		if (selected==6)
		{
			Engine.MaxFps=240;
		}
		if (selected==7)
		{
			Engine.MaxFps=Mathf.RoundToInt(DisplayServer.ScreenGetRefreshRate(DisplayServer.WindowGetCurrentScreen()));
		}
		if (selected==8)
		{
			Engine.MaxFps=0;
		}
	}
}
