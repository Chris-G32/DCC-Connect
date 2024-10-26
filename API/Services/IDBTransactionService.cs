namespace API.Services;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;
public interface IDBTransactionService
{
    public void RunTransaction(Action action);
}
public class DBTransactionService(IDBClientProvider client) : IDBTransactionService
{
    private readonly IDBClientProvider _client = client;
    public void RunTransaction(Action action)
    {
        using (var session =  _client.Client.StartSession())
        {
            session.StartTransaction();
            try
            {
                action();
                session.CommitTransaction();
            }
            catch (Exception e)
            {
                session.AbortTransaction();
                throw;
            }
        }
    }
}