using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = true)]
    public sealed class ValidateMethodAttribute :
        ValidateEnumeratedArgumentsAttribute
    {
        [NotNull]
        private static Dictionary<string, Action<object>> _cache =
            new Dictionary<string, Action<object>>();

        public ValidateMethodAttribute(
            [NotNull] Type validatorType,
            [NotNull] string methodName)
        {
            if (validatorType == null)
            {
                throw new ArgumentNullException(nameof(validatorType));
            }

            if (methodName == null)
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            string fullName = $"{validatorType.FullName}:{methodName}";

            if (!_cache.TryGetValue(fullName, out var validator))
            {
                var (method, parameterType) = FindMethod(validatorType, methodName);

                validator = CreateDelegate(method, parameterType);

                _cache[fullName] = validator;
            }

            this.ValidatorType = validatorType;
            this.MethodName = methodName;

            this._delegate = validator;
        }

        protected override void ValidateElement(
            object element)
        {
            if (element is PSObject p)
            {
                element = p.BaseObject;
            }

            this._delegate(element);
        }

        // テスト用
        internal void InternalValidateElement(
            object element)
        {
            this.ValidateElement(element);
        }

        [NotNull]
        public Type ValidatorType { [Pure] get; }

        [NotNull]
        public string MethodName { [Pure] get; }

        [NotNull]
        private readonly Action<object> _delegate;

        [NotNull]
        [Pure]
        private static Action<object> CreateDelegate(
            [NotNull] MethodInfo method,
            [NotNull] Type parameterType)
        {
            var parameter = Expression.Parameter(typeof(object));

            var expression = Expression.Lambda<Action<object>>(
                Expression.Call(method, Expression.Convert(parameter, parameterType)),
                parameter);

            var d = expression.Compile();
            return d;
        }

        [Pure]
        private static (MethodInfo Method, Type ParameterType) FindMethod(
            [NotNull] Type validatorType,
            [NotNull] string methodName)
        {
            const BindingFlags bindingFlags =
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic;

            var methods = validatorType.GetMember(methodName, MemberTypes.Method, bindingFlags);

            bool found = false;

            MethodInfo method = null;
            Type parameterType = null;

            foreach (MethodInfo m in methods)
            {
                if (m.ReturnType != typeof(void))
                {
                    continue;
                }

                var parameters = m.GetParameters();
                if (parameters.Length != 1)
                {
                    continue;
                }

                if (found)
                {
                    // ambiguous method
                    throw new ArgumentException($"Method {validatorType.FullName}.{methodName} is ambiguous.");
                }

                found = true;

                method = m;
                parameterType = parameters[0].ParameterType;
            }

            if (!found)
            {
                // method not found
                throw new ArgumentException($"Method {validatorType.FullName}.{methodName} not found.");
            }

            return (method, parameterType);
        }
    }
}
