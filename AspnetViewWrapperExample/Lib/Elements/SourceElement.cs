using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Html;

namespace AspnetViewWrapperExample.Elements
{

    public interface IAttribute
    {
        public string AttributeKey { get; }
        public string AttributeValue { get; }
    }
    
    public class Styles : IAttribute
    {
        public Dictionary<string, string> StyleMap { get; set; }
        
        public void AddStyle(string key, string val) => 
            (StyleMap??=new Dictionary<string,string>())[key] = val;

        public string GetStyles() =>
            $"{string.Join("; ", StyleMap.Select(e => $"{e.Key}: {e.Value}"))}";

        public string AttributeKey => "";
        public string AttributeValue => $"{string.Join("; ", StyleMap.Select(e => $"{e.Key}: {e.Value}"))}";

        public override string ToString() => GetStyles();
    }
    
    public class CssClasses
    {
        public HashSet<string> ClassMap { get; set; }

        public string GetClasses() => $"{string.Join(" ", ClassMap)}";
        
        public string addClass
        {
            set => AddClass(value);
        }
        
        public void AddClass(string item)=> 
            (ClassMap??=new HashSet<string>()).Add(item);

        public override string ToString() => GetClasses();
    }

    public class ElementEntry
    {
        public SourceElement Self { get; set; }
        public dynamic DataContainer { get; set; }
    }
    
    public class SourceElement
    {
        public Dictionary<int, ElementEntry> ElementDataStore { get; set; }
        public int ParentId;
        public int Id;

        public Dictionary<string, object>  TabSpaceContent { get; set; }
        private Styles _styles { get; set; }
        private CssClasses _classes { get; set; }

        public Dictionary<string, object> GetTabSpaceContent() => TabSpaceContent ??= new Dictionary<string, object>();

        public Styles Styles
        {
            get => _styles;
            set => _styles = (Styles) (GetTabSpaceContent()["Style"] = value);
        }
        public CssClasses Classes
        {
            get => _classes;
            set => _classes = (CssClasses) (GetTabSpaceContent()["Class"] = value);
        }

        public SourceElement Configure(Action<SourceElement> lam)
        {
            lam?.Invoke(this);
            return this;
        }

        public SourceElement WithStyles(out Styles styles)
        {
            styles = Styles = new Styles();
            return this;
        }
        public SourceElement WithClasses(out CssClasses classes)
        {
            classes = Classes = new CssClasses();
            return this;
        }

        public SourceElement AssignRoot()
        {
            ParentId = -1;
            ElementDataStore = new();
            ElementDataStore[Id] = new ElementEntry() {Self = this};
            return this;
        }

        public SourceElement CreateDataContainer<T>(out T item)
        {
            if (!ElementDataStore.TryGetValue(Id, out ElementEntry elem))
                elem = ElementDataStore[Id] = new ElementEntry();
            
            item = elem.DataContainer = Activator.CreateInstance<T>();
            
            return this;
        }

        public void SetDataContainer<T>(T item)
        {
            if (!ElementDataStore.TryGetValue(Id, out ElementEntry elem))
                elem = ElementDataStore[Id] = new ElementEntry();

            elem.DataContainer = item;
        }

        public string GetDataObjectTemplate(SourceElement index = null)
        {
            //var test = ElementDataStore[(index ?? this).Id].DataContainer;
            return $"@Model.{nameof(ElementDataStore)}[{(index??this).Id}].{nameof(ElementEntry.DataContainer)}";   
        }

        public string GetDataElementEntry(SourceElement index = null)
        {
            //var test = ElementDataStore[(index ?? this).Id].Self;
            return $"@Model.{nameof(ElementDataStore)}[{(index??this).Id}].{nameof(ElementEntry.Self)}";   
        }

        public SourceElement GetData<T>(out T data)
        {
            data = ElementDataStore[Id].DataContainer;
            return this;
        }
        
        public SourceElement AddChild<T>(out T component, Action<T> lam = null) where T: SourceElement
        {
            component = Activator.CreateInstance<T>();
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

        public virtual string RenderedContent => "no content";

        public override string ToString() => RenderedContent;

        public IList<SourceElement> Children { get; set; }
    }

    public class TagElement : SourceElement
    {
        public string Tag { get; set; }
        public string? BeforeContent { get; set; }
        public string? AfterContent { get; set; }

        public const string NO_ACCESSOR_PROVIDED = nameof(NO_ACCESSOR_PROVIDED);
        
        public TagElement SetContent(string property, string accessor = NO_ACCESSOR_PROVIDED, bool before = true)
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

        public IEnumerable<string> GetTabSpaceContents()
        {
            yield return Tag;
            if (TabSpaceContent is null) yield break;
            
            if(TabSpaceContent is null) yield break;

            foreach (var pair in TabSpaceContent)
            { 
                
                yield return $"{pair.Key.ToLower()}=\"{GetDataElementEntry()}.{nameof(TabSpaceContent)}[\"{pair.Key}\"].ToString()\"";  
            }
        }

        public HtmlString TabSpaceContentsAsString => new HtmlString(string.Join(" ", GetTabSpaceContents()));

        public override string RenderedContent => $"<{TabSpaceContentsAsString}>{BeforeContent}{string.Join("\n", (Children??Enumerable.Empty<SourceElement>()))}{AfterContent}</{Tag}>";
    }
}