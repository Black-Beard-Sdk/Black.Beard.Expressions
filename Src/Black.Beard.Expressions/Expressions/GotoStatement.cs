using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bb.Expresssions
{
    public class GotoStatement : Statement
    {

        public Label Label { get; set; }

        public Expression Expression { get; set; }

        public override Expression GetExpression(HashSet<string> variableParent)
        {
            return null;
        }


    }

}
