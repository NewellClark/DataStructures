﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewellClark.DataStructures.Graphs
{
	public static class Path
	{
		/// <summary>
		/// Creates a new empty <see cref="Path{TNode, TCost}"/> with the specified cost functions.
		/// </summary>
		/// <typeparam name="TNode">The type of node in the path.</typeparam>
		/// <typeparam name="TCost">The type used to compute the cost of the path.</typeparam>
		/// <param name="costFunction">A function to compute the cost of moving between two nodes.</param>
		/// <param name="costAdder">A function to add two costs together.</param>
		/// <param name="initial">The initial cost of an empty path.</param>
		/// <returns>
		/// A new empty <see cref="Path{TNode, TCost}"/> that uses the specified <see cref="CostFunction{TNode, TCost}"/> and
		/// <see cref="CostAdder{TCost}"/> to compute the cost of traversal, and that has the specified initial cost.
		/// </returns>
		public static Path<TNode, TCost> Create<TNode, TCost>(
			CostFunction<TNode, TCost> costFunction, 
			CostAdder<TCost> costAdder, 
			TCost initial)
		{
			if (costFunction is null) throw new ArgumentNullException(nameof(costFunction));
			if (costAdder is null) throw new ArgumentNullException(nameof(costAdder));

			return new Path<TNode, TCost>(costFunction, costAdder, initial);
		}
	}

	/// <summary>
	/// An immutable path of nodes that calculates the total cost of traversal.
	/// </summary>
	/// <typeparam name="TNode">The type of each node in the path.</typeparam>
	/// <typeparam name="TCost">The type used to compute the cost of traversing the path.</typeparam>
	public class Path<TNode, TCost> 
	{
		private readonly CommonMembers _common;
		private readonly ImmutableStack<Path<TNode, TCost>> _stack;
		private readonly TNode _head;

		//	Empty path constructor.
		internal Path(CostFunction<TNode, TCost> costFunction, CostAdder<TCost> costAdder, TCost initial)
		{
			Debug.Assert(costFunction != null);
			Debug.Assert(costAdder != null);

			_common = new CommonMembers(costFunction, costAdder);
			_stack = ImmutableStack<Path<TNode, TCost>>.Empty;
			Cost = initial;
		}

		//	Push constructor.
		internal Path(Path<TNode, TCost> tail, TNode head)
		{
			Debug.Assert(tail != null);

			_common = tail._common;
			_stack = tail._stack.Push(tail);
			_head = head;

			if (tail.IsEmpty)
				Cost = tail.Cost;
			else
			{
				TCost newSegmentCost = _common.CalculateCost(tail._head, head);
				Cost = _common.AddCosts(tail.Cost, newSegmentCost);
			}
		}

		/// <summary>
		/// Gets the total cost of traversing the path from start to finish.
		/// </summary>
		public TCost Cost { get; }

		/// <summary>
		/// Indicates whether the current <see cref="Path{TNode, TCost}"/> is empty.
		/// </summary>
		public bool IsEmpty => _stack.IsEmpty;

		/// <summary>
		/// Gets the node at the head of the path without removing it.
		/// </summary>
		public TNode Peek()
		{
			ThrowIfEmpty();

			return _head;
		}

		/// <summary>
		/// Creates a new path by removing the node from the head of the current path. The current path is 
		/// not modified.
		/// </summary>
		/// <returns>
		/// A new path created by removing the head from the current path.
		/// </returns>
		public Path<TNode, TCost> Pop()
		{
			ThrowIfEmpty();

			return _stack.Peek();
		}

		/// <summary>
		/// Creates a new path by pushing the specified node to the end of the current path. The current path instance
		/// is not modified.
		/// </summary>
		/// <param name="node">The node to pushed to the end of the current path.</param>
		/// <returns>A new path created by adding a new node to the end of the current path.</returns>
		public Path<TNode, TCost> Push(TNode node)
		{
			return new Path<TNode, TCost>(this, node);
		}

		/// <summary>
		/// Gets a sequence of all the nodes in the current <see cref="Path{TNode, TCost}"/>, starting with the 
		/// head node.
		/// </summary>
		public IEnumerable<TNode> Nodes => throw new NotImplementedException();

		/// <summary>
		/// Contains members that are the same for all sub-paths in the current <see cref="Path{TNode, TCost}"/>.
		/// </summary>
		private class CommonMembers
		{
			private readonly CostFunction<TNode, TCost> _costFunction;
			private readonly CostAdder<TCost> _costAdder;

			public CommonMembers(CostFunction<TNode, TCost> costFunction, CostAdder<TCost> costAdder)
			{
				Debug.Assert(costFunction != null);
				Debug.Assert(costAdder != null);

				_costFunction = costFunction;
				_costAdder = costAdder;
			}

			public TCost CalculateCost(TNode left, TNode right) => _costFunction(left, right);

			public TCost AddCosts(TCost left, TCost right) => _costAdder(left, right);
		}

		private void ThrowIfEmpty()
		{
			if (IsEmpty)
				throw new InvalidOperationException($"The {GetType().Name} is empty.");
		}
	}
}
