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
            if (parentContext == null)
            {
                throw new ArgumentNullException(nameof(parentContext));
            }

            if (compoundParameterMember == null)
            {
                throw new ArgumentNullException(nameof(compoundParameterMember));
            }

            if (compoundParameterValue == null)
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
            if (compoundParameter == null)
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
            [NotNull][ItemNotNull] IReadOnlyList<MemberInfo> members)
        {
            if (members == null)
            {
                throw new ArgumentNullException(nameof(members));
            }

            var allMembers = new List<MemberInfo>
            {
                this.CurrentParameter
            };

            allMembers.AddRange(members);

            var descriptor = ReflectParameterDescriptor.GetParameterDescriptor(allMembers);

            var instance = this._parentContext.EnableParameter(descriptor);
            return instance;
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
