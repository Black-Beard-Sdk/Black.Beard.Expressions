using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Bb.Expresssions.Statements
{

    public class ConditionalStatement : Statement
    {


        public ConditionalStatement(SourceCode parent)
            : base(parent)
        {

        }

        public Expression ConditionalExpression { get; set; }

        public SourceCode Then
        {
            get
            {
                if (_then == null)
                    _then = new SourceCode(this._parent);

                return _then;
            }
        }

        public SourceCode Else
        {
            get
            {
                if (_else == null)
                    _else = new SourceCode(this._parent);

                return _else;
            }
        }


        private SourceCode _then;
        private SourceCode _else;

        public override Expression GetExpression(HashSet<string> variableParent)
        {

            Expression b1 = Then.GetExpression(new HashSet<string>(variableParent));
            Expression b2 = null;

            if (_else != null)
                b2 = Else.GetExpression(new HashSet<string>(variableParent));

            Expression expression = b2 == null
                ? Expression.IfThen(ConditionalExpression, b1)
                : Expression.IfThenElse(ConditionalExpression, b1, b2)
                ;

            if (expression.CanReduce)
                expression = expression.Reduce();

            return expression;

        }

    }

}
