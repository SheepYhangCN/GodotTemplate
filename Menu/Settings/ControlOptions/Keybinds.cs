using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Keybinds : Control
{
	Array<Actions> instances=new Array<Actions>();
	public override void _Ready()
	{
		GetNode<Control>("PanelContainer/MarginContainer/VBoxContainer/ScrollContainer/List/Actions").QueueFree();
		CreateActionList();
		SaveKeybindsToTemp();
	}

	public override void _Process(double delta)
	{
		var reset_confirm=GetNode<Control>("ResetConfirm");
		//重置按键
		if(GetNode<Button>("Button/Reset").ButtonPressed)
		{
			reset_confirm.Visible=true;
		}
		//确认重置按键
		if(GetNode<Button>("ResetConfirm/Button/Yes").ButtonPressed)
		{
			InputMap.LoadFromProjectSettings();
			CreateActionList();
			SaveKeybindsToTemp();
			reset_confirm.Visible=false;
		}
		//取消重置按键
		if(GetNode<Button>("ResetConfirm/Button/No").ButtonPressed)
		{
			reset_confirm.Visible=false;
		}
		//保存并返回
		if ((GetNode<Button>("Button/Back").ButtonPressed||Input.IsActionJustPressed("pause"))&&(GetTree().CurrentScene.SceneFilePath=="res://Menu/Settings/ControlOptions/ControlOptions.tscn"||Visible))
		{
			if (FileAccess.FileExists("user://GodotTemplate/Settings/keybinds_temp.ini"))
			{
				DirAccess.RemoveAbsolute("user://GodotTemplate/Settings/keybinds_temp.ini");
			}
			var file=new ConfigFile();
			for (var key=0;key<Game.actions.Count;key+=1)
			{
				file.SetValue("Keybinds",Game.actions.Keys.ToArray()[key],InputMap.ActionGetEvents(Game.actions.Keys.ToArray()[key]));
			}
			Error err=file.Save("user://GodotTemplate/Settings/keybinds.ini");
			if (err != Error.Ok)
			{
				GD.PushError($"Settings(Keybinds) file save failed, error code: {(int)err}("+err.ToString()+")");
			}
			Visible=false;
			GetNode<ControlOptions>("../").current=ControlOptions.SCREEN.CONTROL;
			GetNode<Control>("../ControlOption").Visible=true;
		}
	}
	//从文件创建列表
	internal void CreateActionList()
	{
		var action_list=GetNode<VBoxContainer>("PanelContainer/MarginContainer/VBoxContainer/ScrollContainer/List");
		if (!FileAccess.FileExists("user://GodotTemplate/Settings/keybinds.ini"))
		{
			InputMap.LoadFromProjectSettings();
		}
		for (var inst=0;inst<instances.Count;inst+=1)
		{
			instances[inst].QueueFree();
		}
		instances=new Array<Actions>();
		for (var i=0;i<Game.actions.Count;i+=1)
		{
			var scene=GD.Load<PackedScene>("res://Menu/Settings/ControlOptions/Actions.tscn");
			var button=scene.Instantiate<Actions>();
			instances.Add(button);
			button.GetNode<Label>("MarginContainer/VBoxContainer/List/HBoxContainer/Name").Text=Game.actions.Values.ToArray()[i];
			button.num=i;
			action_list.AddChild(button);
		}
	}
	//从缓存创建列表
	internal void CreateActionListFromTemp()
	{
		var fk=new ConfigFile();
		Error erk=fk.Load("user://GodotTemplate/Settings/keybinds_temp.ini");
		if (erk == Error.Ok)
		{
			for (var action=0;action<Game.actions.Count;action+=1)
			{
				InputMap.ActionEraseEvents(Game.actions.Keys.ToArray()[action]);
				var array=(Array<InputEvent>)fk.GetValue("Keybinds",Game.actions.Keys.ToArray()[action],new Array<InputEvent>());
				for (var key=0;key<array.Count;key+=1)
				{
					InputMap.ActionAddEvent(Game.actions.Keys.ToArray()[action],array[key]);
				}
			}
		}
		else
		{
			GD.PushError($"Settings(KeybindsTemp) file load failed, error code: {(int)erk}("+erk.ToString()+")");
		}
		CreateActionList();
	}
	//从缓存保存按键到文件
	internal static void SaveKeybindsToTemp()
	{
		var file=new ConfigFile();
		for (var key=0;key<Game.actions.Count;key+=1)
		{
			file.SetValue("Keybinds",Game.actions.Keys.ToArray()[key],InputMap.ActionGetEvents(Game.actions.Keys.ToArray()[key]));
		}
		Error err=file.Save("user://GodotTemplate/Settings/keybinds_temp.ini");
		if (err != Error.Ok)
		{
			GD.PushError($"Settings(KeybindsTemp) file save failed, error code: {(int)err}("+err.ToString()+")");
		}
	}
}
