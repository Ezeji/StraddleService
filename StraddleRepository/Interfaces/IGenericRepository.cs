using Microsoft.EntityFrameworkCore;
using StraddleData.Enums;
using StraddleRepository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StraddleRepository.Interfaces
{
    public interface IGenericRepository<T> where T : class, new()
    {
        Task<int> BulkCreateAsync(IEnumerable<T> entities, bool isSave = true);

        Task<int> BulkDeleteAsync(IEnumerable<int> ids, bool isSave = true);

        Task<int> CreateAsync(T entity, bool isSave = true);

        Task<int> DeleteAsync(int id, bool isSave = true);

        Task<bool> EntityExistsAsync(int id);

        Task<T> GetByIdAsync(int id);

        Task<T> GetByGuidAsync(Guid id);

        Task<int> UpdateAsync(T entity, bool isSave = true);

        Task<int> SaveChangesToDbAsync();

        #region Non Async

        int BulkCreate(IEnumerable<T> entities, bool isSave = true);

        int BulkDelete(IEnumerable<int> ids, bool isSave = true);

        int Create(T entity, bool isSave = true);

        int Delete(int id, bool isSave = true);

        //Task<bool> EntityExists(T entity, int id);
        T GetById(int id);

        T GetByGuid(Guid id);

        int Update(T entity, bool isSave = true);

        bool EntityExists(int id);

        int SaveChangesToDb();

        #endregion Non Async

        IQueryable<T> GetAll();

        DbSet<T> GetDbSet();

        IQueryable<T> Query();

        Task<List<T>> TakeAndSkipAsync(IQueryable<T> data, int pageSize, int pageIndex);

        Task<PaginatedList<T>> SortPaginateByTextAsync(int pageNumber, int pageSize, IQueryable<T> data, Expression<Func<T, string>> expression, Order order);

        Task<PaginatedList<T>> SortPaginateByDateAsync(int pageNumber, int pageSize, IQueryable<T> data, Expression<Func<T, DateTime>> expression, Order order);

        IQueryable<T> OrderByText(IQueryable<T> data, Order order, Expression<Func<T, string>> expression);

        IQueryable<T> OrderByDate(IQueryable<T> data, Order order, Expression<Func<T, DateTime>> expression);
    }

}
