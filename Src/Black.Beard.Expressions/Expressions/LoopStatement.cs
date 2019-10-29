using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bb.Expresssions
{
    public class LoopStatement : Statement
    {

        public LoopStatement()
        {
            this._breakLabel = this.Body.AddLabel(Labels.GetNewName(), KindLabelEnum.Break);
            this._continueLabel = this.Body.AddLabel(Labels.GetNewName(), KindLabelEnum.Continue);

        }

        public SourceCode Body { get; set; }

        public override Expression GetExpression(HashSet<string> variableParent)
        {

            Expression b1 = Body.GetExpression(new HashSet<string>(variableParent));

            if (b1.CanReduce)
                b1 = b1.Reduce();

            var expression = Expression.Loop(b1, this._breakLabel.Instance, this._continueLabel.Instance);

            return expression;

        }


        private readonly Label _breakLabel;
        private readonly Label _continueLabel;


    }

}
