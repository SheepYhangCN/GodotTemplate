using Godot;
using Godot.Bridge;
using Godot.Collections;
using GodotTask;
using GodotTask.GlobalCancellation;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

public partial class Game : Node2D
{
	internal static readonly string version = ProjectSettings.GetSetting("application/config/version").AsString();//版本号 读取项目设置的版本号 不带开头的v
	private string current_scene_path;//当前的场景目录
	private string screenshot_target_dir = null;//截图的储存路径
	internal int max_fps_sel = 0;//选中的最大fps选项
	public enum LANG_AUDIO//语音语言
	{
		ENGLISH,//英语
		JAPANESE,//日语
		CHS_MANDARIN,//汉语（普通话）
		CHS_CANTONESE,//汉语（粤语）

		MAX//最大值，用于计数
	}
	public static readonly Dictionary<LANG_AUDIO,string> lang_audio_name = new()//语音语言文本
	{
		{LANG_AUDIO.ENGLISH,"locLangEng"},//英语
		{LANG_AUDIO.JAPANESE,"locLangJapan"},//日语
		{LANG_AUDIO.CHS_MANDARIN,"locLangChsMandarin"},//汉语（普通话）
		{LANG_AUDIO.CHS_CANTONESE,"locLangChsCantonese"}//汉语（粤语）
	};
	public static readonly Dictionary<string,Dictionary<LANG_AUDIO,string>> audio_lang_replacement = new()//语音语言替换 {语音,{语言,语音}}
	{

	};
	public enum CONTROL_MODE//控制模式
	{
		KEYBOARD_MOUSE,//键鼠
		CONTROLLER,//手柄
		VIRTURAL_CONTROLLER//虚拟控制器
	}
	internal static readonly Dictionary<string, Texture2D> placeholders = new()
	{
		{"2", GD.Load<Texture2D>("res://Assets/Sprites/Placeholders/MissingNo2.png")},
		{"8", GD.Load<Texture2D>("res://Assets/Sprites/Placeholders/MissingNo8.png")},
		{"16", GD.Load<Texture2D>("res://Assets/Sprites/Placeholders/MissingNo16.png")},
		{"32", GD.Load<Texture2D>("res://Assets/Sprites/Placeholders/MissingNo32.png")},
		{"64", GD.Load<Texture2D>("res://Assets/Sprites/Placeholders/MissingNo64.png")},
		{"double4", GD.Load<Texture2D>("res://Assets/Sprites/Placeholders/MissingNoDouble4.png")},
		{"double16", GD.Load<Texture2D>("res://Assets/Sprites/Placeholders/MissingNoDouble16.png")},
		{"double32", GD.Load<Texture2D>("res://Assets/Sprites/Placeholders/MissingNoDouble32.png")},
		{"double64", GD.Load<Texture2D>("res://Assets/Sprites/Placeholders/MissingNoDouble64.png")},
		{"128", GD.Load<Texture2D>("res://Assets/Sprites/Placeholders/MissingNoDouble128.png")},
		{"fourfold4", GD.Load<Texture2D>("res://Assets/Sprites/Placeholders/MissingNoFourfold8.png")},
		{"fourfold32", GD.Load<Texture2D>("res://Assets/Sprites/Placeholders/MissingNoFourfold32.png")},
		{"fourfold64", GD.Load<Texture2D>("res://Assets/Sprites/Placeholders/MissingNoFourfold64.png")},
		{"fourfold128", GD.Load<Texture2D>("res://Assets/Sprites/Placeholders/MissingNoFourfold128.png")},
		{"256", GD.Load<Texture2D>("res://Assets/Sprites/Placeholders/MissingNoFourfold256.png")}
	};
	internal static readonly Dictionary<string,string> actions = new()//各个按键的名称
	{
		{"screenshot","locKeyScreenshot"},
		{"fullscreen","locKeyFullscreen"}
	};

	internal bool debug = OS.IsDebugBuild() || OS.GetCmdlineUserArgs().Contains("-debug");//debug
	internal bool debug_overlay = false;//debug overlay
	internal bool debug_overlay_r = true;//右侧的debug overlay 一般都是系统讯息等定值
	internal bool mod = OS.GetCmdlineUserArgs().Contains("-mod");//加载mod
	internal bool locale = OS.GetCmdlineUserArgs().Contains("-locales");//加载自定义本地化文件
	internal string[] mods_loaded = [];//加载了的mod 储存文件名 带后缀
	internal string[] locale_loaded = [];//加载了的自定义本地化文件 储存文件名 带后缀

