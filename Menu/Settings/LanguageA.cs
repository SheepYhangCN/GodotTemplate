using Godot;
using System;

public partial class LanguageA : HBoxContainer
{
	public override void _Ready()
	{
		var node = GetNode<OptionButton>("Language1");
		node.ItemCount = (int)Game.LANG_AUDIO.MAX;
		for (var i = 0;i < (int)Game.LANG_AUDIO.MAX;i += 1)
		{
			node.Set("popup/item_"+i.ToString()+"/text",Game.lang_audio_name[(Game.LANG_AUDIO)i]);
		}
		node.Selected=(int)Singleton.Game.lang_audio;
	}
	public void _on_language_1_item_selected(int selected)
	{
		Singleton.Game.lang_audio=(Game.LANG_AUDIO)selected;
	}
}
