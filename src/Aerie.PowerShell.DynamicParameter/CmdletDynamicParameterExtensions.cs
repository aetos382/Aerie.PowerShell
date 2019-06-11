using System;
using System.Linq.Expressions;
using System.Management.Automation;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public static class CmdletDynamicParameterExtensions
    {
        [NotNull]
        public static DynamicParameterInstance EnableDynamicParameter<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] string parameterExpression)
            where TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterExpression is null)
            {
                throw new ArgumentNullException(nameof(parameterExpression));
            }

            var members = new ParameterMemberInfo(typeof(TCmdlet), parameterExpression);
            var descriptor = ReflectParameterDescriptor.GetDescriptor(members);

            var instance = EnableDynamicParameter(cmdlet, descriptor);

            return instance;
        }
        
        [NotNull]
        public static DynamicParameterInstance EnableDynamicParameter<TCmdlet, TParameter>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] Expression<Func<TCmdlet, TParameter>> parameterExpression)
            where TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterExpression is null)
            {
                throw new ArgumentNullException(nameof(parameterExpression));
            }

            var members = new ParameterMemberInfo((MemberExpression)parameterExpression.Body);
            var descriptor = ReflectParameterDescriptor.GetDescriptor(members);

            var instance = EnableDynamicParameter(cmdlet, descriptor);

            return instance;
        }

        [NotNull]
        public static DynamicParameterInstance EnableDynamicParameter<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] DynamicParameterDescriptor parameterDescriptor)
            where TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterDescriptor is null)
            {
                throw new ArgumentNullException(nameof(parameterDescriptor));
            }

            var context = DynamicParameterContext.GetContext(cmdlet);
            var instance = context.EnableParameter(parameterDescriptor);

            return instance;
        }

/*
        public static bool IsParameterEnabled<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] string parameterName)
            where TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            bool enabled = TryGetParameter(cmdlet, parameterName, out _);
            return enabled;
        }

        private static bool IsParameterEnabled<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] DynamicParameterDescriptor descriptor)
            where TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            bool enabled = TryGetParameter(cmdlet, descriptor.ParameterName, out _);
            return enabled;
        }

        public static bool IsParameterEnabled<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] string parameterExpression)
            where  TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterExpression is null)
            {
                throw new ArgumentNullException(nameof(parameterExpression));
            }

            var descriptor = DynamicParameterDescriptor.GetParameterDescriptor(typeof(TCmdlet), parameterExpression);

            bool enabled = TryGetParameter(cmdlet, descriptor.ParameterName, out _);
            return enabled;
        }

        public static bool IsParameterEnabled<TCmdlet, TParameter>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] Expression<Func<TCmdlet, TParameter>> parameterExpression)
            where  TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterExpression is null)
            {
                throw new ArgumentNullException(nameof(parameterExpression));
            }

            var descriptor = DynamicParameterDescriptor.GetParameterDescriptor(parameterExpression);

            bool enabled = TryGetParameter(cmdlet, descriptor.ParameterName, out _);
            return enabled;
        }
*/

        [NotNull]
        public static object GetParameterProxy<TCmdlet>(
            [NotNull] this TCmdlet cmdlet)
            where TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            var proxy = GetDynamicParameterObject(cmdlet, ReflectParameterProxyBuilder.Instance);
            return proxy;
        }

        [NotNull]
        public static object GetDynamicParameterObject<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] IDynamicParameterObjectBuilder factory)
            where  TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var context = DynamicParameterContext.GetContext(cmdlet);

            var dynamicParameterObject = context.GetDynamicParameterObject(factory);

            return dynamicParameterObject;
        }

        public static bool TryGetParameter<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] string parameterName,
            [NotNull] out DynamicParameterInstance parameter)
            where  TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            var context = DynamicParameterContext.GetContext(cmdlet);

            bool enabled = context.TryGetParameter(parameterName, out parameter);
            return enabled;
        }

        public static bool TryGetParameterValue<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] string parameterName,
            [CanBeNull] out object parameterValue)
            where  TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            parameterValue = default;

            bool enabled = TryGetParameter(cmdlet, parameterName, out var parameter);
            if (!enabled)
            {
                return false;
            }

            parameterValue = parameter.Value;
            return parameter.IsSet;
        }

        [Pure]
        public static bool IsParameterSet<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] string parameterName)
            where  TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (parameterName is null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            bool enabled = TryGetParameter(cmdlet, parameterName, out var parameter);
            if (!enabled)
            {
                return false;
            }

            return parameter.IsSet;
        }

        public static void PopulateRuntimeDefinedParameterValues<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] RuntimeDefinedParameterDictionary runtimeDefinedParameters)
            where TCmdlet : Cmdlet
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (runtimeDefinedParameters is null)
            {
                throw new ArgumentNullException(nameof(runtimeDefinedParameters));
            }

            var context = DynamicParameterContext.GetContext(cmdlet);

            foreach (var parameter in runtimeDefinedParameters.Values)
            {
                context.SetParameterValue(parameter.Name, parameter.Value);
            }
        }
    }
}
