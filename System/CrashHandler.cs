using Godot;
using System;

public partial class CrashHandler : Control
{
	public override void _Ready()
	{
		GetNode<Label>("MarginContainer/VBoxContainer/MarginContainer/ScrollContainer/CrashInfo").Text = Game.crash_info;
		GD.PrintErr($"[{Time.GetDatetimeStringFromSystem(false,true)}] CrashHandler: Game crashed, info: {Game.crash_info}");
	}

	public void _on_saves_pressed()
	{
		Game Global=GetNode<Game>("/root/Global");
		Error err;
		err = OS.ShellOpen(OS.GetUserDataDir()+"/GodotTemplate/Saves");
		if (err == Error.Ok)
		{
			Global.CreatePopUpMessage(TranslationServer.Translate("locMessageFolder").ToString().Replace("{Directory}",OS.GetUserDataDir()+"/GSKAOI/Saves"));
		}
		else
		{
			Global.CreatePopUpMessage(TranslationServer.Translate("locMessageFolderFailed").ToString().Replace("{Directory}",OS.GetUserDataDir()+"/GSKAOI/Saves")+((int)err).ToString()+"("+err.ToString()+")",Message.TYPE.ERROR);
			GD.PushError($"Saves' folder open failed, error code: {(int)err}("+err.ToString()+")");
		}
	}
	public void _on_logs_pressed()
	{
		Game Global = GetNode<Game>("/root/Global");
		//var err = OS.ShellOpen(OS.GetUserDataDir()+"/logs");
		var err = OS.ShellShowInFileManager(OS.GetUserDataDir()+"/logs/godot.log");
		if (err == Error.Ok)
		{
			Global.CreatePopUpMessage(TranslationServer.Translate("locMessageFolder").ToString().Replace("{Directory}",OS.GetUserDataDir()+"/logs"));
		}
		else
		{
			Global.CreatePopUpMessage(TranslationServer.Translate("locMessageFolderFailed").ToString().Replace("{Directory}",OS.GetUserDataDir()+"/logs")+((int)err).ToString()+"("+err.ToString()+")",Message.TYPE.ERROR);
			GD.PushError($"Logs' folder open failed, error code: {(int)err}("+err.ToString()+")");
		}
	}
	public void _on_github_pressed()
	{
		OS.ShellOpen("https://github.com/SheepYhangCN/GodotTemplate");
	}
	public void _on_quit_pressed()
	{
		GetTree().Quit();
	}
	public void _on_copy_pressed()
	{
		DisplayServer.ClipboardSet(GetNode<Label>("MarginContainer/VBoxContainer/MarginContainer/ScrollContainer/CrashInfo").Text);
	}
}
