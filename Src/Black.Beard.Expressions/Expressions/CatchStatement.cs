using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bb.Expresssions
{

    public class CatchStatement : Statement
    {

        public CatchStatement()
        {
            this.Body = new SourceCode();
        }

        public SourceCode Body { get; set; }

        public Type TypeToCatch { get; set; }

        public ParameterExpression Parameter { get; set; }

        public override Expression GetExpression(HashSet<string> variableParent)
        {
            return Body.GetExpression(new HashSet<string>(variableParent));
        }

    }

}
