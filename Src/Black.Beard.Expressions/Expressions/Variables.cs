using System;
using System.Collections.Generic;

namespace Bb.Expresssions
{

    public class Variables
    {

        public Variables()
        {
            this._variables = new Dictionary<string, Variable>();
        }

        internal void Add(Variable variable)
        {

            if (variable.Type == null)
                variable.Type = variable.Instance.Type;

            if (!this._variables.TryGetValue(variable.Name, out Variable variable2))
                this._variables.Add(variable.Name, variable);

            else if (variable.Instance != variable2.Instance)
                throw new Exceptions.DuplicatedArgumentNameMethodReferenceException($"{variable.Name} already existings");

        }

        internal Variable GetByName(string name)
        {

            if (this._variables.TryGetValue(name, out Variable variable))
                if (variable.Type == null && variable.Instance != null)
                    variable.Type = variable.Instance.Type;

            return variable;
        }

        internal string GetNewName()
        { 
            return $"var_{this._variables.Count}";
        }

        internal void RemoveByName(string name)
        {
            if (this._variables.ContainsKey(name))
                this._variables.Remove(name);
        }

        internal IEnumerable<Variable> Items { get => this._variables.Values; }

        private readonly Dictionary<string, Variable> _variables;

        internal void Merge(Variables variables)
        {

            if (variables != this)
                foreach (var item in variables.Items)
                    this.Add(item);

        }
    }

}
