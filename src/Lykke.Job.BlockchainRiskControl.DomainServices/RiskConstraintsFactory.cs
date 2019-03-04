using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Lykke.Job.BlockchainRiskControl.Domain;
using Lykke.Job.BlockchainRiskControl.Domain.RiskConstraintsConfiguration;
using Lykke.Job.BlockchainRiskControl.Domain.Services;

namespace Lykke.Job.BlockchainRiskControl.DomainServices
{
    public sealed class RiskConstraintsFactory : IRiskConstraintsFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Assembly[] _additionalAssemblies;

        private readonly Type _constraintInterfaceType;
        private readonly IReadOnlyDictionary<string, Type> _constraintImplementationTypes;

        public RiskConstraintsFactory(
            IServiceProvider serviceProvider,
            params Assembly[] additionalAssemblies)
        {
            _serviceProvider = serviceProvider;
            _additionalAssemblies = additionalAssemblies;

            _constraintInterfaceType = typeof(IRiskConstraint);
            _constraintImplementationTypes = GetConstraintImplementationTypes();
        }

        public IRiskConstraint Create(RiskConstraintConfiguration configuration)
        {
            if (!_constraintImplementationTypes.TryGetValue(configuration.ConstraintName, out var constraintType))
            {
                throw new InvalidOperationException(
                    $"Implementation of risk constraint [{configuration.ConstraintName}] is not found. " +
                    $"Risk constraint implementation should implement {_constraintInterfaceType.FullName}, be placed in the same assembly, " +
                    "be not abstract, not generic, public class with single public constructor without out parameters");
            }

            var constructor = constraintType.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Single();
            var parameters = GetConstructorParameters(configuration, constructor);
            var constraint = constructor.Invoke(parameters);

            return (IRiskConstraint) constraint;
        }

        private IReadOnlyDictionary<string, Type> GetConstraintImplementationTypes()
        {
            var result = new Dictionary<string, Type>();
            var typesToSearch = _additionalAssemblies
                .Prepend(_constraintInterfaceType.Assembly)
                .SelectMany(a => a.GetExportedTypes());

            foreach (var type in typesToSearch)
            {
                if (type.IsAbstract || type.IsGenericType || !_constraintInterfaceType.IsAssignableFrom(type))
                {
                    continue;
                }

                var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

                if (constructors.Length != 1)
                {
                    throw new InvalidOperationException(
                        $"{type.FullName} has {constructors.Length} public constructors, but risk constraint implementation " + 
                        "should has exactly one public constructor.");
                }

                var constructorParameters = constructors.Single().GetParameters();

                if (constructorParameters.Any(p => p.IsOut))
                {
                    throw new InvalidOperationException(
                        $"{type.FullName} has public constructor with out parameters, but risk constraint implementation " + 
                        "should has public constructor without out parameters.");
                }

                var displayName = type.GetDisplayName();

                if (result.TryGetValue(displayName, out var alreadyRegisteredType))
                {
                    throw new InvalidOperationException(
                        $"Ambiguous risk constraint name found {displayName}. " +
                        $"Types {type.FullName} and {alreadyRegisteredType.FullName} both have the same risk constraint name.");
                }

                result.Add(displayName, type);
            }

            return result;
        }

        private object[] GetConstructorParameters(
            RiskConstraintConfiguration configuration,
            ConstructorInfo constructor)
        {
            var parameters = constructor
                .GetParameters()
                .OrderBy(p => p.Position)
                .ToArray();
            var arguments = new object[parameters.Length];

            for(var i = 0; i < parameters.Length; ++i)
            {
                var parameter = parameters[i];

                object argument;

                if (configuration.Parameters.TryGetValue(parameter.Name, out var argumentStringValue))
                {
                    var converter = TypeDescriptor.GetConverter(parameter.ParameterType);
                    
                    argument = converter.ConvertFromInvariantString(argumentStringValue);
                }
                else
                {
                    argument = _serviceProvider.GetService(parameter.ParameterType);
                }

                arguments[i] = argument;
            }

            return arguments;
        }
    }
}
