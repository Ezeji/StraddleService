# Project description
This project contains implementation for straddle service. 
Implementations such as creating a new payment transaction, retrieving payment transaction details, cancelling a payment transaction and refunding a transaction.
 
# Project tools
- ASP.NET Core Web API(For the core API)
- MS SQL Server(For database)
- Automapper(For mapping data transfer objects to models and vice versa)
- Azure Service Bus(For asynchronous processing of transactions)
- Xunit & Moq(For unit testing)
- Entity Framework Core(For object relational mapping)

# Project implementation technique
- Dependency Injection
- Clean Architecture

# Scaling techniques
There are various approaches that could be employed to scale this system for high availability and performance.
Some of such includes:
- Database Connection Pooling: This will involve opening the connection stream to the database for a longer period thereby allowing for a reuse of the tcp connection established at an
initial period when connecting to the database. By doing this, the server is effectively transformed to a http 2 based communication protocol server.
- Introducing a load balancer: This will result to having multiple instances of the server whereby traffic are routed accordingly to avoid load on a specific instance. This will invariably
spread the load properly thereby seeing to little or no downtimes.
- Introduce caching where necessary: Introduction of a cache like Redis where necessary will significantly improve the performance of the system. This is because the server will be less burdened
with having to talk to the database for data all the time and can retrieve such data from a distributed system(recommended) which has a copy of the data stored in its memory state. We can appreciate
the usefulness of caching if we try to analyze from a standpoint of its space and time complexity. Most cache stores are implemented with the hash table data structure which has a time and space complexity
of O(1) in the worst case for data retrieval as opposed to a database system(assuming its a relational database) that is implemented with the B+ Tree data structure with a time and space complexity of O(log n). 
- Writing efficient queries(LINQ or SQL): This can significantly improve the performance of the system if a query is written with an understanding of its impact. To achieve optimized queries, the query explainer for SQL
can help give insights into what a query does under the hood.etc
- Minimize database round trips: This is mostly about taking advantage of Entity Framework Core as an ORM tool. An example is a case where a foreign key relationship exists between one or more tables, there's no need persisting individually
to the table as a single call to the database would take care of persisting to all the related tables.
- Database partitioning and/or sharding: Partitioning involves breaking down large record tables into chunks and having a parent table link all the chunks. Database sharding, on the other hand, involves having different database nodes to contain
a certain amount of data records. The databases are identified in a ring through a hash function. Either of the approach when properly done will improve the performance of the system because data retrievals as the table grows do not need to occur at the leaf
node of the database thereby resulting to an increase in the speed of retrieving data.
- The use of ULID(Universally Unique Lexicographically Sortable Identifier): ULID has proven to be a more secured and efficient identifier when compared to UUID. This is because its generation is done based on the timestamp upto milliseconds precision for a data insertion. This way, data
is not only arranged in an ascending order but ensures that data retrieval process is fast.

# Data Security techniques
Some data security approach to be considered for this system are as follows:
- Store secrets, keys and other sensitive data in Key Vault: Key vault such as Azure Key Vault can be used to store sensitive data as opposed to having them on the code. This is more secured and avoids the potential of leaking such data by chance.
- Mask certain customer information such as phone number, email or account number: Masking certain customer information is useful when such data or information is to be displayed on a frontend client as it avoids exposing the data.
- Regularly backup customer data: This can be useful as it avoids disappointments that can occur due to data loss. By backing up customer data in different regions, the safety of such data record is assured.
- Providing a 2FA option as an extra layer of security: This will ensure that the right individual has authorized access to the server resource and invariably the existing data for which a request is sent. 