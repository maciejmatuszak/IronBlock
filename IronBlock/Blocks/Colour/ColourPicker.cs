namespace IronBlock.Blocks.Colour
{
    public class ColourPicker : ABlock
    {
        public override object EvaluateInternal(Context context)
        {
            return Fields.Get("COLOUR") ?? "#000000";
        }
    }
}