namespace IronBlock.Blocks.Colour
{
    public class ColourPicker : ABlock
    {
        public override object EvaluateInternal(IContext context)
        {
            return Fields.Get("COLOUR") ?? "#000000";
        }
    }
}