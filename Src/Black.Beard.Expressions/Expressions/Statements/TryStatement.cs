using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bb.Expresssions.Statements
{
    public class TryStatement : Statement
    {


        public TryStatement(SourceCode parent)
            : base(parent)
        {

        }


        public CatchStatement Catch(Type self)
        {

            var result = new CatchStatement(this._parent)
            {
                TypeToCatch = self,
                
            };
            this.Catchs.Add(result);

            return result;

        }

        public SourceCode Try { get; set; }

        public List<CatchStatement> Catchs { get; set; } = new List<CatchStatement>();

        public SourceCode Finally 
        {
            get
            {
                if (_finally == null)
                    _finally = new SourceCode(this._parent);

                return _finally;
            }
           
        }



        public override Expression GetExpression(HashSet<string> variableParent)
        {

            List<CatchBlock> _catchs = new List<CatchBlock>();
            Expression resultExpression;
            Expression expressionFinaly = null;

            Expression expressionTry = Try.GetExpression(new HashSet<string>(variableParent));

            foreach (CatchStatement @catch in Catchs)
            {
                CatchBlock c;
                if (@catch.Parameter != null)
                {
                    variableParent.Add(@catch.Parameter.Name);

                    @catch.Body.AddVarIfNotExists(@catch.Parameter);
                    var body = @catch.GetExpression(variableParent);
                    c = Expression.Catch(@catch.Parameter, body);

                    variableParent.Remove(@catch.Parameter.Name);

                }
                else
                {
                    var body = @catch.GetExpression(variableParent);
                    c = Expression.Catch(@catch.TypeToCatch, body);
                }
                _catchs.Add(c);
            }

            if (_finally != null)
                    expressionFinaly = _finally.GetExpression(new HashSet<string>(variableParent));


            if (expressionFinaly != null)
            {
                if (_catchs.Count > 0)
                    resultExpression = Expression.TryCatchFinally(expressionTry, expressionFinaly, _catchs.ToArray());
                else
                    resultExpression =Expression.TryFinally(expressionTry, expressionFinaly);
            }
            else
                resultExpression = Expression.TryCatch(expressionTry, _catchs.ToArray());

            if (resultExpression.CanReduce)
                resultExpression = resultExpression.Reduce();

            return resultExpression;

        }


        private SourceCode _finally;

    }

}
