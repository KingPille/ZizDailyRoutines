using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.Common.Extensions;

public static class AtkValueArrayExtension
{
	extension(AtkValueArray valueArray)
	{
		public static AtkValueArray FromString(string[] inputs)
		{
			object[] array = new object[inputs.Length];
			for (int i = 0; i < inputs.Length; i++)
			{
				string text = inputs[i];
				if (string.IsNullOrWhiteSpace(text))
				{
					array[i] = string.Empty;
					continue;
				}
				ExpressionSyntax expressionSyntax = SyntaxFactory.ParseExpression(text);
				if (!(expressionSyntax is LiteralExpressionSyntax literalExpressionSyntax))
				{
					if (expressionSyntax is PrefixUnaryExpressionSyntax { Operand: LiteralExpressionSyntax operand } prefixUnaryExpressionSyntax && prefixUnaryExpressionSyntax.Kind() == SyntaxKind.UnaryMinusExpression)
					{
						dynamic value = operand.Token.Value;
						array[i] = -value;
					}
					else
					{
						array[i] = text;
					}
				}
				else
				{
					array[i] = literalExpressionSyntax.Token.Value ?? text;
				}
			}
			return new AtkValueArray(array);
		}
	}
}
