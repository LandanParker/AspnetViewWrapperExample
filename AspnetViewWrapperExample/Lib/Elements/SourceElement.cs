using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using Microsoft.AspNetCore.Html;

namespace AspnetViewWrapperExample.Elements
{

    public interface INodeless
    {
        
    }

    public interface INodeable<out T> where T: SourceElement<T>
    {
        public T AddChild<A>(out A component, Action<SourceElement<A>> lam = null)
            where A : SourceElement<A>;
    }

    public class SourceElement
    {
        public const string NO_ACCESSOR_PROVIDED = nameof(NO_ACCESSOR_PROVIDED);
        public Dictionary<int, ElementEntry> ElementDataStore { get; set; }
        public int ParentId { get; set; }
        public int Id { get; set; }
        public string Tag { get; set; }

        private Styles _styles { get; set; }
        private CssClasses _classes { get; set; }
        public Dictionary<string, IAttribute>  TagSpaceContent { get; set; }

        public Dictionary<string, IAttribute> GetTagSpaceContent() => TagSpaceContent ??= new Dictionary<string, IAttribute>();

        public Styles Styles
        {
            get => _styles;
            set => _styles = (Styles) (GetTagSpaceContent()["Styles"] = value);
        }
        public CssClasses Classes
        {
            get => _classes;
            set => _classes = (CssClasses) (GetTagSpaceContent()["Class"] = value);
        }
        
        public IEnumerable<string> GetTagSpaceContents()
        {
            yield return Tag;
            if (TagSpaceContent is null) yield break;

            foreach ((string key, IAttribute val) in TagSpaceContent)
                yield return $"{val.AttributeKey}=\"{GetDataElementEntry()}.{nameof(TagSpaceContent)}[\"{key}\"].ToString()\"";  
        }

        public string TagSpaceContentsAsString => string.Join(" ", GetTagSpaceContents());
        
        public string GetDataObjectTemplate(SourceElement index = null)
        {
            //var test = ElementDataStore[(index ?? this).Id].DataContainer;
            return $"@Model.{nameof(ElementDataStore)}[{(index??this).Id}].{nameof(ElementEntry.DataContainer)}";   
        }

        public string GetDataElementEntry(SourceElement index = null)
        {
            //@[Model-SourceElementInstance].ElementDataStore[(index ?? this).Id].Self;
            return $"@Model.{nameof(ElementDataStore)}[{(index??this).Id}].{nameof(ElementEntry.Self)}";   
        }

        public override string ToString() => RenderedContent;
        public virtual string RenderedContent => $@"<{TagSpaceContentsAsString}>No Content////</{Tag}>";
    }
    
    public class SourceElement<T> : SourceElement where T: SourceElement<T>
    {
        
        public T WithStyles(out Styles styles)
        {
            styles = Styles = new Styles();
            return (T)this;
        }
        public T WithClasses(out CssClasses classes)
        {
            classes = Classes = new CssClasses();
            return (T)this;
        }

        public T AssignRoot()
        {
            ParentId = -1;
            ElementDataStore = new();
            ElementDataStore[Id] = new ElementEntry() {Self = this};
            return (T)this;
        }

        public ElementEntry CreateIfNotExists()
        {
            if (!ElementDataStore.TryGetValue(Id, out ElementEntry elementEntry))
                elementEntry = ElementDataStore[Id] = new ElementEntry();
            return elementEntry;
        }
        
        public T CreateDataContainer<A>(out A item)
        {
            CreateIfNotExists().DataContainer = item = Activator.CreateInstance<A>();
            return (T)this;
        }

        public T SetDataContainer<A>(A item)
        {
            CreateIfNotExists().DataContainer = item;
            return (T)this;
        }

        public T GetData<A>(out A data)
        {
            data = ElementDataStore[Id].DataContainer;
            return (T)this;
        }

        public IList<SourceElement> Children { get; set; }
    }
}