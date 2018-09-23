using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using RssCatalog;
using RssCatalog.Model;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RssController : ControllerBase
    {

        private const string rssCatalogUrl = "fabric:/Techlabs/RssCatalog";

        [HttpPost("sample")]
        public async Task<ActionResult> Sample()
        {


            var blogs = new List<Blog>();
            blogs.Add(new Blog() { Id = Guid.NewGuid(), BlogName = "Wilfried Woivré", FeedType = "atom", FeedUrl = "http://blog.woivre.fr/feed.xml", Url = "http://blog.woivre.fr" });
            blogs.Add(new Blog() { Id = Guid.NewGuid(), BlogName = "Michael Fery", FeedType = "atom", FeedUrl = "http://www.mfery.com/feed/atom/", Url = "http://mfery.com" });

            try
            {

                foreach (var blog in blogs)
                {
                    ServicePartitionKey partitionKey = new ServicePartitionKey(blog.Id.GetHashCode());

                    var rssCatalogClient = ServiceProxy.Create<IRssCatalog>(new Uri(rssCatalogUrl), partitionKey);

                    await rssCatalogClient.CreateFeed(blog);
                }


                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }

        [HttpGet("blogs")]
        public async Task<ActionResult<IEnumerable<Blog>>> Get()
        {
            try
            {

                var results = new List<Blog>();
                FabricClient client = new FabricClient();
                ServicePartitionList partitions = await client.QueryManager.GetPartitionListAsync(new Uri(rssCatalogUrl));

                foreach (var partition in partitions)
                {
                    long minKey = (partition.PartitionInformation as Int64RangePartitionInformation).LowKey;
                    var service = ServiceProxy.Create<IRssCatalog>(new Uri(rssCatalogUrl), new ServicePartitionKey(minKey));

                    IEnumerable<Blog> subResult = await service.GetAllBlogs();
                    if (subResult != null)
                    {
                        results.AddRange(subResult);
                    }

                }

                return StatusCode(StatusCodes.Status200OK, results);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }

        [HttpGet("{blogId}")]
        public async Task<ActionResult<IEnumerable<RssItem>>> GetRssItems(Guid blogId)
        {
            try
            {
                ServicePartitionKey partitionKey = new ServicePartitionKey(blogId.GetHashCode());

                var rssCatalogClient = ServiceProxy.Create<IRssCatalog>(new Uri(rssCatalogUrl), partitionKey);

                var results = await rssCatalogClient.GetRssItems(blogId);

                return StatusCode(StatusCodes.Status200OK, results);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }

    }
}