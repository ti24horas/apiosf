using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace ClassLibraryTest
{
#if NET451
    /// <summary>
    /// This is modafoca class
    /// </summary>
    public class Modafoca
    {
    }
#endif
    public abstract class TestWithConfig : IConfiguration
    {
        public abstract IConfigurationSection GetSection(string key);
        public abstract IEnumerable<IConfigurationSection> GetChildren();
        public abstract IChangeToken GetReloadToken();
        public abstract string this[string key] { get; set; }

        public virtual void DoSomething()
        {
            var x = XDocument.Load("ac");
            foreach (var v in x.Elements())
            {
                Debug.WriteLine(v.Name);
            }

        }
    }
    /// <summary>
    /// my class 1
    /// </summary>
    public class Class1
    {
        /// <summary>
        /// my constructor 1
        /// </summary>
        /// <param name="a">param a</param>
        public Class1(string a, JObject o)
        {
            Debug.WriteLine("OK");
        }

        public int ValueA { get; set; }

        public class Inner1
        {
            public class Inner2
            {
                
            }

            public struct InnerStructA
            {
                
            }
        }
    }

    public struct OuterStruct
    {
        public struct AnotherInnerStruct
        {
            
        }
    }

    /// <summary>
    /// My delegate AbC
    /// </summary>
    /// <param name="a">param 1</param>
    /// <param name="b">param 2</param>
    /// <param name="c">param 3</param>
    public delegate void Abc(string a, string b, string c);

    /// <summary>
    /// This interface is a xml placeholder
    /// </summary>
    public interface InterfaceSample
    {
        /// <summary>
        /// do something
        /// </summary>
        event EventHandler MyEvent;

        /// <summary>
        /// Do and return nothing
        /// </summary>
        void DoSomething();

        /// <summary>
        /// Do and return something.
        /// </summary>
        /// <returns>a value</returns>
        string DoAndReturnSomething();

        /// <summary>
        /// Do something.
        /// </summary>
        /// <param name="a">value a</param>
        /// <param name="b">value b</param>
        /// <param name="c">value c</param>
        void DoSomething(string a, IEnumerable<string> b, string c);

        /// <summary>
        /// Do and return something.
        /// </summary>
        /// <param name="a">value a</param>
        /// <param name="b">value b</param>
        /// <param name="c">value c</param>
        /// <returns>A new string</returns>
        string DoAndReturnSomething(string a, string b, string c);

        /// <summary>
        /// gets or sets property a
        /// </summary>
        string PropertyA
        {
            get;
            set;
        }
    }
}
