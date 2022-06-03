using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MarketPlace.Services.Products
{
    public interface IDescriptionParser
    {
        public Description ParseDescription(string description);
    }
    
    public class Description
    {
        public DescriptionProperty MainDescription { get; set; }
        public IEnumerable<DescriptionProperty> Properties { get; set; }
    }

    public class DescriptionProperty
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string PathType { get; set; }
    }
    public abstract class DescriptionParserManager
    {
        public IDescriptionParser Parser { protected get; set; }

        protected DescriptionParserManager(IDescriptionParser parser)
        { 
            Parser = parser;
        }
        public virtual Description ParseDescription(string description)
        {
            return Parser.ParseDescription(description);
        }
    }
    public class DescriptionParserManagerRealize : DescriptionParserManager
    {
        public DescriptionParserManagerRealize(IDescriptionParser parser) : base(parser)
        {

        }
        
    }
    public class DescriptionParserR : IDescriptionParser
    {

        public const string mainKey = "main";
        public const string propsKey = "props";
        public Description ParseDescription(string description)
        {
            bool isReaded = true;
            JObject j = null;
            try
            {
                j = JObject.Parse(description);
            }
            catch
            {
                isReaded = false;

            }

            

            if (isReaded)
            {
                Description d = new Description();
                var main = j[mainKey].ToString();
                d.MainDescription = new DescriptionProperty
                {
                    Name = "Описание",
                    PathType = j[mainKey].Path,    
                    Value = main
                };
                d.MainDescription.Type = ReadType(d.MainDescription.PathType);

                var props = j[propsKey];
                

                d.Properties = readJT(props);
                
                return d;
            }
            else
                return null;


            
        }


        private string ReadType(string type)
        {
            return new string(type.Reverse().ToArray().TakeWhile(ch => ch != '.').Reverse().ToArray());
        }

        IEnumerable<DescriptionProperty> readJT(JToken t)
        {
            var isBaseInstance = t.Children().Count() == 1 && t.Children().Count(ch => ch.Children().Count() > 0) == 0;
            if (isBaseInstance)
            {
                

                var pathType = t.Parent.Path;

                var type = ReadType(pathType);

                
                var prop = t as JProperty;
                List<DescriptionProperty> props = null;
                if (t is not null) 
                {
                    props = new List<DescriptionProperty>() { new DescriptionProperty { Name = $"{prop.Name}", Type = type, Value = prop.Value.ToString(),PathType = pathType } };
                }
                return props;
                
            }
            else
            {
                var childs = t.Children();
                List<DescriptionProperty> res = new List<DescriptionProperty>();
                foreach (var ch in childs)
                {
                    var props = readJT(ch);
                    foreach(var prop in props)
                    {
                        res.Add(prop);
                    }
                }
                return res;
            }
        }
    }
    
}
