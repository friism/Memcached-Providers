using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using System.ComponentModel;
using Enyim.Caching;
using System.Collections.ObjectModel;
using System.Net;

namespace Enyim.Caching.Configuration
{
	public sealed class MemcachedClientSection : ConfigurationSection, IMemcachedClientConfiguration
	{
		[ConfigurationProperty("servers", IsRequired = true)]
		public EndPointElementCollection Servers
		{
			get { return (EndPointElementCollection)base["servers"]; }
		}

		[ConfigurationProperty("socketPool", IsRequired = false)]
		public SocketPoolElement SocketPool
		{
			get { return (SocketPoolElement)base["socketPool"]; }
			set { base["socketPool"] = value; }
		}

		[ConfigurationProperty("keyTransformer", IsRequired = false), TypeConverter(typeof(TypeNameConverter)), InterfaceValidator(typeof(IKeyTransformer))]
		public Type KeyTransformer
		{
			get { return (Type)base["keyTransformer"]; }
			set { base["keyTransformer"] = value; }
		}

		[ConfigurationProperty("nodeLocator", IsRequired = false), TypeConverter(typeof(TypeNameConverter)), InterfaceValidator(typeof(Enyim.Caching.Memcached.IMemcachedNodeLocator))]
		public Type NodeLocator
		{
			get { return (Type)base["nodeLocator"]; }
			set { base["nodeLocator"] = value; }
		}

		[ConfigurationProperty("transcoder", IsRequired = false), TypeConverter(typeof(TypeNameConverter)), InterfaceValidator(typeof(Enyim.Caching.Memcached.ITranscoder))]
		public Type Transcoder
		{
			get { return (Type)base["transcoder"]; }
			set { base["transcoder"] = value; }
		}

		protected override void PostDeserialize()
		{
			WebContext hostingContext = base.EvaluationContext.HostingContext as WebContext;

			if (hostingContext != null && hostingContext.ApplicationLevel == WebApplicationLevel.BelowApplication)
			{
				throw new InvalidOperationException("The " + this.SectionInformation.SectionName + " section cannot be defined below the application level.");
			}
		}

		#region [ IMemcachedClientConfiguration]
		IList<IPEndPoint> IMemcachedClientConfiguration.Servers
		{
			get { return this.Servers.ToIPEndPointCollection(); }
		}

		ISocketPoolConfiguration IMemcachedClientConfiguration.SocketPool
		{
			get { return this.SocketPool; }
		}

		Type IMemcachedClientConfiguration.KeyTransformer
		{
			get { return this.KeyTransformer; }
			set { this.KeyTransformer = value; }
		}

		Type IMemcachedClientConfiguration.NodeLocator
		{
			get { return this.NodeLocator; }
			set { this.NodeLocator = value; }
		}

		Type IMemcachedClientConfiguration.Transcoder
		{
			get { return this.Transcoder; }
			set { this.Transcoder = value; }
		}
		#endregion
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