//Thanks to 神麤詭末 for inspiring.
using Godot;

public partial class TranslationOverride : Translation
{
	Translation old_obj;
	Translation new_obj;

	internal static TranslationOverride Create(string lang,Translation old_obje,Translation new_obje)
	{
		var result = new TranslationOverride();
		result.Locale = lang;
		result.old_obj = old_obje;
		result.new_obj = new_obje;
		return result;
	}

	public override StringName _GetMessage(StringName srcMessage, StringName context)
	{
		var src = new_obj.GetMessage(srcMessage, context);
		return (src.Equals("") ? old_obj.GetMessage(srcMessage, context) : src);
	}
}