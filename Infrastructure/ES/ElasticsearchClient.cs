using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Infrastructure.ES;
using Domain.DI;

namespace Infrastructure.ES
{
  public class ElasticsearchClient : IElasticsearchClient, IScopedDependency
  {
    private ElasticClient _client;
    public ElasticClient Instace => _client ?? (_client = GetElasticClient());

    /// <summary>
    /// 获取客户端
    /// </summary>
    /// <param name="indexName"></param>
    /// <returns></returns>
    private ElasticClient GetElasticClient(string indexName = "personalblog")
    {
      var connectString = "http://127.0.0.1:9200";
      var nodesStr = connectString.Split('|');
      var nodes = nodesStr.Select(s => new Uri(s)).ToList();
      var connectionPool = new SniffingConnectionPool(nodes);
      var settings = new ConnectionSettings(connectionPool);
      if (!string.IsNullOrWhiteSpace(indexName))
        settings.DefaultIndex(indexName);
      var client = new ElasticClient(settings);
      return client;
    }

    /// <summary>
    /// 批量添加数据
    /// </summary>
    public async Task<IBulkResponse> Bulk<TDocument>(IEnumerable<TDocument> documents) where TDocument : class
    {
      BulkDescriptor buiDescriptor = new BulkDescriptor();
      buiDescriptor.CreateMany(documents);
      IBulkResponse response = await Instace.BulkAsync(buiDescriptor);
      return response;
    }
    /// <summary>
    /// 添加一条文档
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    /// <param name="document"></param>
    /// <returns></returns>
    public async Task<ICreateResponse> CreateDocument<TDocument>(TDocument document) where TDocument : class
    {
      var createResponse = await Instace.CreateAsync<TDocument>(document);
      return createResponse;
    }
    /// <summary>
    /// 更新所有字段
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    /// <param name="document"></param>
    /// <returns></returns>
    public async Task<IUpdateResponse<TDocument>> UpdateDocument<TDocument>(TDocument document) where TDocument : class
    {
      DocumentPath<TDocument> documentPath = new DocumentPath<TDocument>(document);
      var updateRequest = await Instace.UpdateAsync(documentPath,
        (p) => p.Doc(
          document
      ));
      return updateRequest;
    }
    /// <summary>
    /// 更新部分字段
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    /// <param name="document"></param>
    /// <returns></returns>
    public async Task<IUpdateResponse<TDocument>> UpdateDocumentPartial<TDocument, TPartialDocument>(TDocument document, object partialDocument)
      where TDocument : class where TPartialDocument : class
    {
      DocumentPath<TDocument> documentPath = new DocumentPath<TDocument>(document);
      IUpdateRequest<TDocument, object> updateRequest = new UpdateRequest<TDocument, object>(documentPath);
      updateRequest.Doc = partialDocument;
      var updateResponse = await Instace.UpdateAsync(updateRequest);
      return updateResponse;
    }
    /// <summary>
    /// 批量更新
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    /// <typeparam name="TPartialDocument"></typeparam>
    /// <param name="documents">需要更新的集合</param>
    /// <returns></returns>
    public async Task<IBulkResponse> BulkUpdateDocumentPartial<TDocument, TPartialDocument>(IEnumerable<TDocument> documents) where TDocument : class where TPartialDocument : class
    {
      BulkDescriptor bulkDescriptor = new BulkDescriptor();
      BulkUpdateDescriptor<TDocument, TPartialDocument> bulkUpdateDescriptor = new BulkUpdateDescriptor<TDocument, TPartialDocument>();
      var bulk = bulkDescriptor.UpdateMany<TDocument>(documents, (o, t) =>
      {
        return o.Doc(t);
      });
      return await Instace.BulkAsync(bulk);
    }
    /// <summary>
    /// 删除文档
    /// </summary>
    /// <param name="indexName"></param>
    /// <param name="typeName"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IDeleteResponse> DeleteDocument(string indexName, string typeName, string id)
    {
      DeleteRequest deleteRequest = new DeleteRequest(indexName, typeName, id);
      IDeleteResponse deleteResponse = await Instace.DeleteAsync(deleteRequest);
      return deleteResponse;
    }
    /// <summary>
    /// 删除文档
    /// </summary>
    /// <param name="indexName"></param>
    /// <param name="typeName"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IDeleteResponse> DeleteDocument<TDocument>(TDocument document) where TDocument : class
    {
      DocumentPath<TDocument> documentPath = new DocumentPath<TDocument>(document);
      //DeleteRequest deleteRequest = new DeleteRequest(indexName, typeName, id);
      IDeleteResponse deleteResponse = await Instace.DeleteAsync(documentPath);
      return deleteResponse;
    }
    /// <summary>
    /// 查询数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="searchFunc"></param>
    /// <returns></returns>
    public async Task<ISearchResponse<T>> Query<T>(Func<SearchDescriptor<T>, ISearchRequest> searchFunc) where T : class
    {      
      var searchResponse =await Instace.SearchAsync<T>(searchFunc);
      return searchResponse;
    }
    /// <summary>
    /// 高亮查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queryFunc"></param>
    /// <param name="highFieldFunc"></param>
    /// <param name="tag">标签</param>
    /// <param name="count">查询数</param>
    /// <returns></returns>
    public async Task<ISearchResponse<T>> HighQuery<T>(
      Func<QueryContainerDescriptor<T>, QueryContainer>[] queryFunc,
      string tag = "em", int count = 10, int from = 1,
      params Func<HighlightFieldDescriptor<T>, IHighlightField>[] highFieldFunc) where T : class
    {
      return await Instace.SearchAsync<T>(q => q
      .From((from-1)*count)
      .Size(count)

        .Query(query => 
          query.Bool(b =>
            b.Should(queryFunc)))
        .Highlight(h => h
        .PreTags($"<{tag}>")
        .PostTags($"</{tag}>")
        .Fields(highFieldFunc)
        )
      );
    }

    public async Task<ISearchResponse<TDocument>> WhereQuery<TDocument>(params QueryContainerDescriptor<TDocument>[] queryContainerDescriptor) where TDocument : class
    {
      //QueryContainerDescriptor<TDocument> queryContainer1 = new QueryContainerDescriptor<TDocument>();
      //queryContainer1.Match(m => m.Field("Field").Query("FieldQuery"));
      //QueryContainerDescriptor<TDocument> queryContainer2 = new QueryContainerDescriptor<TDocument>();
      //queryContainer2.Match(m => m.Field("Field").Query("FieldQuery"));

      var queryResponse = await Instace.SearchAsync<TDocument>(q =>
      q.Size(10)
      .From(0)
      .Query(qu =>
        qu.Bool(b =>
          b.Should(
            //f => f.Match(m => m.Field("title").Query("Test"))
            queryContainerDescriptor
            )
          )
        )
      );
      return queryResponse;
    }

  }
}
