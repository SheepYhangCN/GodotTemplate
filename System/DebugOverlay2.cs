using Godot;
using System;

public partial class DebugOverlay2 : Label
{
	bool enabled=false;
	double timer=0;
	/*public override void _Ready()
	{
		Text=Update();
	}*/
	public override void _Process(double delta)
	{
		Game Global=GetNode<Game>("/root/Global");
		enabled = Global.debug_overlay && Global.debug_overlay_r;
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
			if (!Visible)
			{
				Text = Update();
				Visible = true;
			}
			timer+=delta;
			if (timer>=1)
			{
				timer=0;
				Text=Update();
			}
		}
		else
		{
			if (Visible)
			{
				Visible=false;
			}
		}
	}
	internal static string Update()
	{
		var Text="System: "+OS.GetName()+"("+OS.GetDistributionName()+") "+OS.GetVersion()+" "+Engine.GetArchitectureName();
		Text+="\nSystem Locale: "+OS.GetLocale()+" ("+TranslationServer.GetLocaleName(OS.GetLocale())+")";
		Text+="\nDevice ID: "+((OS.GetUniqueId()==null || OS.GetUniqueId()=="") ? "N/A" : OS.GetUniqueId());
		Text+="\nModel Name: "+OS.GetModelName();
		Text+="\nCPU: "+((OS.GetProcessorName()==null || OS.GetProcessorName()=="") ? "N/A" : OS.GetProcessorName());
		Text+="\nCPU Processors Count: "+OS.GetProcessorCount().ToString();
		Text+="\nVideo Adapter: "+((RenderingServer.GetVideoAdapterName()==null || RenderingServer.GetVideoAdapterName()=="")? "N/A" : "("+RenderingServer.GetVideoAdapterVendor()+") "+RenderingServer.GetVideoAdapterName()+" "+RenderingServer.GetVideoAdapterApiVersion());
		Text+="\nDisplayServer Name: "+DisplayServer.GetName();
		Text+="\nRendering Method: "+RenderingServer.GetCurrentRenderingMethod();
		Text+="\nRendering Driver: "+RenderingServer.GetCurrentRenderingDriverName();
		Text+="\nScreen Size,Usable Rect: "+DisplayServer.ScreenGetSize(DisplayServer.WindowGetCurrentScreen()).ToString()+", {"+DisplayServer.ScreenGetUsableRect(DisplayServer.WindowGetCurrentScreen()).ToString()+"}";
		var layout=DisplayServer.KeyboardGetCurrentLayout();
		Text+="\nKeyboard Layout: "+DisplayServer.KeyboardGetLayoutName(layout)+" ("+DisplayServer.KeyboardGetLayoutLanguage(layout)+")";
		Text+="\nScreen DPI: "+((DisplayServer.ScreenGetDpi()==72) ? "N/A" : DisplayServer.ScreenGetDpi(DisplayServer.WindowGetCurrentScreen()));
		Text+="\nGame Locale: "+TranslationServer.GetLocale()+" ("+TranslationServer.GetLocaleName(TranslationServer.GetLocale())+")";
		//Text+="\nAudio Language: "+Global.lang_audio.ToString();
		Text+="\nEditor Locale: "+TranslationServer.GetToolLocale()+" ("+TranslationServer.GetLocaleName(TranslationServer.GetToolLocale())+")";
		Text+="\nProcess ID: "+OS.GetProcessId().ToString();
		Text+="\nMain Thread ID: "+OS.GetMainThreadId().ToString();
		Text+="\nCurrent Thread ID: "+OS.GetThreadCallerId().ToString();
		Text+="\nUser Data Directory: \""+OS.GetUserDataDir()+"\"";
		Text+="\nExecutable Path: \""+OS.GetExecutablePath()+"\"";
		//current rendering driver todo
		var joypads=Input.GetConnectedJoypads();
		Text+="\nConnected Joypads: ";
		for (var a=0;a<joypads.Count;a+=1)
		{
			Text+="{"+joypads[a].ToString()+",\""+Input.GetJoyName(joypads[a])+"\"},";
		}
		Text=Text.TrimSuffix(",");
		if (Text.EndsWith(": "))
		{
			Text+="{}";
		}
		var list=AudioServer.GetInputDeviceList();
		Text+="\nAudio Input Devices: {\"";
		for (var a=0;a<list.Length;a+=1)
		{
			Text+=list[a]+"\",\"";
		}
		Text=Text.TrimSuffix(",\"")+"}";
		if (Text.EndsWith("{\"}"))
		{
			Text=Text.TrimSuffix("\"}")+"}";
		}
		list=AudioServer.GetOutputDeviceList();
		Text+="\nAudio Output Devices: {\"";
		for (var a=0;a<list.Length;a+=1)
		{
			Text+=list[a]+"\",\"";
		}
		Text=Text.TrimSuffix(",\"")+"}";
		if (Text.EndsWith("{\"}"))
		{
			Text=Text.TrimSuffix("\"}")+"}";
		}
		list=OS.GetCmdlineArgs();
		Text+="\nCmdline Arguments: {\"";
		for (var a=0;a<list.Length;a+=1)
		{
			Text+=list[a]+"\",\"";
		}
		Text=Text.TrimSuffix(",\"")+"}";
		if (Text.EndsWith("{\"}"))
		{
			Text=Text.TrimSuffix("\"}")+"}";
		}
		list=OS.GetCmdlineUserArgs();
		Text+="\nCmdline User Arguments: {\"";
		for (var a=0;a<list.Length;a+=1)
		{
			Text+=list[a]+"\",\"";
		}
		Text=Text.TrimSuffix(",\"")+"}";
		if (Text.EndsWith("{\"}"))
		{
			Text=Text.TrimSuffix("\"}")+"}";
		}
		return Text;
	}
}
