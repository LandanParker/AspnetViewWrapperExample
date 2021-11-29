using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspnetViewWrapperExample.Elements;
using Microsoft.Extensions.Primitives;

namespace AspnetViewWrapperExample.Lib.Elements.ElementDevices
{
    public class NodeElement : SourceElement<NodeElement>, INodeable<NodeElement>
    {
        
        public string? BeforeContent { get; set; }
        public string? AfterContent { get; set; }
        
        public NodeElement AddChild<A>(out A component, Action<SourceElement<A>> lam = null) where A: SourceElement<A>
        {
            component = Activator.CreateInstance<A>();
            component.ParentId = Id;
            component.ElementDataStore = ElementDataStore;
            component.Id = component.ElementDataStore.Count;
            
            if (!component.ElementDataStore.ContainsKey(component.Id))
            {
                component.ElementDataStore[component.Id] = new ElementEntry {Self = component};
            }
            
            lam?.Invoke(component);
            
            (Children??=new List<SourceElement>()).Add(component);
            return this;
        }
        
        public NodeElement SetContent(string property, string accessor = NO_ACCESSOR_PROVIDED, bool before = true)
        {
            var builder = new StringBuilder();
            
            switch (accessor)
            {
                case NO_ACCESSOR_PROVIDED:
                    builder.Append($"{this.GetDataObjectTemplate()}.{property}");
                    break;
                default:
                    builder.Append($"{accessor}.{property}");
                    break;
            }
            
            var hold = before?BeforeContent=builder.ToString():AfterContent = builder.ToString();
            
            return this;
        }
        
        public string ChildContentOrNullAsString => string.Join("\n", (Children ?? Enumerable.Empty<SourceElement>()));

        private string[] RenderContentArray =
        {
            "",//tag content
            "",//before content
            "",//content
            "",//after content
            "",//tag
        };
        
        public override string RenderedContent {
            get
            {
                RenderContentArray[0] = $"<{TagSpaceContentsAsString}>";
                RenderContentArray[1] = BeforeContent;
                RenderContentArray[2] = ChildContentOrNullAsString;
                RenderContentArray[3] = AfterContent;
                RenderContentArray[4] = $"</{Tag}>";

                return string.Join("", RenderContentArray.Where(e => !string.IsNullOrEmpty(e)));
            }
        }
    }

    public class SoleElement : SourceElement<SoleElement>
    {
        public override string RenderedContent => $@"<{TagSpaceContentsAsString}/>";
    }
}