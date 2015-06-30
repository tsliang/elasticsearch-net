﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nest.Resolvers.Converters;
using Newtonsoft.Json;

namespace Nest
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeConverter<SpanFirstQueryDescriptor<object>>))]
	public interface ISpanFirstQuery : ISpanSubQuery
	{
		[JsonProperty(PropertyName = "match")]
		ISpanQuery Match { get; set; }

		[JsonProperty(PropertyName = "end")]
		int? End { get; set; }
        [JsonProperty(PropertyName = "boost")]
        double? Boost { get; set; }
	}

	public class SpanFirstQuery : Query, ISpanFirstQuery
	{
		bool IQuery.Conditionless => IsConditionless(this);
		public ISpanQuery Match { get; set; }
		public int? End { get; set; }
	    public double? Boost { get; set; }

		protected override void WrapInContainer(IQueryContainer c) => c.SpanFirst = this;
		internal static bool IsConditionless(ISpanFirstQuery q) => q.Match == null || q.Match.Conditionless;
	}

	public class SpanFirstQueryDescriptor<T> : ISpanFirstQuery where T : class
	{
		private ISpanFirstQuery Self { get { return this; }}
		string IQuery.Name { get; set; }
		bool IQuery.Conditionless => SpanFirstQuery.IsConditionless(this);	
		ISpanQuery ISpanFirstQuery.Match { get; set; }
		int? ISpanFirstQuery.End { get; set; }
        double? ISpanFirstQuery.Boost { get; set; }

		public SpanFirstQueryDescriptor<T> Name(string name)
		{
			Self.Name = name;
			return this;
		}

		public SpanFirstQueryDescriptor<T> MatchTerm(Expression<Func<T, object>> fieldDescriptor
			, string value
			, double? Boost = null)
		{
			var span = new SpanQuery<T>();
			span = span.SpanTerm(fieldDescriptor, value, Boost);
			Self.Match = span;
			return this;
		}

		public SpanFirstQueryDescriptor<T> MatchTerm(string field, string value, double? Boost = null)
		{
			var span = new SpanQuery<T>();
			span = span.SpanTerm(field, value, Boost);
			Self.Match = span;
			return this;
		}

		public SpanFirstQueryDescriptor<T> Match(Func<SpanQuery<T>, SpanQuery<T>> selector)
		{
			selector.ThrowIfNull("selector");
			Self.Match = selector(new SpanQuery<T>());
			return this;
		}

		public SpanFirstQueryDescriptor<T> End(int end)
		{
			Self.End = end;
			return this;
		}

        public SpanFirstQueryDescriptor<T> Boost(double boost)
        {
            Self.Boost = boost;
            return this;
        }
	}
}
