using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProductStore.Models;

namespace ProductStore.Controllers
{
    public class ProductsController : ApiController
    {
        /*Calling new ProductRepository() in the controller is not the best design, 
         * because it ties the controller to a particular implementation of IProductRepository. 
         * For a better approach, see Using the Web API Dependency Resolver.*/
        static readonly IProductRepository repository = new ProductRepository();

        public IEnumerable<Product> GetAllProducts()
        {
            return repository.GetAll();
        }

        public Product GetProduct(int id)
        {
            Product item = repository.Get(id);
            if (item == null)
            {   
                // Translated as a 404 (not found)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return item;
        }

        public IEnumerable<Product> GetProductByCategory(string category)
        {
            // LINQ for Where method
            // Web API tries to match the query parameters to parameters on the controller method
            return repository.GetAll().Where(
                p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase));
        }

        //This example does not validate the Product. For information about model validation, see Model Validation in ASP.NET Web API.
        public HttpResponseMessage PostProduct(Product item)
        {
            item = repository.Add(item);

            //The CreateResponse method creates an HttpResponseMessage 
            // Returning an HttpResponseMessage instead of a Product, we can control the details of the HTTP response message,
            var response = Request.CreateResponse<Product>(HttpStatusCode.Created, item);

            string uri = Url.Link("DefaultApi", new { id = item.Id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        //The method name starts with "Put...", so Web API matches it to PUT requests
        // The method takes two parameters, the product ID and the updated product
        public void PutProduct(int id, Product product)
        {
            product.Id = id;
            if (!repository.Update(product))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        public void DeleteProduct(int id)
        {
            Product item = repository.Get(id);

            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            repository.Remove(id);
        }

    }
}
