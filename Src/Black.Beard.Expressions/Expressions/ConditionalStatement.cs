using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Bb.Expresssions
{

    public class ConditionalStatement : Statement
    {

        public Expression Expression { get; set; }

        public SourceCode Then { get; set; }

        public SourceCode Else { get; set; }

        public override Expression GetExpression(HashSet<string> variableParent)
        {

            Expression b1 = Then.GetExpression(new HashSet<string>(variableParent));
            Expression b2 = null;

            if (Else != null)
                b2 = Else.GetExpression(new HashSet<string>(variableParent));

            Expression expression = b2 == null
                ? Expression.IfThen(Expression, b1)
                : Expression.IfThenElse(Expression, b1, b2)
                ;

            if (expression.CanReduce)
                expression = expression.Reduce();

            return expression;

        }

    }

}
