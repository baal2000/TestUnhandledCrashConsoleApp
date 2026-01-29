using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestUnhandledCrashConsoleApp
{
    /// <summary>
    /// Demonstrates LINQ optimization patterns for property reflection.
    /// This addresses the review comment from gemini-code-assist about avoiding
    /// unnecessary intermediate anonymous type allocations.
    /// </summary>
    public class LinqPatternDemonstration
    {
        // Example attribute for demonstration
        [AttributeUsage(AttributeTargets.Property)]
        public class CustomAttribute : Attribute
        {
            public string Value { get; set; }
            public CustomAttribute(string value) => Value = value;
        }

        // Example class with properties for demonstration
        public class SampleClass
        {
            [Custom("Property1")]
            public string Property1 { get; set; }

            [Custom("Property2")]
            public int Property2 { get; set; }

            public string PropertyWithoutAttribute { get; set; }

            [Custom("Property3")]
            public DateTime Property3 { get; set; }
        }

        // Container for property and its attribute
        public record PropertyWithAttribute(PropertyInfo Property, CustomAttribute Attribute);

        /// <summary>
        /// LESS EFFICIENT PATTERN: Uses intermediate anonymous type allocation.
        /// This creates anonymous objects that are later discarded, causing unnecessary GC pressure.
        /// </summary>
        public static IEnumerable<PropertyWithAttribute> GetPropertiesLessEfficient(Type type)
        {
            return type.GetProperties()
                .Select(prop => new { Property = prop, Attribute = prop.GetCustomAttribute<CustomAttribute>(inherit: false) })
                .Where(x => x.Attribute != null)
                .Select(x => new PropertyWithAttribute(x.Property, x.Attribute));
        }

        /// <summary>
        /// MORE EFFICIENT PATTERN: Directly creates the final object type.
        /// This avoids allocating intermediate anonymous types, reducing memory allocation
        /// and improving performance, especially with large collections.
        /// </summary>
        public static IEnumerable<PropertyWithAttribute> GetPropertiesMoreEfficient(Type type)
        {
            return type.GetProperties()
                .Select(prop => new PropertyWithAttribute(prop, prop.GetCustomAttribute<CustomAttribute>(inherit: false)))
                .Where(pwa => pwa.Attribute != null);
        }

        /// <summary>
        /// Demonstrates both patterns and verifies they produce the same results.
        /// </summary>
        public static void Demonstrate()
        {
            Console.WriteLine("=== LINQ Pattern Optimization Demonstration ===\n");

            var type = typeof(SampleClass);

            // Test less efficient pattern
            Console.WriteLine("1. Less Efficient Pattern (with intermediate anonymous type):");
            var resultsLessEfficient = GetPropertiesLessEfficient(type).ToList();
            foreach (var item in resultsLessEfficient)
            {
                Console.WriteLine($"   - {item.Property.Name}: {item.Attribute.Value}");
            }

            Console.WriteLine();

            // Test more efficient pattern
            Console.WriteLine("2. More Efficient Pattern (direct object creation):");
            var resultsMoreEfficient = GetPropertiesMoreEfficient(type).ToList();
            foreach (var item in resultsMoreEfficient)
            {
                Console.WriteLine($"   - {item.Property.Name}: {item.Attribute.Value}");
            }

            Console.WriteLine();

            // Verify both produce the same results
            bool areEqual = resultsLessEfficient.Count == resultsMoreEfficient.Count &&
                            resultsLessEfficient.Zip(resultsMoreEfficient, (a, b) => 
                                a.Property.Name == b.Property.Name && 
                                a.Attribute.Value == b.Attribute.Value)
                            .All(x => x);

            Console.WriteLine($"Results are equivalent: {areEqual}");

            Console.WriteLine("\n=== Key Improvements ===");
            Console.WriteLine("- Eliminates intermediate anonymous type allocation");
            Console.WriteLine("- Reduces GC pressure by allocating fewer objects");
            Console.WriteLine("- Improves performance especially with large collections");
            Console.WriteLine("- More readable and concise code");
        }
    }
}