	[Export] internal LANG_AUDIO lang_audio=LANG_AUDIO.ENGLISH;
	internal Color fader_color;//fader颜色 RGBA
	internal CONTROL_MODE control_mode;//控制模式
	internal bool input_remapping = false;//正在设置按键
	internal static string crash_info = "";// crash handler 崩溃讯息

	internal void GameInit(bool first_time=false)
	{
		var tree=GetTree();
		if (OS.IsStdOutVerbose())
		{
			GD.Print("Verbose stdout was enabled.");
		}
		Singleton.Game = this;
		fader_color = new Color(0,0,0,0);
		control_mode = CONTROL_MODE.KEYBOARD_MOUSE;
		mod = OS.GetCmdlineUserArgs().Contains("-mod");
		locale = OS.GetCmdlineUserArgs().Contains("-locales");
		mods_loaded = [];
		locale_loaded = [];
		//初始化语言
		if ((!debug)&&(!FileAccess.FileExists("user://GodotTemplate/Settings/settings.ini")))
		{
			if (OS.GetLocale() == "zh_TW" || OS.GetLocale() == "zh_HK" || OS.GetLocale() == "zh_MO")
			{
				TranslationServer.SetLocale("zh_TW");
			}
			else if (OS.GetLocaleLanguage() == "zh" || OS.GetLocale() == "zh_CN" || OS.GetLocale() == "zh_SG")
			{
				TranslationServer.SetLocale("zh_CN");
			}
			else
			{
				TranslationServer.SetLocale(OS.GetLocale());
			}
		}
		//创建目录
		if (!DirAccess.DirExistsAbsolute("user://GodotTemplate"))
		{
			DirAccess.MakeDirAbsolute("user://GodotTemplate");
		}
		if (!DirAccess.DirExistsAbsolute("user://GodotTemplate/Settings"))
		{
			DirAccess.MakeDirAbsolute("user://GodotTemplate/Settings");
		}
		if (!DirAccess.DirExistsAbsolute("user://GodotTemplate/Saves"))
		{
			DirAccess.MakeDirAbsolute("user://GodotTemplate/Saves");
		}
		if (!DirAccess.DirExistsAbsolute("user://GodotTemplate/Screenshots"))
		{
			DirAccess.MakeDirAbsolute("user://GodotTemplate/Screenshots");
		}
		if (mod && !DirAccess.DirExistsAbsolute("user://GodotTemplate/Mods"))
		{
			DirAccess.MakeDirAbsolute("user://GodotTemplate/Mods");
		}
		//加载设置
		var file=new ConfigFile();
		Error load = file.Load("user://GodotTemplate/Settings/settings.ini");
		if (load == Error.Ok)
		{
			TranslationServer.SetLocale(file.GetValue("Settings","language_display","en").AsString());
			lang_audio=(LANG_AUDIO)file.GetValue("Settings","language_audio",0).AsInt32();
			DisplayServer.WindowSetMode((DisplayServer.WindowMode)file.GetValue("Settings","display_mode",(int)DisplayServer.WindowMode.Fullscreen).AsInt32());
			if (DisplayServer.WindowGetMode()==DisplayServer.WindowMode.Windowed)
			{
				var resolution=(int)file.GetValue("Settings","resolution",2);
				if (resolution==0)
				{
					DisplayServer.WindowSetSize(new Vector2I(640,360));
				}
				if (resolution==1)
				{
					DisplayServer.WindowSetSize(new Vector2I(960,540));
				}
				if (resolution==2)
				{
					DisplayServer.WindowSetSize(new Vector2I(1280,720));
				}
				if (resolution==3)
				{
					DisplayServer.WindowSetSize(new Vector2I(1600,900));
				}
				if (resolution==4)
				{
					DisplayServer.WindowSetSize(new Vector2I(1920,1080));
				}
				if (resolution==5)
				{
					DisplayServer.WindowSetSize(new Vector2I(2560,1440));
				}
				if (resolution==6)
				{
					DisplayServer.WindowSetSize(new Vector2I(3200,1800));
				}
				if (resolution==7)
				{
					DisplayServer.WindowSetSize(new Vector2I(3840,2160));
				}
				GetWindow().MoveToCenter();
			}
			var maxfps=file.GetValue("Settings","fps_limit",8).AsInt32();
			max_fps_sel=maxfps;
			if (maxfps==0)
			{
				Engine.MaxFps=24;
			}
			if (maxfps==1)
			{
				Engine.MaxFps=30;
			}
			if (maxfps==2)
			{
				Engine.MaxFps=60;
			}
			if (maxfps==3)
			{
				Engine.MaxFps=90;
			}
			if (maxfps==4)
			{
				Engine.MaxFps=120;
			}
			if (maxfps==5)
			{
				Engine.MaxFps=144;
			}
			if (maxfps==6)
			{
				Engine.MaxFps=240;
			}
			if (maxfps==7)
			{
				Engine.MaxFps=Mathf.RoundToInt(DisplayServer.ScreenGetRefreshRate(DisplayServer.WindowGetCurrentScreen()));
			}
			if (maxfps==8)
			{
				Engine.MaxFps=0;
			}
			DisplayServer.WindowSetVsyncMode((DisplayServer.VSyncMode)file.GetValue("Settings","vsync",(int)DisplayServer.VSyncMode.Enabled).AsInt32());
			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"),file.GetValue("Settings","volume",0).AsSingle());
			GetViewport().UseHdr2D=file.GetValue("Settings","use_hdr",false).AsBool();
		}
		else
		{
			//DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
			GD.PushError($"Settings file load failed, error code: {(int)load}("+load.ToString()+")");
		}
		foreach (var arg in OS.GetCmdlineUserArgs())
		{
			if (arg.StartsWith("-locale="))
			{
				TranslationServer.SetLocale(arg.Right(8));
				GD.Print($"Launch argument \"{arg}\" was detected, changing language.");
			}
		}
		//加载控制选项
		var f=new ConfigFile();
		Error er=f.Load("user://GodotTemplate/Settings/control.ini");
		if (er == Error.Ok)
		{
			control_mode=(CONTROL_MODE)(int)f.GetValue("ControlOptions","control_mode",(int)CONTROL_MODE.KEYBOARD_MOUSE);
		}
		else
		{
			GD.PushError($"Settings(Controls) file load failed, error code: {(int)er}("+er.ToString()+")");
		}
		//加载按键绑定
		var fk=new ConfigFile();
		Error erk=fk.Load("user://GodotTemplate/Settings/keybinds.ini");
		if (erk == Error.Ok)
		{
			for (var action=0;action<actions.Count;action+=1)
			{
				InputMap.ActionEraseEvents(actions.Keys.ToArray()[action]);
				var array=(Array<InputEvent>)fk.GetValue("Keybinds",actions.Keys.ToArray()[action],new Array<InputEvent>());
				for (var key=0;key<array.Count;key+=1)
				{
					InputMap.ActionAddEvent(actions.Keys.ToArray()[action],array[key]);
				}
			}
		}
		else
		{
			GD.PushError($"Settings(Keybinds) file load failed, error code: {(int)erk}("+erk.ToString()+")");
		}
		//日志输出信息
		if (OS.IsStdOutVerbose())
		{
			var Text="\nVersion: v"+version + (mod ? " - Modded" : "") + (locale ? " - Locale" : "");
			Text+="\nCurrent Time ("+Time.GetTimeZoneFromSystem()["name"]+"): "+Time.GetDatetimeStringFromSystem(false,true);
			Text+="\nCurrent Time (Universal Time Coordinated): "+Time.GetDatetimeStringFromSystem(true,true);
			Text+="\nSystem: "+OS.GetName()+"("+OS.GetDistributionName()+") "+OS.GetVersion()+" "+Engine.GetArchitectureName();
			Text+="\nSystem Locale: "+OS.GetLocale()+" ("+TranslationServer.GetLocaleName(OS.GetLocale())+")";
			Text+="\nGame Locale: "+TranslationServer.GetLocale()+" ("+TranslationServer.GetLocaleName(TranslationServer.GetLocale())+")";
			Text+="\nAudio Language: "+lang_audio.ToString();
			Text+="\nEditor Locale: "+TranslationServer.GetToolLocale()+" ("+TranslationServer.GetLocaleName(TranslationServer.GetToolLocale())+")";
			Text+="\nDevice ID: "+((OS.GetUniqueId()==null || OS.GetUniqueId()=="") ? "N/A" : OS.GetUniqueId());
			Text+="\nModel Name: "+OS.GetModelName();
			Text+="\nCPU: "+((OS.GetProcessorName()==null || OS.GetProcessorName()=="") ? "N/A" : OS.GetProcessorName());
			Text+="\nCPU Processors Count: "+OS.GetProcessorCount().ToString();
			Text+="\nVideo Adapter: "+((RenderingServer.GetVideoAdapterName()==null || RenderingServer.GetVideoAdapterName()=="")? "N/A" : "("+RenderingServer.GetVideoAdapterVendor()+") "+RenderingServer.GetVideoAdapterName()+" "+RenderingServer.GetVideoAdapterApiVersion());
			Text+="\nDisplayServer Name: "+DisplayServer.GetName();
			Text+="\nRendering Method: "+RenderingServer.GetCurrentRenderingMethod();
			Text+="\nRendering Driver: "+RenderingServer.GetCurrentRenderingDriverName();
			Text+="\nWindow Mode: "+DisplayServer.WindowGetMode().ToString();
			Text+="\nWindow Current Screen: "+DisplayServer.WindowGetCurrentScreen().ToString();
			Text+="\nWindow Position,Decorations: "+DisplayServer.WindowGetPosition().ToString()+","+DisplayServer.WindowGetPositionWithDecorations().ToString();
			Text+="\nScreen Size,Usable Rect: "+DisplayServer.ScreenGetSize(DisplayServer.WindowGetCurrentScreen()).ToString()+", {"+DisplayServer.ScreenGetUsableRect(DisplayServer.WindowGetCurrentScreen()).ToString()+"}";
			Text+="\nWindow Size,Decorations: "+DisplayServer.WindowGetSize().ToString()+","+DisplayServer.WindowGetSizeWithDecorations().ToString();
			Text+="\nWindow Focused: "+DisplayServer.WindowIsFocused().ToString();
			var layout=DisplayServer.KeyboardGetCurrentLayout();
			Text+="\nKeyboard Layout: "+DisplayServer.KeyboardGetLayoutName(layout)+" ("+DisplayServer.KeyboardGetLayoutLanguage(layout)+")";
			Text+="\nUser Data Directory: \""+OS.GetUserDataDir()+"\"";
			Text+="\nExecutable Path: \""+OS.GetExecutablePath()+"\"";
			Text+="\nWindow Current Screen: "+DisplayServer.WindowGetCurrentScreen().ToString();
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
			GD.Print(Text+"\n");
		}
		//加载语言文件
		if (locale)
		{
			var ffff=FileAccess.Open(GetGameDirPath("Locales")+"/LoadOrder.txt",FileAccess.ModeFlags.Read);
			var errrr=FileAccess.GetOpenError();
			if (errrr == Error.Ok)
			{
				var locales=ffff.GetAsText(true).Split("\n");
				ffff.Close();
				foreach (var i in locales)
				{
					var csvm=GetGameDirPath("Locales")+"/"+i;
					if (!i.Equals(""))
					{
						if (FileAccess.FileExists(csvm))
						{
							var fff=FileAccess.Open(csvm,FileAccess.ModeFlags.Read);
							var errr=FileAccess.GetOpenError();
							if (errr == Error.Ok)
							{
								if (csvm.EndsWith(".csv"))
								{
									locale_loaded = locale_loaded.Append(i).ToArray<string>();
									var langs=fff.GetCsvLine();
									var lines=fff.GetAsText(true).Split("\n");
									foreach (var lang in langs)
									{
										if (lang != "keys")
										{
											fff.Close();
											fff=FileAccess.Open(csvm,FileAccess.ModeFlags.Read);
											var old_trans = TranslationServer.FindTranslations(lang, true);
											var translation_old = old_trans.Count == 0 ? null : old_trans[0];
											var translation = new Translation();
											translation.Locale=lang;
											foreach (var b in lines)
											{
												var current=fff.GetCsvLine();
												if (System.Array.IndexOf(lines,b) != 0)
												{
													if (!b.Equals(""))
													{
														translation.EraseMessage(current[0]);
														translation.AddMessage(current[0],current[System.Array.IndexOf(langs,lang)].Replace("\\n","\n"));
													}
												}
											}
											if (translation_old != null)
											{
												TranslationServer.RemoveTranslation(translation_old);
												TranslationServer.AddTranslation(TranslationOverride.Create(lang,translation_old,translation));
											}
											else
											{
												TranslationServer.AddTranslation(translation);
											}
										}
									}
								}
								else if (csvm.EndsWith(".po"))
								{
									locale_loaded = locale_loaded.Append(i).ToArray<string>();
									var lines = fff.GetAsText(true).Split("\n");
									var lang = i.Substring(0,i.LastIndexOf("."));
									foreach (var line in lines)
									{
										if (line.StartsWith("\"Language: "))
										{
											lang = line.Substring(11,(line.Length - 11 - 3));
											break;
										}
									}
									var old_trans = TranslationServer.FindTranslations(lang, true);
									var translation_old = old_trans.Count == 0 ? null : old_trans[0];
									var translation=new Translation();
									translation.Locale=lang;
									foreach (var line in lines)
									{
										if (line.StartsWith("msgid"))
										{
											var id = line.Substring(7,(line.Length - 7 - 1));
											var nextline = lines[System.Array.IndexOf(lines,line) + 1];
											var str = "";
											if (nextline != "" && nextline.StartsWith("msgstr"))
											{
												str = nextline.Substring(8,(nextline.Length - 8 - 1));
												var check = ((System.Array.IndexOf(lines,nextline) + 2) <= lines.Length);
												if (check)
												{
													nextline = lines[System.Array.IndexOf(lines,nextline) + 1];
												}
												while (check && nextline.StartsWith("\"") && nextline.EndsWith("\""))
												{
													str += nextline.Substring(1,(nextline.Length - 2));
													check = ((System.Array.IndexOf(lines,nextline) + 2) <= lines.Length);
													if (check)
													{
														nextline = lines[System.Array.IndexOf(lines,nextline) + 1];
													}
												}
											}
											if ((!id.Equals(""))&&(!str.Equals("")))
											{
												translation.EraseMessage(id);
												translation.AddMessage(id,str.Replace("\\n","\n"));
											}
										}
									}
									if (translation_old != null)
									{
										TranslationServer.RemoveTranslation(translation_old);
										TranslationServer.AddTranslation(TranslationOverride.Create(lang,translation_old,translation));
									}
									else
									{
										TranslationServer.AddTranslation(translation);
									}
								}
								else
								{
									GD.PushError($"Localization file ({csvm}) can't be loaded, only CSV and PO file is allowed.");
								}
								fff.Close();
							}
							else
							{
								GD.PushError($"Localization file ({csvm}) load failed, error code: {(int)errr}("+errr.ToString()+")");
							}
						}
					}
				}
			}
			else
			{
				GD.PushError($"Localization file's load order file load failed, error code: {(int)errrr}("+errrr.ToString()+")");
			}
			var print="Localization files Loaded: {\"";
			for (var a=0;a<locale_loaded.Length;a+=1)
			{
				print+=locale_loaded[a]+"\",\"";
			}
			print=print.TrimSuffix(",\"")+"}";
			if (print.EndsWith("{\"}"))
			{
				print=print.TrimSuffix("\"}")+"}";
			}
			GD.Print(print);
		}
		//加载mod文件
		if (mod)
		{
			//模组文件
			var fffff=FileAccess.Open(GetGameDirPath("Mods")+"/LoadOrder.txt",FileAccess.ModeFlags.Read);
			var errrrr=FileAccess.GetOpenError();
			if (errrrr == Error.Ok)
			{
				var mods=fffff.GetAsText(true).Split("\n");
				fffff.Close();
				foreach (var modi in mods)
				{
					if (!modi.Equals(""))
					{
						var mod=GetGameDirPath("Mods")+"/"+modi;
						if (FileAccess.FileExists(mod+".dll"))
						{
							var dll = mod+".dll";
							Assembly a = null;
							try
							{
								//a = Assembly.LoadFrom(dll);
								a = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()).LoadFromAssemblyPath(dll);
								ScriptManagerBridge.LookupScriptsInAssembly(a);
							}
							catch (Exception e)
							{
								GD.PushError($"Mod \"{modi}\" ({dll}) load failed ({e}).");
								a = null;
								throw;
							}
							if (a != null)
							{
								/*try
								{
									a.GetType("Init").GetMethod("Init").Invoke(null,null);
								}
								catch (Exception e)
								{
									GD.PushError($"Mod \"{modi}\" ({dll}) Init method invoke failed ({e}).");
									a = null;
									throw;
								}
								if (a != null)
								{*/
									mods_loaded = mods_loaded.Append(modi+".dll").ToArray<string>();
									GD.Print($"Mod \"{modi}\" ({dll}) loaded.");
								//}
							}
						}
						if (FileAccess.FileExists(mod+".pck"))
						{
							var pck = mod+".pck";
							var loaded=ProjectSettings.LoadResourcePack(pck);
							if (loaded)
							{
								mods_loaded = mods_loaded.Append(modi+".pck").ToArray<string>();
								GD.Print($"Mod \"{modi}\" ({pck}) loaded.");
							}
							else
							{
								GD.PushError($"Mod \"{modi}\" ({pck}) load failed.");
							}
						}
						if (FileAccess.FileExists("res://ModsLoaded/"+modi+"/Init.tscn"))
						{
							GD.Load<PackedScene>("res://ModsLoaded/"+modi+"/Init.tscn").Instantiate();
						}
					}
				}
			}
			else
			{
				GD.PushError($"Mod's load order file load failed, error code: {(int)errrrr}("+errrrr.ToString()+")");
			}
			var print="Mods Loaded: {\"";
			for (var a=0;a<mods_loaded.Length;a+=1)
			{
				print+=mods_loaded[a]+"\",\"";
			}
			print=print.TrimSuffix(",\"")+"}";
			if (print.EndsWith("{\"}"))
			{
				print=print.TrimSuffix("\"}")+"}";
			}
			GD.Print(print);
		}
		//清空消息列表
		ClearMessageList();
		//检测RenderingDriver
		var driver = RenderingServer.GetCurrentRenderingDriverName();
		/*if (driver == "d3d12")
		{
			CreatePopUpMessage("locWarningDriverD3d12",Message.TYPE.WARNING);
		}
		else */if (driver.StartsWith("opengl3"))
		{
			CreatePopUpMessage("locWarningDriverOpenGL3",Message.TYPE.WARNING);
		}
		if (OS.GetName() == "Windows" && driver == "vulkan")
		{
			CreatePopUpMessage("locWarningDriverVulkanWindows",Message.TYPE.WARNING);
		}
		//GDTask Tracker
		TaskTracker.EnableTracking = debug;
		//删除绑键缓存
		if (FileAccess.FileExists("user://GodotTemplate/Settings/keybinds_temp.ini"))
		{
			DirAccess.RemoveAbsolute("user://GodotTemplate/Settings/keybinds_temp.ini");
		}
		//随机化 重置Camera 加载入场动画
		GD.Randomize();
		GetNode<Camera2D>("Camera2D").Zoom = Vector2.One;
		if ((!first_time) && (tree.CurrentScene.SceneFilePath != "res://System/Empty.tscn"))
		{
			tree.ChangeSceneToFile("res://System/Empty.tscn");
		}
	}

	public override void _EnterTree()
	{
		GameInit(true);
	}

	public override void _Process(double delta)
	{
		var tree = GetTree();
		//场景更新
		if (tree.CurrentScene != null && current_scene_path != tree.CurrentScene.SceneFilePath)
		{
			current_scene_path = tree.CurrentScene.SceneFilePath;
			if (OS.IsStdOutVerbose())
			{
				GD.Print($"[{Time.GetDatetimeStringFromSystem(false,true)}] Scene changed: {tree.CurrentScene.Name} ({tree.CurrentScene.SceneFilePath})");
			}
		}
		//最大fps根据刷新率更新
		if (max_fps_sel==7 && Engine.MaxFps!=Mathf.RoundToInt(DisplayServer.ScreenGetRefreshRate(DisplayServer.WindowGetCurrentScreen())))
		{
			Engine.MaxFps=Mathf.RoundToInt(DisplayServer.ScreenGetRefreshRate(DisplayServer.WindowGetCurrentScreen()));
		}
		//Game.tscn防进入
		if (tree.CurrentScene!=null && tree.CurrentScene.SceneFilePath=="res://System/Game.tscn")
		{
			Crashed(TranslationServer.Translate("locErrorGameScene"));
		}
		//fader颜色
		GetNode<ColorRect>("CanvasLayer/Fader").Color=fader_color;
		//截图
		if ((!input_remapping) && Input.IsActionJustPressed("screenshot"))
		{
			var datetime=Time.GetDatetimeStringFromSystem(false,true).Replace(" ","_").Replace(":","-");
			screenshot_target_dir="user://GodotTemplate/Screenshots/"+datetime+".png";
			if (FileAccess.FileExists(screenshot_target_dir))
			{
				var files=DirAccess.GetFilesAt("user://GodotTemplate/Screenshots");
				for (var a=1;a<=files.Length;a+=1)
				{
					screenshot_target_dir=screenshot_target_dir.TrimSuffix(".png").TrimSuffix("_"+(a-1).ToString())+"_"+a.ToString()+".png";
					if (!FileAccess.FileExists(screenshot_target_dir))
					{
						break;
					}
				}
			}
			RenderingServer.FramePostDraw+=SaveScreenshot;
		}
		//全屏
		if ((!input_remapping) && Input.IsActionJustPressed("fullscreen") && !Engine.IsEmbeddedInEditor())
		{
			if (DisplayServer.WindowGetMode()==DisplayServer.WindowMode.Windowed)
			{
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
			}
			else if (DisplayServer.WindowGetMode()==DisplayServer.WindowMode.ExclusiveFullscreen || DisplayServer.WindowGetMode()==DisplayServer.WindowMode.Fullscreen)
			{
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			}
			UpdateConfigFile("user://GodotTemplate/Settings/settings.ini","Settings","display_mode",(int)DisplayServer.WindowGetMode());
		}
		//Debug功能
		if (debug)
		{
			if (Input.IsActionJustPressed("debug_overlay"))
			{
				debug_overlay = !debug_overlay;
				GetNode<DebugOverlay>("CanvasLayer/DebugOverlay").Text = DebugOverlay.Update(GetNode<DebugOverlay>("CanvasLayer/DebugOverlay"),tree);
				GetNode<DebugOverlay2>("CanvasLayer/DebugOverlay2").Text = DebugOverlay2.Update();
			}
			if (debug_overlay && Input.IsActionJustPressed("debug_overlay_r"))
			{
				debug_overlay_r = !debug_overlay_r;
				GetNode<DebugOverlay2>("CanvasLayer/DebugOverlay2").Text = DebugOverlay2.Update();
			}
			if (Input.IsActionJustPressed("debug_restart"))
			{
				if (OS.IsStdOutVerbose())
				{
					GD.Print($"[{Time.GetDatetimeStringFromSystem(false,true)}] Restarted game with keybind.");
				}
				GameInit();
			}
			if (Input.IsActionJustPressed("debug_restart_scene"))
			{
				if (OS.IsStdOutVerbose())
				{
					GD.Print($"[{Time.GetDatetimeStringFromSystem(false,true)}] Restarted scene with keybind: {tree.CurrentScene.Name} ({tree.CurrentScene.SceneFilePath})");
				}
				tree.ReloadCurrentScene();
			}
			if (Input.IsActionJustPressed("debug_gdtask_tracker"))
			{
				TaskTracker.ShowTrackerWindow();
			}
			if (Input.IsActionJustPressed("debug_menu_window"))
			{
				GetNode<Window>("CanvasLayer/Windows/DebugMenu").Visible = !GetNode<Window>("CanvasLayer/Windows/DebugMenu").Visible;
			}
			if (Input.IsActionPressed("debug_zoom_in"))
			{
				GetNode<Camera2D>("Camera2D").Zoom += Vector2.One * 0.005f;
			}
			if (Input.IsActionPressed("debug_zoom_out"))
			{
				GetNode<Camera2D>("Camera2D").Zoom -= Vector2.One * 0.005f;
			}
			if (Input.IsActionPressed("debug_zoom_reset"))
			{
				GetNode<Camera2D>("Camera2D").Zoom = Vector2.One;
			}
		}
	}
	//创建弹出消息
	internal void CreatePopUpMessage(string message,Message.TYPE type=Message.TYPE.MESSAGE,bool copy_button=true,double shown_time=5,bool close_button=true,bool custom_button=false,string custom_button_text=null,string custom_method_name=null)
	{
		var scene=GD.Load<PackedScene>("res://System/Message.tscn");
		var m=scene.Instantiate<Message>();
		m.shown_time=shown_time;
		m.type=type;
		m.GetNode<RichTextLabel>("PanelContainer/MarginContainer/Text").Text=message;
		m.GetNode<Button>("Button/Copy").Visible=copy_button;
		m.GetNode<Button>("Button/Close").Visible=close_button;
		GetNode<VBoxContainer>("CanvasLayer/Messages").AddChild(m);
		var t="["+Time.GetDatetimeStringFromSystem(false,true)+"][Popup";
		if (type==Message.TYPE.MESSAGE)
		{
			t+=",Message";
		}
		if (type==Message.TYPE.WARNING)
		{
			t+=",Warning";
		}
		if (type==Message.TYPE.ERROR)
		{
			t+=",Error";
		}
		t+=","+TranslationServer.GetLocale();
		//if (message.StartsWith("loc"))
		if (TranslationServer.Translate(message) != message)
		{
			t+="(en)";
			message=TranslationServer.FindTranslations("en", true)[0].GetMessage(message);
		}
		if (close_button)
		{
			t+=",CloseButton";
		}
		if (copy_button)
		{
			t+=",CopyButton";
		}
		if (custom_button)
		{
			if (custom_button_text == null)
			{
				GD.PushError("Error: Null custom_button_text with CustomButton enabled");
			}
			else if (custom_button_text == "")
			{
				GD.PushError("Error: Empty custom_button_text with CustomButton enabled");
			}
			else
			{
				var bt=custom_button_text;
				if (bt.StartsWith("loc"))
				{
					bt="(en)"+TranslationServer.FindTranslations("en", true)[0].GetMessage(bt);
				}
				t+=",CustomButton{\""+bt+"\",\""+custom_method_name+"\"}";
			}
		}
		if (type==Message.TYPE.MESSAGE)
		{
			GD.Print(t+"] "+message);
		}
		if (type==Message.TYPE.WARNING)
		{
			GD.PushWarning(t+"] "+message);
		}
		if (type==Message.TYPE.ERROR)
		{
			GD.PushError(t+"] "+message);
		}
		m.GetNode<Button>("Button/Custom").Visible=custom_button;
		m.GetNode<Button>("Button/Custom").Text=custom_button_text;
		m.custom_method_name=custom_method_name;
	}
	//清空消息列表
	internal void ClearMessageList()
	{
		var children=GetNode<VBoxContainer>("CanvasLayer/Messages").GetChildren();
		for (var a=0;a<children.Count;a+=1)
		{
			children[a].QueueFree();
		}
	}
	//截屏
	void SaveScreenshot()
	{
		var window_size = ((DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen || DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen) ? DisplayServer.ScreenGetSize(DisplayServer.WindowGetCurrentScreen()) : DisplayServer.WindowGetSize(DisplayServer.WindowGetCurrentScreen()));
		var img = GetViewport().GetTexture().GetImage();
		img.Resize(window_size[0],window_size[1]);//should be replace with another method in the future
		var err = img.SavePng(screenshot_target_dir);
		if (err == Error.Ok)
		{
			CreatePopUpMessage(TranslationServer.Translate("locMessageScreenshot").ToString().Replace("{ScreenshotDirectory}",screenshot_target_dir.Replace("user:/",OS.GetUserDataDir())));
		}
		else
		{
			CreatePopUpMessage(TranslationServer.Translate("locMessageScreenshotFailed").ToString().Replace("{ScreenshotDirectory}",screenshot_target_dir.Replace("user:/",OS.GetUserDataDir()))+((int)err).ToString()+"("+err.ToString()+")");
		}
		RenderingServer.FramePostDraw-=SaveScreenshot;
	}
	//更新档案
	internal static bool UpdateConfigFile(string path,string section,string key,Variant variant,string password=null)
	{
		var result=true;
		var file=new ConfigFile();
		if (FileAccess.FileExists(path))
		{
			Error er;
			if (password!=null)
			{
				er=file.LoadEncryptedPass(path,password);
			}
			else
			{
				er=file.Load(path);
			}
			if (er != Error.Ok)
			{
				GD.PushError($"\"{path}\" load failed, error code: {(int)er}("+er.ToString()+")");
				result=false;
			}
		}
		file.SetValue(section,key,variant);
		Error err;
		if (password!=null)
		{
			err=file.SaveEncryptedPass(path,password);
		}
		else
		{
			err=file.Save(path);
		}
		if (err != Error.Ok)
		{
			GD.PushError($"\"{path}\" save failed, error code: {(int)err}("+err.ToString()+")");
			result=false;
		}
		return result;
	}
	// Crash Handler
	internal void Crashed(string crashinfo)
	{
		crash_info = crashinfo;
		GetTree().ChangeSceneToFile("res://System/CrashHandler.tscn");
	}
	//文件目录
	internal static string GetGameDirPath(string str = "")
	{
		if (OS.HasFeature("editor"))
		{
			return ProjectSettings.GlobalizePath("res://"+str);
		}
		else
		{
			return OS.GetExecutablePath().GetBaseDir()+"/"+str;
		}
	}
	//语言是否是中日韩
	internal static bool IsLocaleCJK(string loc = null)
	{
		if (loc == null)
		{
			loc = TranslationServer.GetLocale();
		}
		return (loc.StartsWith("zh") || loc.StartsWith("ja") || loc.StartsWith("ko"));
	}
	//游戏退出
	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			if (OS.IsStdOutVerbose())
			{
				GD.Print($"[{Time.GetDatetimeStringFromSystem(false,true)}] Game shutting down.");
			}
			//停止所有GDTask
			GDTaskGlobalCancellation.Cancel();
		}
	}
}
