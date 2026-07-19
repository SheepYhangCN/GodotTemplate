using Godot;
using GodotTask;
using System;

public partial class Message : Control
{
	internal enum TYPE//消息类型
	{
		MESSAGE,//消息
		WARNING,//警告
		ERROR//错误
	}
	bool fade_out=false;
	double timer=0;
	internal double shown_time=5;
	internal TYPE type=TYPE.MESSAGE;
	internal string custom_method_name=null;
	public override async void _Ready()
	{
		await GDTask.Delay(Mathf.RoundToInt(shown_time * 1000));//ToSignal(GetTree().CreateTimer(shown_time), "timeout");
		fade_out=true;
	}

	public override void _Process(double delta)
	{
		//panel color
		var color=new Color(1,1,1,0.25f);
		var bcolor=new Color(1,1,1,0.75f);
		var bbcolor=new Color(0,0,0,0.25f);
		var bbbcolor=new Color(0.5f,0.5f,0.5f,0.75f);
		if (type==TYPE.WARNING)
		{
			color=new Color(1,1,0,0.25f);
			bcolor=new Color(1,1,0,0.75f);
			bbcolor=color;
			bbbcolor=bcolor;
		}
		if (type==TYPE.ERROR)
		{
			color=new Color(1,0,0,0.25f);
			bcolor=new Color(1,0,0,0.75f);
			bbcolor=color;
			bbbcolor=bcolor;
		}
		var theme=GetNode<Panel>("Panel").Theme;
		var sbtype=theme.GetStyleboxTypeList()[0];
		var stylebox=(StyleBoxFlat)theme.GetStylebox(theme.GetStyleboxList(sbtype)[0],sbtype);
		stylebox.BgColor=color;
		stylebox.BorderColor=bcolor;
		theme.SetStylebox(theme.GetStyleboxList(sbtype)[0],sbtype,stylebox);
		//button color
		theme=GetNode<Control>("Button").Theme;
		sbtype=theme.GetStyleboxTypeList()[0];
		stylebox=(StyleBoxFlat)theme.GetStylebox(theme.GetStyleboxList(sbtype)[0],sbtype);
		stylebox.BgColor=bbcolor;
		stylebox.BorderColor=bbbcolor;
		theme.SetStylebox(theme.GetStyleboxList(sbtype)[0],sbtype,stylebox);
		
		shown_time -= delta;
		shown_time = Math.Max(0, shown_time);
		if (fade_out)
		{
			GetNode<Button>("Button/Close").Text="locClose";
			timer+=delta;
			Modulate=new Color(1,1,1,(float)(1-timer));
		}
		else
		{
			GetNode<Button>("Button/Close").Text=TranslationServer.Translate("locClose")+"("+Math.Round(shown_time).ToString()+"s)";
		}
		if (Modulate.A<=0)
		{
			QueueFree();
		}
	}
	public void _on_copy_pressed()
	{
		DisplayServer.ClipboardSet(GetNode<RichTextLabel>("PanelContainer/MarginContainer/Text").Text);
		Singleton.Game.CreatePopUpMessage("locMessageInfoCopied",TYPE.MESSAGE,false);
	}
	public void _on_close_pressed()
	{
		fade_out=true;
		GetNode<Button>("Button/Close").Disabled=true;
	}
	public void _on_custom_pressed()
	{
		if (GetNode<Button>("Button/Custom").Visible)
		{
			switch (custom_method_name)
			{
				default:
					GD.PushError("Error: Invalid custom_method_name: "+custom_method_name);
					break;
				case null:
					GD.PushError("Error: Null custom_method_name with CustomButton enabled");
					break;
				case "":
					GD.PushError("Error: Empty custom_method_name with CustomButton enabled");
					break;
			}
		}
	}
}
