using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using DFXScheme.Models;

namespace DFXScheme.Repository
{
    public interface IOrders
    {
        IList<OrderCreationModel> GetOrders();

    }
}