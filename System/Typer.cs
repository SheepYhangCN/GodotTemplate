using Godot;
using System;

[GlobalClass]
public partial class Typer : RichTextLabel
{
	[Export] internal double Speed = 8d;
	[Export] internal bool Sound = true;
	[Export] internal AudioStream SoundAudio = GD.Load<AudioStream>("res://Resources/Audios/TextType.wav");
	double timer = 0f;
	AudioStreamPlayer player;
	[Signal] public delegate void finished_typingEventHandler();
	[Signal] public delegate void skipped_typingEventHandler();
	public override void _Ready()
	{
		BbcodeEnabled = true;
		FitContent = true;
		ScrollActive = false;
		ClipContents = false;
		AutowrapMode = TextServer.AutowrapMode.Off;
		VisibleCharacters=0;
		player = new AudioStreamPlayer();
		player.Stream = SoundAudio;
		player.MaxPolyphony = 5;
		AddChild(player);
	}

	public override void _Process(double delta)
	{
		timer += delta;
		if (Sound && VisibleCharacters <= GetTotalCharacterCount() && VisibleCharacters != (int)Math.Floor(timer*Speed))
		{
			player.Play();
		}
		VisibleCharacters = (int)Math.Floor(timer*Speed);
		if ((!Text.Equals("")) && VisibleCharacters >= GetTotalCharacterCount())
		{
			EmitSignal("finished_typing");
		}
	}

	internal void Reset()
	{
		timer = 0d;
		VisibleCharacters = 0;
	}

	internal void Skip()
	{
		timer = Math.Floor(GetTotalCharacterCount()/Speed)+1;
		VisibleCharacters = GetTotalCharacterCount();
		EmitSignal("skipped_typing");
	}
}
