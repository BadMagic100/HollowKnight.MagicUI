using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace StyleGenerators
{
    internal class StyleSyntaxReceiver : ISyntaxContextReceiver
    {
        public List<INamedTypeSymbol> Classes { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is not ClassDeclarationSyntax cd || cd.AttributeLists.Count == 0)
            {
                return;
            }

            INamedTypeSymbol ts = context.SemanticModel.GetDeclaredSymbol(cd) as INamedTypeSymbol;
            if (ts.GetAttributes().Any(ad => ad.AttributeClass.ToDisplayString() == "MagicUI.Styles.StylableAttribute"))
            {
                Classes.Add(ts);
            }
        }
    }
}
