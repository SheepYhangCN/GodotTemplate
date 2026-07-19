using Godot;
using System;

public partial class DebugOverlay : Label
{
	bool enabled=false;
	double timer=0;
	/*public override void _Ready()
	{
		Text=Update(this,GetTree(),Singleton.Game);
	}*/
	public override void _Process(double delta)
	{
		enabled=Singleton.Game.debug_overlay;
		if (enabled)
		{
			/*var tree=GetTree();
			if (DisplayServer.WindowGetMode()==DisplayServer.WindowMode.Windowed)
			{
				Scale=new Vector2(1280f/DisplayServer.WindowGetSize().X,720f/DisplayServer.WindowGetSize().Y);
			}
			if (DisplayServer.WindowGetMode()==DisplayServer.WindowMode.Fullscreen||DisplayServer.WindowGetMode()==DisplayServer.WindowMode.ExclusiveFullscreen)
			{
				Scale=new Vector2(1280f/DisplayServer.ScreenGetSize().X,720f/DisplayServer.ScreenGetSize().Y);
			}*/
			if (!Visible){Visible = true;}
			timer+=delta;
			/*if (timer>=1)
			{
				timer=0;*/
				Text=Update(this,GetTree());
			//}
		}
		else
		{
			if (Visible)
			{
				Visible=false;
			}
		}
	}
	internal static string Update(DebugOverlay thisi, SceneTree tree)
	{
		string Text;
		Text="Version: v"+Game.version + (Singleton.Game.mod ? " - Modded" : "") + (Singleton.Game.locale ? " - Locale" : "");
		if (tree.CurrentScene == null)
		{
			Text+="\nScene: Null";
		}
		else
		{
			Text+="\nScene: "+tree.CurrentScene.Name+" (\""+tree.CurrentScene.SceneFilePath+"\")";
		}
		Text+="\nFPS: "+Engine.GetFramesPerSecond().ToString()+"("+(1d/Performance.GetMonitor(Performance.Monitor.TimeProcess)).ToString()+")/"+Engine.MaxFps.ToString();
		/*Text+="\nMemory Info: "+OS.GetMemoryInfo().ToString();
		Text+="\nFree Physical Memory Amount: "+((double)OS.GetMemoryInfo()["free"]/1024/1024).ToString()+"/"+((double)OS.GetMemoryInfo()["physical"]/1024/1024).ToString()+" MiB";//+OS.GetMemoryInfo().ToString();
		Text+="\nStatic Memory Used: "+(Performance.GetMonitor(Performance.Monitor.MemoryStatic)/1024/1024).ToString()+" MiB";
		Text+="\nVideo Memory Used: "+(Performance.GetMonitor(Performance.Monitor.RenderVideoMemUsed)/1024/1024).ToString()+" MiB";
		*/Text+="\nNode Count: "+tree.GetNodeCount().ToString();
		Text+="\nObject Count: "+Performance.GetMonitor(Performance.Monitor.ObjectCount).ToString();
		Text+="\nObject Resource Count: "+Performance.GetMonitor(Performance.Monitor.ObjectResourceCount).ToString();
		Text+="\nMouse Position: "+thisi.GetGlobalMousePosition().ToString();
		Text+="\nMouse Mode: "+DisplayServer.MouseGetMode().ToString();
		Text+="\nWindow Mode: "+DisplayServer.WindowGetMode().ToString();
		Text+="\nWindow Current Screen: "+DisplayServer.WindowGetCurrentScreen().ToString();
		Text+="\nWindow Position,Decorations: "+DisplayServer.WindowGetPosition().ToString()+","+DisplayServer.WindowGetPositionWithDecorations().ToString();
		Text+="\nWindow Size,Decorations: "+DisplayServer.WindowGetSize().ToString()+","+DisplayServer.WindowGetSizeWithDecorations().ToString();
		Text+="\nWindow Focused: "+DisplayServer.WindowIsFocused().ToString();
		Text+="\nSingleton.Game.fader_color = "+Singleton.Game.fader_color.ToString();
		Text+="\nSingleton.Game.control_mode = "+((int)Singleton.Game.control_mode).ToString()+"("+Singleton.Game.control_mode.ToString()+")";
		Text+="\nSingleton.Game.input_remapping = "+Singleton.Game.input_remapping.ToString();
		if (Singleton.Game.mod)
		{
			Text+="\nMods Loaded: {\"";
			for (var a=0;a<Singleton.Game.mods_loaded.Length;a+=1)
			{
				Text+=Singleton.Game.mods_loaded[a]+"\",\"";
			}
			Text=Text.TrimSuffix(",\"")+"}";
			if (Text.EndsWith("{\"}"))
			{
				Text=Text.TrimSuffix("\"}")+"}";
			}
			Text+="\nLocalization files Loaded: {\"";
			for (var a=0;a<Singleton.Game.locale_loaded.Length;a+=1)
		{
				Text+=Singleton.Game.locale_loaded[a]+"\",\"";
			}
			Text=Text.TrimSuffix(",\"")+"}";
			if (Text.EndsWith("{\"}"))
			{
				Text=Text.TrimSuffix("\"}")+"}";
			}
		}
		return Text;
	}
}
