using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Text;

namespace Enyim.Reflection
{
	/// <summary>
	/// <para>Implements a very fast object factory for dynamic object creation. Dynamically generates a factory class whihc will use the new() constructor of the requested type.</para>
	/// <para>Much faster than using Activator at the price of the first invocation being significantly slower than subsequent calls.</para>
	/// <remarks>Only supports parameterless constructors.</remarks>
	/// </summary>
	public static class FastActivator
	{
		private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(FastActivator));

		private static object SyncObj = new Object();
		private static ModuleBuilder factoryModule;
		private static Dictionary<Type, IFastObjectFacory> factoryCache = new Dictionary<Type, IFastObjectFacory>();
		private const MethodAttributes DefaultMethodAttrs = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.SpecialName | MethodAttributes.Final;
		private static long counter = 1;

		/// <summary>
		/// Creates an instance of the specified type using a generated factory to avoid using Reflection.
		/// </summary>
		/// <param name="type">The type to be created</param>
		/// <returns>The newly created instance.</returns>
		public static object CreateInstance(Type type)
		{
			if (type.IsNotPublic)
			{
				log.InfoFormat("Type {0} is not public. Falling back to Activator.", type);

				return Activator.CreateInstance(type);
			}

			return GetFactory(type).CreateInstance();
		}

		/// <summary>
		/// Creates an instance of the type designated by the specified generic type parameter using a generated factory to avoid using Reflection.
		/// </summary>
		/// <typeparam name="T">The type to be created.</typeparam>
		/// <returns>The newly created instance.</returns>
		public static T CreateInstance<T>() where T : class
		{
			Type type = typeof(T);

			if (!type.IsPublic)
			{
				log.InfoFormat("Type {0} is not public. Falling back to Activator.", type);

				return Activator.CreateInstance<T>();
			}

			return (T)GetFactory(type).CreateInstance();
		}

		private static IFastObjectFacory GetFactory(Type type)
		{
			IFastObjectFacory retval;

			if (!factoryCache.TryGetValue(type, out retval))
			{
				lock (factoryCache)
				{
					if (!factoryCache.TryGetValue(type, out retval))
					{
						retval = CreateFactory(type);

						Thread.MemoryBarrier();

						factoryCache.Add(type, retval);
					}
				}
			}

			return retval;
		}

		private static IFastObjectFacory CreateFactory(Type type)
		{
			if (factoryModule == null)
			{
				lock (SyncObj)
				{
					if (factoryModule == null)
					{
						AppDomain domain = Thread.GetDomain();
						AssemblyName name = new AssemblyName();

						string assemblyName = Path.GetRandomFileName();
						name.Name = assemblyName;
						name.Version = new Version(2, 0, 0, 0);
						name.Flags = AssemblyNameFlags.EnableJITcompileOptimizer;

						AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
						assemblyBuilder.DefineVersionInfoResource();

						Thread.MemoryBarrier();

						factoryModule = assemblyBuilder.DefineDynamicModule(Path.GetRandomFileName());
					}
				}
			}

			return (IFastObjectFacory)Activator.CreateInstance(EmitFactory(factoryModule, type));
		}

		private static class TypeOf
		{
			public static Type IFastObjectFacory = typeof(IFastObjectFacory);
			public static Type Object = typeof(object);
		}

		private static string MakeSafeName(Type type)
		{
			StringBuilder retval = new StringBuilder();

			// TODO chaneg this to return a guid or random string when it's been fully tested
			retval.Append("generated.F_");
			retval.Append(type.FullName);
			retval.Replace('+', '_');
			retval.Replace('.', '_');

			retval.Append('_').Append(counter++);

			return retval.ToString();
		}

		private static Type EmitFactory(ModuleBuilder module, Type type)
		{
			//  public class generated.F_REQUESTED_TYPE : IFastObjectFactory
			//  {
			//      object IFastObjectFacory.CreateInstance()
			//      {
			//          return new REQUESTED_TYPE()
			//      }
			//  }
			//

			// get the .ctor() of the type
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor == null)
				throw new InvalidOperationException(String.Format("The type {0} must have a .ctor().", type.FullName));

			TypeBuilder typeBuilder = module.DefineType(MakeSafeName(type), TypeAttributes.Public | TypeAttributes.Class);
			typeBuilder.AddInterfaceImplementation(TypeOf.IFastObjectFacory);
			typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

			// object IFastObjectFacory.Create()
			MethodBuilder methodBuilder = typeBuilder.DefineMethod(TypeOf.IFastObjectFacory.FullName + ".CreateInstance", DefaultMethodAttrs, TypeOf.Object, Type.EmptyTypes);

			ILGenerator ilgen = methodBuilder.GetILGenerator();

			// implement the interface member
			typeBuilder.DefineMethodOverride(methodBuilder, TypeOf.IFastObjectFacory.GetMethod("CreateInstance", Type.EmptyTypes));
			ilgen.Emit(OpCodes.Newobj, constructor);
			ilgen.Emit(OpCodes.Ret);

			return typeBuilder.CreateType();
		}

	}
}

#region [ License information          ]
/* ************************************************************
 *
 * Copyright (c) Attila Kiskó, enyim.com, 2007
 *
 * This source code is subject to terms and conditions of 
 * Microsoft Permissive License (Ms-PL).
 * 
 * A copy of the license can be found in the License.html
 * file at the root of this distribution. If you can not 
 * locate the License, please send an email to a@enyim.com
 * 
 * By using this source code in any fashion, you are 
 * agreeing to be bound by the terms of the Microsoft 
 * Permissive License.
 *
 * You must not remove this notice, or any other, from this
 * software.
 *
 * ************************************************************/
#endregion