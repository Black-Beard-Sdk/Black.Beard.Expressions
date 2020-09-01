using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bb.Expresssions.Statements
{
    public class ForStatement : LoopStatement
    {


        public ForStatement(SourceCode parent, Expression InitialExpression)
            : base(parent)
        {

        }

        public ParameterExpression Index { get; set; }

        public Expression MoveIndex { get; set; }


        public override Expression GetExpression(HashSet<string> variableParent)
        {

            if (MoveIndex == null)
                throw new NullReferenceException(nameof(MoveIndex));

            Body.Add(MoveIndex);

            return base.GetExpression(variableParent);

        }


    }

}
