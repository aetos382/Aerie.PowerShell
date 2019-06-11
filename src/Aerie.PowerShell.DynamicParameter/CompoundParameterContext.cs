using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal class CompoundParameterContext :
        ICompoundParameterContext
    {
        [NotNull]
        private static readonly ConditionalWeakTable<ICompoundParameter, CompoundParameterContext> _contexts =
            new ConditionalWeakTable<ICompoundParameter, CompoundParameterContext>();

        [NotNull]
        public static CompoundParameterContext Register(
            [NotNull] DynamicParameterContext parentContext,
            [NotNull] MemberInfo compoundParameterMember,
            [NotNull] ICompoundParameter compoundParameterValue)
        {
            if (parentContext is null)
            {
                throw new ArgumentNullException(nameof(parentContext));
            }

            if (compoundParameterMember is null)
            {
                throw new ArgumentNullException(nameof(compoundParameterMember));
            }

            if (compoundParameterValue is null)
            {
                throw new ArgumentNullException(nameof(compoundParameterValue));
            }

            var context = _contexts.GetValue(
                compoundParameterValue,
                _ => new CompoundParameterContext(parentContext, compoundParameterMember));

            return context;
        }

        [NotNull]
        public static CompoundParameterContext GetContext(
            [NotNull] ICompoundParameter compoundParameter)
        {
            if (compoundParameter is null)
            {
                throw new ArgumentNullException(nameof(compoundParameter));
            }

            bool result = _contexts.TryGetValue(compoundParameter, out var context);

            if (!result)
            {
                throw new ArgumentException();
            }

            return context;
        }

        private CompoundParameterContext(
            [NotNull] DynamicParameterContext parentContext,
            [NotNull] MemberInfo compoundParameterMember)
        {
            this._parentContext = parentContext;
            this.CurrentParameter = compoundParameterMember;
        }

        [NotNull]
        private readonly DynamicParameterContext _parentContext;

        [NotNull]
        public DynamicParameterInstance EnableParameter(
            [NotNull][ItemNotNull] ParameterMemberInfo member)
        {
            /*
            Ensure.ArgumentNotNull(member, nameof(member));

            var allMembers = new List<MemberInfo>
            {
                this.CurrentParameter
            };

            allMembers.AddRange(member);

            var descriptor = ReflectParameterDescriptor.GetDescriptor(new ParameterMemberInfo(allMembers));

            var instance = this._parentContext.EnableParameter(descriptor);
            return instance;
            */
            throw new NotImplementedException();
        }

        public IDynamicParameterContext ParentContext
        {
            get
            {
                return this._parentContext;
            }
        }

        public MemberInfo CurrentParameter { get; }
    }
}
