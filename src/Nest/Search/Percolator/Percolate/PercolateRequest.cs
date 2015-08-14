﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Elasticsearch.Net;
using Newtonsoft.Json;

namespace Nest
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IPercolateRequest<TDocument> : IIndexTypePath<PercolateRequestParameters>, IPercolateOperation
		where TDocument : class
	{
		[JsonProperty(PropertyName = "doc")]
		TDocument Document { get; set; }

	}

	internal static class PercolatePathInfo
	{
		public static void Update<T>(ElasticsearchPathInfo<PercolateRequestParameters> pathInfo, IPercolateRequest<T> request)
			where T : class
		{
			pathInfo.Id = request.Id;
			pathInfo.HttpMethod = HttpMethod.POST;
		}
	}

	public partial class PercolateRequest<TDocument> : IndexTypePathBase<PercolateRequestParameters, TDocument>, IPercolateRequest<TDocument>
		where TDocument : class
	{
		public IHighlightRequest Highlight { get; set; }
		public QueryContainer Query { get; set; }
		public QueryContainer Filter { get; set; }
		public IDictionary<string, IAggregationContainer> Aggregations { get; set; }
		

		public string Id { get; set; }
		public int? Size { get; set; }
		public bool? TrackScores { get; set; }
		public TDocument Document { get; set; }
		public IDictionary<FieldName, ISort> Sort { get; set; }

		public PercolateRequest(TDocument document)
		{
			this.Document = document;
		}

		public PercolateRequest(string id) { this.Id = id; }

		public PercolateRequest(long id) { this.Id = id.ToString(CultureInfo.InvariantCulture); }
		
		IRequestParameters IPercolateOperation.GetRequestParameters()
		{
			return this.Request.RequestParameters;
		}
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<PercolateRequestParameters> pathInfo)
		{
			PercolatePathInfo.Update(pathInfo, this);
		}

	}
	public partial class PercolateDescriptor<T> : IndexTypePathDescriptor<PercolateDescriptor<T>, PercolateRequestParameters, T>, IPercolateRequest<T>
		where T : class
	{
		private IPercolateRequest<T> Self => this;

		IHighlightRequest IPercolateOperation.Highlight { get; set; }
		QueryContainer IPercolateOperation.Query { get; set; }
		QueryContainer IPercolateOperation.Filter { get; set; }

		string IPercolateOperation.Id { get; set; }
		int? IPercolateOperation.Size { get; set; }
		bool? IPercolateOperation.TrackScores { get; set; }
		
		T IPercolateRequest<T>.Document { get; set; }


		IDictionary<FieldName, ISort> IPercolateOperation.Sort { get; set; }
		IDictionary<string, IAggregationContainer> IPercolateOperation.Aggregations { get; set; }
		
		IRequestParameters IPercolateOperation.GetRequestParameters()
		{
			return this.Request.RequestParameters;
		}
		/// <summary>
		/// The object to perculate
		/// </summary>
		public PercolateDescriptor<T> Document(T @object)
		{
			Self.Document = @object;
			return this;
		}

		/// <summary>
		/// The object to perculate
		/// </summary>
		public PercolateDescriptor<T> Id(string id)
		{
			Self.Id = id;
			return this;
		}

		/// <summary>
		/// The object to perculate
		/// </summary>
		public PercolateDescriptor<T> Id(long id)
		{
			Self.Id = id.ToString(CultureInfo.InvariantCulture);
			return this;
		}
		/// <summary>
		/// Make sure we keep calculating score even if we are sorting on a field.
		/// </summary>
		public PercolateDescriptor<T> TrackScores(bool trackscores = true)
		{
			Self.TrackScores = trackscores;
			return this;
		}

		public PercolateDescriptor<T> Aggregations(Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>> aggregationsSelector)
		{
			var aggs = aggregationsSelector(new AggregationContainerDescriptor<T>());
			if (aggs == null) return this;
			Self.Aggregations = ((IAggregationContainer)aggs).Aggregations;
			return this;
		}

		/// <summary>
		/// Allow to highlight search results on one or more fields. 
		/// </summary>
		public PercolateDescriptor<T> Highlight(int size, Func<HighlightDescriptor<T>,HighlightDescriptor<T>> highlightDescriptor)
		{
			highlightDescriptor.ThrowIfNull("highlightDescriptor");
			var d  = highlightDescriptor(new HighlightDescriptor<T>());
			Self.Size = size;
			Self.Highlight = d;
			return this;
		}

		public PercolateDescriptor<T> Size(int size)
		{
			Self.Size = size;
			return this;
		}

		/// <summary>
		/// <para>Allows to add one or more sort on specific fields. Each sort can be reversed as well.
		/// The sort is defined on a per field level, with special field name for _score to sort by score.
		/// </para>
		/// <para>
		/// Sort ascending.
		/// </para>
		/// </summary>
		public PercolateDescriptor<T> SortAscending(Expression<Func<T, object>> objectPath)
		{
			if (Self.Sort == null) Self.Sort = new Dictionary<FieldName, ISort>();

			Self.Sort.Add(objectPath, new Sort() { Order = SortOrder.Ascending });
			return this;
		}

		/// <summary>
		/// <para>Allows to add one or more sort on specific fields. Each sort can be reversed as well.
		/// The sort is defined on a per field level, with special field name for _score to sort by score.
		/// </para>
		/// <para>
		/// Sort descending.
		/// </para>
		/// </summary>
		public PercolateDescriptor<T> SortDescending(Expression<Func<T, object>> objectPath)
		{
			if (Self.Sort == null) Self.Sort = new Dictionary<FieldName, ISort>();

			Self.Sort.Add(objectPath, new Sort() { Order = SortOrder.Descending });
			return this;
		}

		/// <summary>
		/// <para>Allows to add one or more sort on specific fields. Each sort can be reversed as well.
		/// The sort is defined on a per field level, with special field name for _score to sort by score.
		/// </para>
		/// <para>
		/// Sort ascending.
		/// </para>
		/// </summary>
		public PercolateDescriptor<T> SortAscending(string field)
		{
			if (Self.Sort == null) Self.Sort = new Dictionary<FieldName, ISort>();
			Self.Sort.Add(field, new Sort() { Order = SortOrder.Ascending });
			return this;
		}

		/// <summary>
		/// <para>Allows to add one or more sort on specific fields. Each sort can be reversed as well.
		/// The sort is defined on a per field level, with special field name for _score to sort by score.
		/// </para>
		/// <para>
		/// Sort descending.
		/// </para>
		/// </summary>
		public PercolateDescriptor<T> SortDescending(string field)
		{
			if (Self.Sort == null)
				Self.Sort = new Dictionary<FieldName, ISort>();

			Self.Sort.Add(field, new Sort() { Order = SortOrder.Descending });
			return this;
		}

		/// <summary>
		/// <para>Sort() allows you to fully describe your sort unlike the SortAscending and SortDescending aliases.
		/// </para>
		/// </summary>
		public PercolateDescriptor<T> Sort(Func<SortFieldDescriptor<T>, IFieldSort> sortSelector)
		{
			if (Self.Sort == null)
				Self.Sort = new Dictionary<FieldName, ISort>();

			sortSelector.ThrowIfNull("sortSelector");
			var descriptor = sortSelector(new SortFieldDescriptor<T>());
			Self.Sort.Add(descriptor.Field, descriptor);
			return this;
		}

		/// <summary>
		/// <para>SortGeoDistance() allows you to sort by a distance from a geo point.
		/// </para>
		/// </summary>
		public PercolateDescriptor<T> SortGeoDistance(Func<SortGeoDistanceDescriptor<T>, IGeoDistanceSort> sortSelector)
		{
			if (Self.Sort == null)
				Self.Sort = new Dictionary<FieldName, ISort>();

			sortSelector.ThrowIfNull("sortSelector");
			var descriptor = sortSelector(new SortGeoDistanceDescriptor<T>());
			Self.Sort.Add("_geo_distance", descriptor);
			return this;
		}

		/// <summary>
		/// <para>SortScript() allows you to sort by a distance from a geo point.
		/// </para>
		/// </summary>
		public PercolateDescriptor<T> SortScript(Func<SortScriptDescriptor<T>, IScriptSort> sortSelector)
		{
			if (Self.Sort == null)
				Self.Sort = new Dictionary<FieldName, ISort>();

			sortSelector.ThrowIfNull("sortSelector");
			var descriptor = sortSelector(new SortScriptDescriptor<T>());
			Self.Sort.Add("_script", descriptor);
			return this;
		}

		/// <summary>
		/// Describe the query to perform using a query descriptor lambda
		/// </summary>
		public PercolateDescriptor<T> Query(Func<QueryContainerDescriptor<T>, QueryContainer> query)
		{
			query.ThrowIfNull("query");
			var q = new QueryContainerDescriptor<T>();
			var bq = query(q);
			return this.Query(bq);
		}

		public PercolateDescriptor<T> Query(QueryContainer query)
		{
			if (query == null)
				return this;

			if (query.IsConditionless)
				return this;
			Self.Query = query;
			return this;

		}

		/// <summary>
		/// Shortcut to .Query(q=>q.QueryString(qs=>qs.Query("string"))
		/// Does a match_all if the userInput string is null or empty;
		/// </summary>
		public PercolateDescriptor<T> QueryString(string userInput)
		{
			var q = new QueryContainerDescriptor<T>();
			QueryContainer bq;
			if (userInput.IsNullOrEmpty())
				bq = q.MatchAll();
			else
				bq = q.QueryString(qs => qs.Query(userInput));
			Self.Query = bq;
			return this;
		}

		/// <summary>
		/// Filter search using a filter descriptor lambda
		/// </summary>
		public PercolateDescriptor<T> Filter(Func<QueryContainerDescriptor<T>, QueryContainer> filter)
		{
			filter.ThrowIfNull("filter");
			var f = new QueryContainerDescriptor<T>();

			var bf = filter(f);
			if (bf == null)
				return this;
			if (bf.IsConditionless)
				return this;


			Self.Filter = bf;
			return this;
		}
		/// <summary>
		/// Filter search
		/// </summary>
		public PercolateDescriptor<T> Filter(QueryContainer QueryDescriptor)
		{
			QueryDescriptor.ThrowIfNull("filter");
			Self.Filter = QueryDescriptor;
			return this;
		}


		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<PercolateRequestParameters> pathInfo)
		{
			PercolatePathInfo.Update(pathInfo, this);
		}
	}
}