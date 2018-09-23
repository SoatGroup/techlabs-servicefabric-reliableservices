using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using RssCatalog.Model;
using System.Xml;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Atom;
using Microsoft.SyndicationFeed.Rss;

namespace RssCatalog
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class RssCatalog : StatefulService, IRssCatalog
    {
        private const string allBlogs = "all_blogs";
        private const string rssItems = "{0}-rss_items";

        public RssCatalog(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        public async Task CreateFeed(Blog blog)
        {
            if (blog == null) { throw new NullReferenceException("blog");  }

            using (var txn = this.StateManager.CreateTransaction())
            {
                var blogs = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Blog>>(allBlogs);
                await blogs.AddAsync(txn, blog.Id.ToString("N"), blog);
                await txn.CommitAsync();
            }

            await ProcessBlog(blog);
        }



        public async Task<List<Blog>> GetAllBlogs()
        {
            using (var txn = this.StateManager.CreateTransaction())
            {
                var result = new List<Blog>();
                var blogs = await this.StateManager.TryGetAsync<IReliableDictionary<string, Blog>>(allBlogs);
                if (blogs.HasValue)
                {
                    var enumerable = await blogs.Value.CreateEnumerableAsync(txn, EnumerationMode.Unordered);
                    var enumerator = enumerable.GetAsyncEnumerator();
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        result.Add(enumerator.Current.Value);
                    }
                }
                return result;
            }
        }

        public async Task<List<RssItem>> GetRssItems(Guid blogId)
        {
            using (var txn = this.StateManager.CreateTransaction())
            {
                var result = new List<RssItem>();
                var items = await this.StateManager.TryGetAsync<IReliableDictionary<string, RssItem>>(string.Format(rssItems, blogId.ToString("N")));
                if (items.HasValue)
                {
                    var enumerable = await items.Value.CreateEnumerableAsync(txn, EnumerationMode.Unordered);
                    var enumerator = enumerable.GetAsyncEnumerator();
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        result.Add(enumerator.Current.Value);
                    }
                }
                return result;
            }
        }



        private async Task ProcessBlog(Blog blog)
        {
            using (var reader = XmlReader.Create(blog.FeedUrl, new XmlReaderSettings { Async = true, DtdProcessing = DtdProcessing.Parse }))
            {
                ISyndicationFeedReader feedReader = null;
                if (blog.FeedType == "atom")
                {
                    feedReader = new AtomFeedReader(reader);
                }
                else
                {
                    feedReader = new RssFeedReader(reader);

                }

                while (await feedReader.Read())
                {

                    switch (feedReader.ElementType)
                    {
                        case SyndicationElementType.Item:
                            ISyndicationItem item = await feedReader.ReadItem();

                            var entity = new RssItem()
                            {
                                Title = item.Title,
                                Link = item.Id,
                            };

                            if (!entity.Link.StartsWith("http"))
                            {
                                entity.Link = blog.Url + item.Id;
                            }

                            using (var txn = this.StateManager.CreateTransaction())
                            {
                                var items = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, RssItem>>(string.Format(rssItems, blog.Id.ToString("N")));
                                await items.AddAsync(txn, Guid.NewGuid().ToString("N"), entity);
                                await txn.CommitAsync();
                            }
                            break;
                        default:
                            break;
                    }

                }
            }
        }
    }

}
