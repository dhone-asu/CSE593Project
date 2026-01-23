using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAS
{
    public enum RuleElement { Expr, Op, LParen, RParen, Ident, Num, Params };

    class RuleManager
    {
        public static RuleElement[] RuleBinaryOp = new RuleElement[] { RuleElement.Expr, RuleElement.Op, RuleElement.Expr };
        public static RuleElement[] RuleParentheses = new RuleElement[] { RuleElement.LParen, RuleElement.Expr, RuleElement.RParen };
        public static RuleElement[] RuleIdentifier = new RuleElement[] { RuleElement.Ident };
        public static RuleElement[] RuleNumeral = new RuleElement[] { RuleElement.Num };

        public static Dictionary<TokenType, RuleElement> RuleElementMap = new Dictionary<TokenType, RuleElement>()
        { 
            { TokenType.Plus, RuleElement.Op },
            { TokenType.Minus, RuleElement.Op },
            { TokenType.Div, RuleElement.Op },
            { TokenType.Mult, RuleElement.Op },
            { TokenType.Pow, RuleElement.Op },
            { TokenType.Mod, RuleElement.Op },
            { TokenType.Eq, RuleElement.Op },
            { TokenType.Neq, RuleElement.Op },
            { TokenType.Gt, RuleElement.Op },
            { TokenType.Lt, RuleElement.Op },
            { TokenType.Gte, RuleElement.Op },
            { TokenType.Lte, RuleElement.Op },
            { TokenType.Identifier, RuleElement.Ident },
            { TokenType.Numeral, RuleElement.Num }
        };

        public static readonly Dictionary<TokenType, BinaryOperator> OperatorMap = new Dictionary<TokenType, BinaryOperator>
        {
            {TokenType.Plus, BinaryOperator.Plus},
            {TokenType.Minus, BinaryOperator.Minus},
            {TokenType.Div, BinaryOperator.Div},
            {TokenType.Mult, BinaryOperator.Mult},
            {TokenType.Pow, BinaryOperator.Pow},
            {TokenType.Mod, BinaryOperator.Mod},
            {TokenType.Eq, BinaryOperator.Eq},
            {TokenType.Gt, BinaryOperator.Gt},
            {TokenType.Lt, BinaryOperator.Lt},
            {TokenType.Gte, BinaryOperator.Gte},
            {TokenType.Lte, BinaryOperator.Lte},
        };
        public RuleManager()
        {
            Rules.Add(RuleBinaryOp);
            Rules.Add(RuleParentheses);
            Rules.Add(RuleIdentifier);
            Rules.Add(RuleNumeral);
        }

        public List<RuleElement[]> Rules { get; set; } = new List<RuleElement[]>();
    }
}
