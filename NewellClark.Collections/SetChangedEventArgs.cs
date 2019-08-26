﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewellClark.DataStructures.Collections
{
	/// <summary>
	/// Contains information about a set changed event.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SetChangedEventArgs<T> : EventArgs
	{
		/// <summary>
		/// Creates a new instance of <see cref="SetChangedEventArgs{T}"/>.
		/// </summary>
		/// <param name="added">The items that were added.</param>
		/// <param name="removed">The items that were removed.</param>
		public SetChangedEventArgs(IReadOnlyCollection<T> added, IReadOnlyCollection<T> removed)
		{
			if (added is null) throw new ArgumentNullException(nameof(added));
			if (removed is null) throw new ArgumentNullException(nameof(removed));

			this.Added = added;
			this.Removed = removed;
		}

		/// <summary>
		/// Gets the items that were added.
		/// </summary>
		public IReadOnlyCollection<T> Added { get; }

		/// <summary>
		/// Gets the items that were removed.
		/// </summary>
		public IReadOnlyCollection<T> Removed { get; }
	}
}
