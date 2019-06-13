using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

using JetBrains.Annotations;

namespace Aerie.PowerShell.DynamicParameter
{
    public class DynamicProxyProvider :
        IDynamicParameterObjectProvider
    {
        [NotNull]
        private static readonly Dictionary<string, Func<CmdletContext, object>> _constructorCache =
            new Dictionary<string, Func<CmdletContext, object>>();

        [NotNull]
        private static readonly AssemblyBuilder _assemblyBuilder;

        [NotNull]
        private static readonly ModuleBuilder _moduleBuilder;

        static DynamicProxyProvider()
        {
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName(AssemblyInfo.ProxyAssemblyName),
                AssemblyBuilderAccess.RunAndCollect);

            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(
                AssemblyInfo.ProxyAssemblyName + ".dll");
        }

        private DynamicProxyProvider()
        {
        }

        [NotNull]
        public static readonly DynamicProxyProvider Instance = new DynamicProxyProvider();

        public object GetDynamicParameterObject(
            CmdletContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var parameters = context.GetParameters();
            string descriptorIds = string.Join("_", parameters.Select(x => x.ParameterDescriptor.Id).OrderBy(x => x));

            string proxyTypeName = $"DynamicProxy_{descriptorIds}";

            if (!_constructorCache.TryGetValue(proxyTypeName, out var constructor))
            {
                _constructorCache[proxyTypeName] = constructor = CreateProxyType(proxyTypeName, context);
            }

            var proxyObject = constructor(context);
            return proxyObject;
        }

        [NotNull]
        private static Func<CmdletContext, object> CreateProxyType(
            [NotNull] string proxyTypeName,
            [NotNull] CmdletContext context)
        {
            const TypeAttributes typeAttributes =
                TypeAttributes.Class |
                TypeAttributes.Public |
                TypeAttributes.Sealed;

            const FieldAttributes fieldAttributes =
                FieldAttributes.InitOnly |
                FieldAttributes.Private;

            var typeBuilder = _moduleBuilder.DefineType(
                proxyTypeName,
                typeAttributes);

            var contextFieldBuilder = typeBuilder.DefineField(
                "_context",
                typeof(CmdletContext),
                fieldAttributes);

            GenerateConstructor(typeBuilder, contextFieldBuilder);

            foreach (var parameter in context.GetParameters())
            {
                AddParameter(
                    typeBuilder,
                    parameter.ParameterDescriptor,
                    contextFieldBuilder);
            }

            var generatedType = typeBuilder.CreateTypeInfo();
            var constructorDelegate = GetConstructorDelegate(generatedType);

            return constructorDelegate;
        }

        private static void GenerateConstructor(
            [NotNull] TypeBuilder typeBuilder,
            [NotNull] FieldInfo contextField)
        {
            const MethodAttributes constructorAttributes =
                MethodAttributes.Public |
                MethodAttributes.HideBySig |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName;

            var constructorBuilder = typeBuilder.DefineConstructor(
                constructorAttributes,
                CallingConventions.Standard,
                new[] { typeof(CmdletContext) });

            var baseConstructor = typeof(object).GetConstructor(Type.EmptyTypes);

            var constructorGenerator = constructorBuilder.GetILGenerator();

            constructorGenerator.Emit(OpCodes.Ldarg_0); // load this
            constructorGenerator.Emit(OpCodes.Call, baseConstructor); // call base::ctor
            
            constructorGenerator.Emit(OpCodes.Ldarg_0); // load this
            constructorGenerator.Emit(OpCodes.Ldarg_1); // load constructor parameter
            constructorGenerator.Emit(OpCodes.Stfld, contextField); // set field

            constructorGenerator.Emit(OpCodes.Ret);
        }

        private static void AddParameter(
            [NotNull] TypeBuilder typeBuilder,
            [NotNull] ParameterDescriptor descriptor,
            [NotNull] FieldInfo contextField)
        {
            string parameterName = descriptor.ParameterName;
            var parameterType = descriptor.ParameterType;

            var propertyBuilder = typeBuilder.DefineProperty(
                parameterName,
                PropertyAttributes.None,
                parameterType,
                Type.EmptyTypes);

            SetParameterAttributes(propertyBuilder, descriptor, true);

            const MethodAttributes accessorAttributes =
                MethodAttributes.Public |
                MethodAttributes.PrivateScope |
                MethodAttributes.HideBySig |
                MethodAttributes.SpecialName;

            var getterBuilder = typeBuilder.DefineMethod(
                "get_" + parameterName,
                accessorAttributes,
                parameterType,
                Type.EmptyTypes);

            var getterGenerator = getterBuilder.GetILGenerator();
            
            EmitLoadMember(getterGenerator, descriptor, contextField);

            var setterBuilder = typeBuilder.DefineMethod(
                "set_" + parameterName,
                accessorAttributes,
                typeof(void),
                new[] { parameterType });

            var setterGenerator = setterBuilder.GetILGenerator();

            EmitStoreMember(setterGenerator, descriptor, contextField);

            propertyBuilder.SetGetMethod(getterBuilder);
            propertyBuilder.SetSetMethod(setterBuilder);
        }

