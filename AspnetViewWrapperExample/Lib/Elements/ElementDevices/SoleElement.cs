using AspnetViewWrapperExample.Elements;

namespace AspnetViewWrapperExample.Lib.Elements.ElementDevices
{
    public class SoleElement : SourceElement<SoleElement>
    {
        public override string RenderedContent => $@"<{TagSpaceContentsAsString}/>";
    }
}