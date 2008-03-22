using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Collections.ObjectModel;

namespace Enyim.Caching.Configuration
{
	public sealed class EndPointElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new EndPointElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			EndPointElement ep = (EndPointElement)element;

			return String.Concat(ep.Address, ":", ep.Port.ToString(CultureInfo.InvariantCulture));
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		internal IList<IPEndPoint> ToIPEndPointCollection()
		{
			List<IPEndPoint> retval = new List<IPEndPoint>(this.Count);
			foreach (EndPointElement e in this)
			{
				retval.Add(e.EndPoint);
			}

			return retval.AsReadOnly();
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