        [NotNull]
        private static Func<CmdletContext, object> GetConstructorDelegate(
            [NotNull] TypeInfo generatedType)
        {
            var constructor = generatedType.GetConstructor(new[] { typeof(CmdletContext) });

            var contextParameterExpression = Expression.Parameter(typeof(CmdletContext));

            var lambdaExpression = Expression.Lambda<Func<CmdletContext, object>>(
                Expression.Convert(
                    Expression.New(
                        constructor,
                        contextParameterExpression),
                    typeof(object)),
                true,
                contextParameterExpression);

            var constructorDelegate = lambdaExpression.Compile();
            return constructorDelegate;
        }

        private static void EmitLoadMember(
            [NotNull] ILGenerator generator,
            [NotNull] ParameterDescriptor descriptor,
            [NotNull] FieldInfo contextField)
        {
            var method = typeof(CmdletContext).GetMethod(
                nameof(CmdletContext.GetParameterValue),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, contextField);

            generator.Emit(OpCodes.Ldstr, descriptor.ParameterName);
            generator.EmitCall(OpCodes.Call, method, Type.EmptyTypes);

            if (descriptor.ParameterType.IsValueType)
            {
                generator.Emit(OpCodes.Unbox_Any, descriptor.ParameterType);
            }

            generator.Emit(OpCodes.Ret);
        }

        private static void EmitStoreMember(
            [NotNull] ILGenerator generator,
            [NotNull] ParameterDescriptor descriptor,
            [NotNull] FieldInfo contextField)
        {
            var method = typeof(CmdletContext).GetMethod(
                nameof(CmdletContext.SetParameterValue),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, contextField);

            generator.Emit(OpCodes.Ldstr, descriptor.ParameterName);
            generator.Emit(OpCodes.Ldarg_1);

            if (descriptor.ParameterType.IsValueType)
            {
                generator.Emit(OpCodes.Box, descriptor.ParameterType);
            }

            generator.EmitCall(OpCodes.Call, method, Type.EmptyTypes);

            generator.Emit(OpCodes.Ret);
        }

        private static void SetParameterAttributes(
            [NotNull] PropertyBuilder propertyBuilder,
            [NotNull] ParameterDescriptor parameterDescriptor,
            bool checkAttributeTarget)
        {
            foreach (var attribute in parameterDescriptor.Attributes)
            {
                if (checkAttributeTarget)
                {
                    bool valid = IsAttributeValidOnProperty(attribute);
                    if (!valid)
                    {
                        continue;
                    }
                }

                var attributeBuilder = ConvertToCustomAttributeBuilder(attribute);
                propertyBuilder.SetCustomAttribute(attributeBuilder);
            }
        }

        [NotNull]
        private static CustomAttributeBuilder ConvertToCustomAttributeBuilder(
            [NotNull] CustomAttributeData attributeData)
        {
            var constructorArgumentValues =
                attributeData.ConstructorArguments.Select(x => x.Value).ToArray();

            var properties = new List<PropertyInfo>();
            var propertyValues = new List<object>();

            var fields = new List<FieldInfo>();
            var fieldValues = new List<object>();

            foreach (var argument in attributeData.NamedArguments)
            {
                if (argument.IsField)
                {
                    fields.Add((FieldInfo)argument.MemberInfo);
                    fieldValues.Add(argument.TypedValue.Value);
                }
                else
                {
                    properties.Add((PropertyInfo)argument.MemberInfo);
                    propertyValues.Add(argument.TypedValue.Value);
                }
            }

            var attributeBuilder = new CustomAttributeBuilder(
                attributeData.Constructor,
                constructorArgumentValues,
                properties.ToArray(),
                propertyValues.ToArray(),
                fields.ToArray(),
                fieldValues.ToArray());

            return attributeBuilder;
        }

        [NotNull]
        private static readonly Dictionary<Type, bool> _attributeIsValidOnProperty = new Dictionary<Type, bool>();

        private static bool IsAttributeValidOnProperty(
            [NotNull] CustomAttributeData attribute)
        {
            var type = attribute.AttributeType;

            if (!_attributeIsValidOnProperty.TryGetValue(type, out bool valid))
            {
                var attributeUsageAttribute =
                    type.GetCustomAttribute<AttributeUsageAttribute>();

                if (attributeUsageAttribute != null)
                {
                    valid = attributeUsageAttribute.ValidOn.HasFlag(AttributeTargets.Property);
                }
                else
                {
                    valid = true;
                }

                _attributeIsValidOnProperty[type] = valid;
            }

            return valid;
        }
    }
}
