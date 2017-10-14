using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Domain.DI;
namespace Infrastructure.ES
{
  public interface IElasticsearchClient:IScopedDependency
  {
    ElasticClient Instace { get; }
    /// <summary>
    /// 批量添加数据
    /// </summary>
    Task<IBulkResponse> Bulk<TDocument>(IEnumerable<TDocument> documents) where TDocument : class;
    /// <summary>
    /// 添加一条文档
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    /// <param name="document"></param>
    /// <returns></returns>
    Task<ICreateResponse> CreateDocument<TDocument>(TDocument document) where TDocument : class;
    /// <summary>
    /// 更新所有字段
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    /// <param name="document"></param>
    /// <returns></returns>
    Task<IUpdateResponse<TDocument>> UpdateDocument<TDocument>(TDocument document) where TDocument : class;
    /// <summary>
    /// 更新部分字段
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    /// <param name="document"></param>
    /// <returns></returns>
    Task<IUpdateResponse<TDocument>> UpdateDocumentPartial<TDocument, TPartialDocument>(TDocument document, object partialDocument)
      where TDocument : class where TPartialDocument : class;
    /// <summary>
    /// 批量更新
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    /// <typeparam name="TPartialDocument"></typeparam>
    /// <param name="documents">需要更新的集合</param>
    /// <returns></returns>
    Task<IBulkResponse> BulkUpdateDocumentPartial<TDocument, TPartialDocument>(IEnumerable<TDocument> documents) 
      where TDocument : class where TPartialDocument : class;
    /// <summary>
    /// 删除文档
    /// </summary>
    /// <param name="indexName"></param>
    /// <param name="typeName"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IDeleteResponse> DeleteDocument(string indexName, string typeName, string id);
    /// <summary>
    /// 删除文档
    /// </summary>
    /// <param name="indexName"></param>
    /// <param name="typeName"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IDeleteResponse> DeleteDocument<TDocument>(TDocument document) where TDocument : class;
    /// <summary>
    /// 查询数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="searchFunc"></param>
    /// <returns></returns>
    Task<ISearchResponse<T>> Query<T>(Func<SearchDescriptor<T>, ISearchRequest> searchFunc) where T : class;
    /// <summary>
    /// 高亮查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="queryFunc"></param>
    /// <param name="tag">标签</param>
    /// <param name="count">每页个数</param>
    /// <param name="from">从第几页开始</param>
    ///     /// <param name="highFieldFunc"></param>
    /// <returns></returns>
    Task<ISearchResponse<T>> HighQuery<T>(
      Func<QueryContainerDescriptor<T>, QueryContainer>[] queryFunc,
      string tag = "em", int count = 10, int from = 1,
      params Func<HighlightFieldDescriptor<T>, IHighlightField>[] highFieldFunc) where T : class;
    /// <summary>
    /// 多字段查询
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    /// <param name="queryContainerDescriptor"></param>
    /// <returns></returns>
    Task<ISearchResponse<TDocument>> WhereQuery<TDocument>(params QueryContainerDescriptor<TDocument>[] queryContainerDescriptor) where TDocument : class;


  }

}
