public static class ExpressionEvaluator
{
    private enum TokenType { Number, Plus, Minus, Mul, Div }

    private struct Token
    {
        public TokenType type;
        public float number;

        public Token(TokenType t)
        {
            type = t;
            number = 0f;
        }

        public Token(float num)
        {
            type = TokenType.Number;
            number = num;
        }
    }

    public static bool TryEvaluate(string expression, out float result)
    {
        result = 0f;
        if (string.IsNullOrWhiteSpace(expression))
            return false;

        if (!TryTokenize(expression, out var tokens))
            return false;

        if (!TryEvalTokens(tokens, out result))
            return false;

        return true;
    }

    private static bool TryTokenize(string expr, out System.Collections.Generic.List<Token> tokens)
    {
        tokens = new System.Collections.Generic.List<Token>();
        int i = 0;
        bool expectNumber = true; // at start, a number or a signed number

        while (i < expr.Length)
        {
            char c = expr[i];

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            if (expectNumber)
            {
                // number can start with + or - sign
                int start = i;
                if (c == '+' || c == '-')
                {
                    i++;
                }

                bool hasDigits = false;
                while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.'))
                {
                    hasDigits = true;
                    i++;
                }

                if (!hasDigits)
                    return false;

                string numStr = expr.Substring(start, i - start);
                if (!float.TryParse(numStr, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out float value))
                {
                    return false;
                }

                tokens.Add(new Token(value));
                expectNumber = false;
            }
            else
            {
                // expecting an operator
                TokenType opType;
                switch (c)
                {
                    case '+': opType = TokenType.Plus; break;
                    case '-': opType = TokenType.Minus; break;
                    case '*': opType = TokenType.Mul; break;
                    case '/': opType = TokenType.Div; break;
                    default: return false;
                }

                tokens.Add(new Token(opType));
                i++;
                expectNumber = true; // after an operator, we expect another number
            }
        }

        // expression can't end with an operator
        if (expectNumber && tokens.Count > 0)
            return false;

        return tokens.Count > 0;
    }

    private static bool TryEvalTokens(System.Collections.Generic.List<Token> tokens, out float result)
    {
        // First pass: handle * and /
        var reduced = new System.Collections.Generic.List<Token>
        {
            tokens[0] // first must be a number
        };

        int i = 1;
        while (i < tokens.Count)
        {
            var op = tokens[i];
            var next = tokens[i + 1];

            if (op.type == TokenType.Mul || op.type == TokenType.Div)
            {
                if (reduced[reduced.Count - 1].type != TokenType.Number || next.type != TokenType.Number)
                {
                    result = 0f;
                    return false;
                }

                float a = reduced[reduced.Count - 1].number;
                float b = next.number;
                float v = 0f;

                if (op.type == TokenType.Mul)
                    v = a * b;
                else
                    v = b != 0f ? a / b : 0f;

                reduced[reduced.Count - 1] = new Token(v);
            }
            else
            {
                // + or - just copy
                reduced.Add(op);
                reduced.Add(next);
            }

            i += 2;
        }

        // Second pass: handle + and -
        if (reduced.Count == 0 || reduced[0].type != TokenType.Number)
        {
            result = 0f;
            return false;
        }

        float acc = reduced[0].number;
        i = 1;

        while (i < reduced.Count)
        {
            var op = reduced[i];
            var num = reduced[i + 1];

            if (num.type != TokenType.Number)
            {
                result = 0f;
                return false;
            }

            switch (op.type)
            {
                case TokenType.Plus:
                    acc += num.number;
                    break;
                case TokenType.Minus:
                    acc -= num.number;
                    break;
                default:
                    result = 0f;
                    return false;
            }

            i += 2;
        }

        result = acc;
        return true;
    }
}