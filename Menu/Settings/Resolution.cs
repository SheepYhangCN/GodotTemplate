using Godot;
using System;

public partial class Resolution : HBoxContainer
{
	public override void _Ready()
	{
		Update(GetNode<OptionButton>("Resolution1"));
		var file=new ConfigFile();
		Error load = file.Load("user://GodotTemplate/Settings/settings.ini");
		if (load == Error.Ok)
		{
			GetNode<OptionButton>("Resolution1").Selected=file.GetValue("Settings","resolution",2).AsInt32();
		}
		else
		{
			GD.PushError($"Settings file load failed, error code: {(int)load}("+load.ToString()+")");
		}
	}

	public override void _Process(double delta)
	{
		Update(GetNode<OptionButton>("Resolution1"));
	}
	public void _on_resolution_1_item_selected(int selected)
	{
		if (selected==0)
		{
			DisplayServer.WindowSetSize(new Vector2I(640,360));
			GetWindow().MoveToCenter();
		}
		if (selected==1)
		{
			DisplayServer.WindowSetSize(new Vector2I(960,540));
			GetWindow().MoveToCenter();
		}
		if (selected==2)
		{
			DisplayServer.WindowSetSize(new Vector2I(1280,720));
			GetWindow().MoveToCenter();
		}
		if (selected==3)
		{
			DisplayServer.WindowSetSize(new Vector2I(1600,900));
			GetWindow().MoveToCenter();
		}
		if (selected==4)
		{
			DisplayServer.WindowSetSize(new Vector2I(1920,1080));
			GetWindow().MoveToCenter();
		}
		if (selected==5)
		{
			DisplayServer.WindowSetSize(new Vector2I(2560,1440));
			GetWindow().MoveToCenter();
		}
		if (selected==6)
		{
			DisplayServer.WindowSetSize(new Vector2I(3200,1800));
			GetWindow().MoveToCenter();
		}
		if (selected==7)
		{
			DisplayServer.WindowSetSize(new Vector2I(3840,2160));
			GetWindow().MoveToCenter();
		}
	}
	private static void Update(OptionButton option)
	{
		/*if (DisplayServer.WindowGetMode()==DisplayServer.WindowMode.Windowed && ((OS.GetName()=="Windows") || (OS.GetName()=="macOS") || (OS.GetName()=="Linux") || (OS.GetName()=="BSD")) && !Engine.IsEmbeddedInEditor())
		{
			option.Disabled=false;
			option.Set("popup/item_2/text","1280x720");
		}
		else
		{
			option.Disabled=true;
			option.Selected = -1;
			option.Set("popup/item_2/text",DisplayServer.ScreenGetSize().X.ToString()+"x"+DisplayServer.ScreenGetSize().Y.ToString());
		}*/
	}
}
