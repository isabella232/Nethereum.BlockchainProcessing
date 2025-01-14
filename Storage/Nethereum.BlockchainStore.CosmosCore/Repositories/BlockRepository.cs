﻿using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Nethereum.BlockchainStore.CosmosCore.Entities;
using Nethereum.BlockchainStore.Entities.Mapping;
using Nethereum.BlockchainStore.Repositories;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public class BlockRepository : CosmosRepositoryBase, IBlockRepository
    {
        public BlockRepository(DocumentClient client, string databaseName) : base(client, databaseName, CosmosCollectionName.Blocks)
        {
        }

        public async Task<BlockchainStore.Entities.IBlockView> FindByBlockNumberAsync(HexBigInteger blockNumber)
        {
            var uri = CreateDocumentUri(new CosmosBlock() {Id = blockNumber.Value.ToString()});
            try
            {
                var response = await Client.ReadDocumentAsync<CosmosBlock>(uri);
                return response.Document;
            }
            catch (DocumentClientException dEx)
            {
                if (dEx.IsNotFound())
                    return null;

                throw;
            }
        }

        public async Task<BigInteger?> GetMaxBlockNumberAsync()
        {
            var countQuery = await Client.CreateDocumentQuery<CosmosBlock>(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName)).CountAsync();

            if (countQuery == 0)
                return null;

            var sqlQuery = "SELECT VALUE MAX(b.BlockNumber) FROM Blocks b";

            var query = Client.CreateDocumentQuery<BigInteger>(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName),
                sqlQuery).AsDocumentQuery();

            while (query.HasMoreResults)
            {
                var results = await query.ExecuteNextAsync();
                var result = results.AsEnumerable().First();
                return result;
            }

            return 0;
        }

        public async Task UpsertBlockAsync(Block source)
        {
            var block = new CosmosBlock { };
            block.Map(source);
            block.UpdateRowDates();
            await UpsertDocumentAsync(block);
        }
    }
}
