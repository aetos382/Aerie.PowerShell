using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public abstract class DynamicParameterDescriptor
    {
        protected DynamicParameterDescriptor(
            [NotNull] string parameterName,
            [NotNull] Type parameterType)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            if (parameterType == null)
            {
                throw new ArgumentNullException(nameof(parameterType));
            }

            this.ParameterName = parameterName;
            this.ParameterType = parameterType;

            this.Attributes = new List<CustomAttributeData>();

            this.Id = GetId();
        }

        [NotNull]
        public string ParameterName { [Pure] get; }

        [NotNull]
        public Type ParameterType { [Pure] get; }

        [NotNull]
        [ItemNotNull]
        public IList<CustomAttributeData> Attributes { [Pure] get; }

        [CanBeNull]
        protected internal abstract object GetParameterValue(
            [NotNull] IDynamicParameterContext context);

        protected internal abstract void SetParameterValue(
            [NotNull] IDynamicParameterContext context,
            [CanBeNull] object value);

        internal int Id { [Pure] get; }

        private static int _currentId = 0;

        private static int GetId()
        {
            return Interlocked.Increment(ref _currentId);
        }
    }
}
