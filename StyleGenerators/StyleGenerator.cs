using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StyleGenerators
{
    [Generator]
    public class StyleGenerator : ISourceGenerator
    {
        private static readonly Accessibility[] legalAccessibilities = { Accessibility.NotApplicable, Accessibility.Public };

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new StyleSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not StyleSyntaxReceiver ssr)
            {
                return;
            }

            INamedTypeSymbol styleIgnoreAttrSymbol = context.Compilation.GetTypeByMetadataName("MagicUI.Styles.StyleIgnoreAttribute");

            foreach (INamedTypeSymbol ts in ssr.Classes)
            {
                string generatedStyleName = ts.Name + "Style";
                string generatedContent = ProcessClass(ts, generatedStyleName, styleIgnoreAttrSymbol);
                context.AddSource($"{generatedStyleName}.g.cs", SourceText.From(generatedContent, Encoding.UTF8));
            }
        }

        private string ProcessClass(INamedTypeSymbol classSymbol, string generatedClassName, ISymbol styleIgnoreSymbol)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                // todo: issue a diagnostic that stylable can only be applied to top-level classes
                return null;
            }

            IEnumerable<IPropertySymbol> eligibleProps = GetTypesNested(classSymbol)
                .SelectMany(t => t.GetMembers().Select(m => m as IPropertySymbol))
                .Where(p => p != null && p.DeclaredAccessibility == Accessibility.Public)
                .Where(p => !p.GetAttributes().Any(ad => ad.AttributeClass.Equals(styleIgnoreSymbol, SymbolEqualityComparer.Default)));

            StringBuilder source = new($@"
#nullable annotations

namespace MagicUI.Styles
{{
    /// <summary>
    /// Generated styles for a <see cref=""{classSymbol.ToDisplayString()}""/>
    /// </summary>
    public sealed class {generatedClassName} : MagicUI.Styles.IStyle<{classSymbol.ToDisplayString()}>
    {{
");
            List<IPropertySymbol> finalProperties = new();

            foreach (IPropertySymbol ps in eligibleProps)
            {
                // we need properties which have a public getter and setter. we know the prop itself is declared public from above
                if (ps.GetMethod == null || !legalAccessibilities.Contains(ps.GetMethod.DeclaredAccessibility))
                {
                    continue;
                }
                if (ps.SetMethod == null || !legalAccessibilities.Contains(ps.SetMethod.DeclaredAccessibility))
                {
                    continue;
                }

                source.AppendLine($@"
        /// <summary>
        /// Controls the <see cref=""{ps.ToDisplayString()}""/> property of the {classSymbol.Name}
        /// </summary>
        public {ps.Type.ToDisplayString()} {ps.Name} {{ get; set; }}");
                finalProperties.Add(ps);
            }

            source.AppendLine($@"
        /// <summary>
        /// Creates a style based on the stylable properties of the base {classSymbol.Name}
        /// </summary>
        /// <param name=""source"">The base {classSymbol.Name} to derive the properties from</param>
        public {generatedClassName}({classSymbol.ToDisplayString()} source)
        {{
");
            foreach (IPropertySymbol ps in finalProperties)
            {
                source.AppendLine($"            {ps.Name} = source.{ps.Name};");
            }

            source.AppendLine($@"
        }}

        /// <inheritdoc/>
        public void Apply({classSymbol.ToDisplayString()} target)
        {{
");
            foreach (IPropertySymbol ps in finalProperties)
            {
                source.AppendLine($"            target.{ps.Name} = {ps.Name};");
            }

            source.AppendLine(@"
        }
    }
}
");
            return source.ToString();
        }

        private IEnumerable<ITypeSymbol> GetTypesNested(ITypeSymbol ts)
        {
            ITypeSymbol current = ts;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }
    }
}
