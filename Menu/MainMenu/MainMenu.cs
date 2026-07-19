using Godot;
using System;

public partial class MainMenu : Control
{
	double timer=0;
	bool pressed=true;
	public override void _Ready()
	{
		var quit=GetNode<Button>("Control/Quit");
		var settings=GetNode<Button>("Control/Settings");
		var version=GetNode<Label>("Control/Version");
		quit.ResetSize();
		settings.ResetSize();
		version.ResetSize();
		var t="";
		if (Singleton.Game.mod)
		{
			t+=TranslationServer.Translate("locInfoMod").ToString().Replace("{ModsLoaded}",Singleton.Game.mods_loaded.Length.ToString());
		}
		if (Singleton.Game.locale)
		{
			t+=TranslationServer.Translate("locLocInfo").ToString().Replace("{LocalesLoaded}",Singleton.Game.locale_loaded.Length.ToString());
		}
		if (Singleton.Game.mods_loaded.Length > 1)
		{
			t=t.Replace("{ModS}","mods");
		}
		else
		{
			t=t.Replace("{ModS}","mod");
		}
		if (Singleton.Game.locale_loaded.Length > 1)
		{
			t=t.Replace("{fileS}","files");
		}
		else
		{
			t=t.Replace("{fileS}","file");
		}
		version.Text=t+"v"+Game.version+(Singleton.Game.mod ? " - "+TranslationServer.Translate("locEnabledMod") : "");
	}

	public override void _Process(double delta)
	{
		var tree=GetTree();
		var quit=GetNode<Button>("Control/Quit");
		var settings=GetNode<Button>("Control/Settings");
		if (pressed)
		{
			timer+=delta;
			if (settings.ButtonPressed)
			{
				tree.ChangeSceneToFile("res://Menu/Settings/Settings.tscn");
			}
			if (quit.ButtonPressed)
			{
				tree.Quit();
			}
		}
	}
}
