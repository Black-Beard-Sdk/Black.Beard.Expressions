using Bb.Expresssions;
using Bb.Expresssions.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Bb.Expresssions
{

    public partial class SourceCode
    { 


        #region Exceptions

        public TryStatement Try()
        {
            return Try(new SourceCode(this));
        }

        public TryStatement Try(SourceCode self)
        {

            self._parent = this;

            var tryStatement = new TryStatement(this)
            {
                Try = self,
            };

            this.Add(tryStatement);

            return tryStatement;

        }

        #endregion Exceptions


        #region Goto

        public GotoStatement Break()
        {
            var label = this.GetLabelImpl(KindLabelEnum.Break);
            var l = new GotoStatement(this) { Label = label };
            Add(l);
            return l;
        }

        public GotoStatement Continue()
        {
            var label = this.GetLabelImpl(KindLabelEnum.Continue);
            var l = new GotoStatement(this) { Label = label };
            Add(l);
            return l;
        }

        public GotoStatement Return(Expression @return)
        {
            var label = this.GetLabelImpl(KindLabelEnum.Return);
            var l = new GotoStatement(this) { Label = label, Expression = @return, };
            Add(l);
            return l;

        }

        private Label GetLabelImpl(KindLabelEnum kind)
        {

            Label label = this._labels.Items.FirstOrDefault(c => c.Kind == kind);

            if (label == null && _parent == null)
                label = _parent.GetLabelImpl(kind);

            if (label == null)
                throw new Exceptions.InvalidArgumentNameMethodReferenceException($"no label of {kind.ToString()} defined");

            return label;

        }

        #region Labels 

        public Label AddLabel(string name = null, KindLabelEnum kind = KindLabelEnum.Default)
        {

            if (string.IsNullOrEmpty(name))
                name = this._variables.GetNewName();

            var instance = Expression.Label(Labels.GetNewName());
            var label = new Label() { Instance = instance, Kind = kind, Name = instance.Name };
            this.AddLabel(label);

            return label;
        }

        public Label AddLabel(Label label)
        {
            this._labels.Add(label);
            return label;
        }

        #endregion Labels 

        #endregion Goto


        #region variables

        public ParameterExpression AddVarIfNotExists(ParameterExpression parameter)
        {

            var vari = this.GetVar(parameter.Name);
            if (vari == null)
                this.AddVar(parameter);

            return parameter;

        }

        public ParameterExpression AddVarIfNotExists(Type type, string name)
        {

            var variable = this.GetVar(name);
            if (variable != null)
                return variable;

            if (string.IsNullOrEmpty(name))
                name = this._variables.GetNewName();

            var instance = Expression.Parameter(type, name);
            this.AddVar(instance);

            return instance;

        }

        public ParameterExpression AddVar(Type type, string name = null, Expression initialization = null)
        {

            if (string.IsNullOrEmpty(name))
                name = this._variables.GetNewName();

            var instance = Expression.Parameter(type, name);
            this.AddVar(instance);

            if (initialization != null)
                this.Assign(instance, initialization);

            return instance;
        }

        public ParameterExpression AddVar(ParameterExpression arg)
        {

            var vari = this._variables.GetByName(arg.Name);
            if (vari != null)
            {
                if (vari.Instance != arg)
                    throw new Exceptions.DuplicatedArgumentNameMethodReferenceException($"parameter {arg.Name} already exists");
            }
            else
            {
                vari = new Variable() { Name = arg.Name, Instance = arg };
                this._variables.Add(vari);
                this.LastVariable = arg;
            }

            return vari.Instance;

        }

        public virtual ParameterExpression GetVar(string name)
        {
            var variable = _variables.GetByName(name);
            if (variable == null)
            {
                if (_parent != null)
                    return _parent.GetVar(name);
            }
            else
                return variable.Instance;

            return null;

        }

        #endregion variables



    }


}
