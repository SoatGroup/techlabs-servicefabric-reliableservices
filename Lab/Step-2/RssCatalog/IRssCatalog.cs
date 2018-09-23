using Microsoft.ServiceFabric.Services.Remoting;
using RssCatalog.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RssCatalog
{
    public interface IRssCatalog : IService
    {
        Task CreateFeed(Blog blog);
        Task<List<Blog>> GetAllBlogs();

        Task<List<RssItem>> GetRssItems(Guid blogId);
    }
}
