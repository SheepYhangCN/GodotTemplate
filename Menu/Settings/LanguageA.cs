using Godot;
using System;

public partial class LanguageA : HBoxContainer
{
	public override void _Ready()
	{
		//GetNode<OptionButton>("Language1").Selected=(int)GetNode<Game>("/root/Global").lang_audio;
	}
	public void _on_language_1_item_selected(int selected)
	{
		//GetNode<Game>("/root/Global").lang_audio=(Game.LANG_AUDIO)selected;
	}
}
