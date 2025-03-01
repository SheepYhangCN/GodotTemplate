using Godot;
using System;
using System.Linq;
using System.Collections;

public partial class InitLang : Control
{
	bool fading=false;
	double timer=0;
	string[] locales;
	public override void _Ready()
	{
		var node=GetNode<OptionButton>("Language");
		locales=TranslationServer.GetLoadedLocales();
		node.ItemCount=locales.Length;
		foreach (var current in locales)
		{
			node.Set("popup/item_"+Array.IndexOf(locales,current).ToString()+"/text",TranslationServer.GetTranslationObject(current).GetMessage("locLanguageName"));
		}
		node.Selected=Array.IndexOf(locales.ToArray(),locales.Contains(TranslationServer.GetLocale()) ? TranslationServer.GetLocale() : TranslationServer.GetLocale().Left(2));
	}

	public override void _Process(double delta)
	{
		Game Global=GetNode<Game>("/root/Global");
		if (fading)
		{
			timer+=delta;
			Global.fader_color=new Color(0,0,0,(float)timer);
		}
		if (timer>=1)
		{
			Global.fader_color.A=0;
			GetTree().ChangeSceneToFile("res://Menu/MainMenu/MainMenu.tscn");
		}
		if (GetNode<Button>("Button").ButtonPressed)
		{
			GetNode<Button>("Button").Disabled=true;
			fading=true;
			Game.UpdateConfigFile("user://GodotTemplate/Settings/settings.ini","Settings","language_display",TranslationServer.GetLocale());
			//Game.UpdateConfigFile("user://GodotTemplate/Settings/settings.ini","Settings","language_audio",(int)Global.lang_audio);
		}
	}
	public void _on_language_item_selected(int selected)
	{
		if (OS.IsStdOutVerbose())
		{
			GD.Print($"[{Time.GetDatetimeStringFromSystem(false,true)}] Language changed from {TranslationServer.GetLocale()} to {locales[selected]}.");
		}
		TranslationServer.SetLocale(locales[selected]);
	}
	public void _on_language_a_item_selected(int selected)
	{

	}
}
