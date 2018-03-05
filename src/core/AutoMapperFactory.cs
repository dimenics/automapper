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
                Action<IMapperConfigurationExpression> action = eagerLoadAssemblies ? ScanAllAssemblies() : ScanAssemblies();

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
        {
            Action<IMapperConfigurationExpression> mappingConfiguration = eagerLoadAssemblies ? ScanAllAssemblies() : ScanAssemblies();
            if (mappingConfiguration != default(Action<IMapperConfigurationExpression>))
                Mapper.Initialize(mappingConfiguration);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private static Action<IMapperConfigurationExpression> ScanAssemblies()
        {
            // The IAutoMapper interface is what we're looking for in the assemblies
            Type iMapperType = typeof(IAutoMapper);
            IEnumerable<Type> mapperClasses = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetLoadableTypes()
                .Where(y => iMapperType.IsAssignableFrom(y) && !y.IsAbstract && y.IsClass));

            // For each implementation of the interface we invoke the Configure method - which returns an Action<IMapperConfigurationExpression>
            List<Action<IMapperConfigurationExpression>> configurators = new List<Action<IMapperConfigurationExpression>>();
            foreach (Type type in mapperClasses)
            {
                object mapperObject = Activator.CreateInstance(type);
                MethodInfo method = type.GetMethod(nameof(IAutoMapper.Configure), BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    if (method.Invoke(mapperObject, null) is Action<IMapperConfigurationExpression> returnValue)
                        configurators.Add(returnValue);
                }
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
        private static Action<IMapperConfigurationExpression> ScanAllAssemblies()
        {
            Type iMapperType = typeof(IAutoMapper);
            IEnumerable<Type> mapperClasses = GetAllAssemblies().SelectMany(x => x.GetTypes()
                .Where(y => iMapperType.IsAssignableFrom(y) && !y.IsAbstract && y.IsClass));

            List<Action<IMapperConfigurationExpression>> configurators = new List<Action<IMapperConfigurationExpression>>();
            foreach (Type type in mapperClasses)
            {
                object mapperObject = Activator.CreateInstance(type);
                MethodInfo method = type.GetMethod(nameof(IAutoMapper.Configure), BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    if (method.Invoke(mapperObject, null) is Action<IMapperConfigurationExpression> returnValue)
                        configurators.Add(returnValue);
                }
            }

            Action<IMapperConfigurationExpression> action = null;
            foreach (Action<IMapperConfigurationExpression> singleAction in configurators)
                action += singleAction;

            return action;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Assembly> GetAllAssemblies()
        {
            // Start with the current domain's assembly
            Assembly[] currentDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
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