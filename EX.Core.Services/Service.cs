using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EX.Core.Interfaces;

namespace EX.Core.Services
{
    public class Service<T> : IService<T> where T : class
    {
        protected readonly IUnitOfWork _uow;
        private readonly IRepository<T> _repo;

        public Service(IUnitOfWork uow)
        {
            _uow = uow;
            _repo = _uow.GetRepository<T>();
        }

        public void Add(T entity)
        {
            _repo.Add(entity);
            _uow.Save();
        }

        public void Delete(T entity)
        {
            _repo.Delete(entity);
            _uow.Save();
        }

        public T? Get(int id)
        {
            return _repo.Get(id);
        }

        public T? Get(string id)
        {
            return _repo.Get(id);
        }

        public IList<T> GetAll()
        {
            return _repo.GetAll();
        }

        public void Update(T entity)
        {
            _repo.Update(entity);
            _uow.Save();
        }
    }

}
