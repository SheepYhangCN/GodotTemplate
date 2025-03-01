using Godot;
using System;
using System.Linq;

public partial class Language : HBoxContainer
{
	string[] locales;
	public override void _Ready()
	{
		var node=GetNode<OptionButton>("Language1");
		locales=TranslationServer.GetLoadedLocales();
		node.ItemCount=locales.Length;
		foreach (var current in locales)
		{
			node.Set("popup/item_"+Array.IndexOf(locales,current).ToString()+"/text",TranslationServer.GetTranslationObject(current).GetMessage("locLanguageName"));
		}
		node.Selected=Array.IndexOf(locales.ToArray(),locales.Contains(TranslationServer.GetLocale()) ? TranslationServer.GetLocale() : TranslationServer.GetLocale().Left(2));
	}
	public void _on_language_1_item_selected(int selected)
	{
		GetNode<Game>("/root/Global").ClearMessageList();
		if (OS.IsStdOutVerbose())
		{
			GD.Print($"[{Time.GetDatetimeStringFromSystem(false,true)}] Language changed from {TranslationServer.GetLocale()} to {locales[selected]}.");
		}
		TranslationServer.SetLocale(locales[selected]);
		//GetNode<Settings>("../../../").ContainerUpdate();
		GetTree().ReloadCurrentScene();
	}
}
