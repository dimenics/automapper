using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace AutoMapper
{
    /// <summary>
    /// Scanner that looks for all implementations of the <seealso cref="IAutoMapper"/> interface
    /// and consequently initializes the mappings
    /// </summary>
    [Obsolete("Switch to Automapper 9.x.x and use AutoMapper.Profile instead of IAutoMapper")]
    public static class AutoMapperFactory
    {
        /// <summary>
        /// Scans the assemblies of the AppDomain for implementations of the <seealso cref="IAutoMapper"/> interface
        /// and includes them in the construction of the AutoMapper.
        /// </summary>
        /// <returns>Instance of IMapper</returns>
        public static IMapper Create(bool eagerLoadAssemblies = false)
        {
            try
            {
                // Get the expressions
                Action<IMapperConfigurationExpression> action = eagerLoadAssemblies ? ScanAllAssemblies(new List<string>()) : ScanAssemblies(new List<string>());

                // Initialize auto mapper
                MapperConfiguration mapper = new MapperConfiguration(action);
                return mapper.CreateMapper();
            }
            catch (ReflectionTypeLoadException loaderException)
            {
                throw MapperExceptionFactory.Throw(loaderException);
            }
        }

        /// <summary>
        /// Scans the assemblies of the AppDomain for implementations of the <seealso cref="IAutoMapper"/> interface
        /// and includes them in the construction of the AutoMapper.
        /// </summary>
        /// <returns></returns>
        public static void Initialize(bool eagerLoadAssemblies = false)
            => Initialize(eagerLoadAssemblies, new List<string>());

        /// <summary>
        /// Scans the assemblies of the AppDomain for implementations of the <seealso cref="IAutoMapper"/> interface
        /// and includes them in the construction of the AutoMapper.
        /// </summary>
        /// <returns></returns>
        public static void Initialize(bool eagerLoadAssemblies, IEnumerable<string> assembliesToIgnore)
        {
            Action<IMapperConfigurationExpression> mappingConfiguration = eagerLoadAssemblies ? ScanAllAssemblies(assembliesToIgnore) : ScanAssemblies(assembliesToIgnore);
            if (mappingConfiguration != default(Action<IMapperConfigurationExpression>))
                Mapper.Initialize(mappingConfiguration);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private static Action<IMapperConfigurationExpression> ScanAssemblies(IEnumerable<string> assembliesToIgnore)
        {
            // The IAutoMapper interface is what we're looking for in the assemblies
            Type iMapperType = typeof(IAutoMapper);
            IEnumerable<Type> mapperClasses = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x =>
                {
                    string assemblyName = x.GetName().Name;
                    return assembliesToIgnore.All(y => !assemblyName.Contains(y));
                })
                .SelectMany(x => x.GetLoadableTypes()
                .Where(y => iMapperType.IsAssignableFrom(y) && !y.IsAbstract && y.IsClass))
                .ToList();

            // For each implementation of the interface we invoke the Configure method - which returns an Action<IMapperConfigurationExpression>
            List<Action<IMapperConfigurationExpression>> configurators = new List<Action<IMapperConfigurationExpression>>();
            foreach (Type type in mapperClasses)
            {
                object mapperObject = Activator.CreateInstance(type);
                MethodInfo method = type.GetMethod(nameof(IAutoMapper.Configure), BindingFlags.Public | BindingFlags.Instance);
                if (method == null) 
                    continue;

                if (method.Invoke(mapperObject, null) is Action<IMapperConfigurationExpression> returnValue)
                    configurators.Add(returnValue);
            }

            // Concatenate the expressions
            Action<IMapperConfigurationExpression> action = null;
            foreach (Action<IMapperConfigurationExpression> configurator in configurators)
                action += configurator;

            return action;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private static Action<IMapperConfigurationExpression> ScanAllAssemblies(IEnumerable<string> assembliesToIgnore)
        {
            Type iMapperType = typeof(IAutoMapper);
            IEnumerable<Type> mapperClasses = GetAllAssemblies(assembliesToIgnore).SelectMany(x => x.GetTypes()
                .Where(y => iMapperType.IsAssignableFrom(y) && !y.IsAbstract && y.IsClass));

            List<Action<IMapperConfigurationExpression>> configurators = new List<Action<IMapperConfigurationExpression>>();
            foreach (Type type in mapperClasses)
            {
                object mapperObject = Activator.CreateInstance(type);
                MethodInfo method = type.GetMethod(nameof(IAutoMapper.Configure), BindingFlags.Public | BindingFlags.Instance);
                if (method == null)
                    continue;

                if (method.Invoke(mapperObject, null) is Action<IMapperConfigurationExpression> returnValue)
                    configurators.Add(returnValue);
            }

            return configurators.Aggregate<Action<IMapperConfigurationExpression>, Action<IMapperConfigurationExpression>>(null, (current, singleAction) => current + singleAction);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Assembly> GetAllAssemblies(IEnumerable<string> assembliesToIgnore)
        {
            // Start with the current domain's assembly
            IEnumerable<Assembly> currentDomainAssemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x =>
                {
                    string assemblyName = x.GetName().Name;
                    return assembliesToIgnore.All(y => !assemblyName.Contains(y));
                })
                .ToList();

            foreach (Assembly assembly in currentDomainAssemblies)
            {
                // Fetch the referenced assemblies for the current assembly
                foreach (Assembly referencedAssembly in GetAllAssembliesEagerly(assembly))
                    yield return referencedAssembly;

                // Include this assembly in the result set
                yield return assembly;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static IEnumerable<Assembly> GetAllAssembliesEagerly(Assembly assembly)
        {
            AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies() ?? new AssemblyName[0];
            foreach (AssemblyName referencedAssembly in referencedAssemblies)
            {
                Assembly loadedAssembly = null;
                try
                {
                    loadedAssembly = Assembly.Load(referencedAssembly);
                    GetAllAssembliesEagerly(loadedAssembly);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error when loading assembly: " + referencedAssembly.Name + Environment.NewLine + ex);
                }

                if (loadedAssembly != null)
                    yield return loadedAssembly;
            }
        }
    }
}